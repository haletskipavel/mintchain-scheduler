using Airdrops.Mint.Scheduler.Infrastructure.Abstractions;
using Airdrops.Mint.Scheduler.Models.Dtos;
using Airdrops.Mint.Scheduler.Settings;
using Quartz;

namespace Airdrops.Mint.Scheduler
{
    public class MintchainJob : IJob
    {
        private readonly IMintchainApiClient _mintchainApiClient;
        private readonly ILogger<MintchainJob> _logger;
        private readonly UserSettings _userSettings;

        public MintchainJob(
            IMintchainApiClient mintchainApiClient,
            ILogger<MintchainJob> logger,
            UserSettings userSettings)
        {
            _mintchainApiClient = mintchainApiClient;
            _logger = logger;
            _userSettings = userSettings;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var energyList = await _mintchainApiClient.FetchEnergyAsync(_userSettings.AccessToken);
                _logger.LogInformation("Energy list retrieved with {Count} items", energyList.Result.Count);

                foreach (var energy in energyList.Result)
                {
                    if (energy.Freeze)
                    {
                        _logger.LogInformation("Skipping frozen energy of {Amount}", energy.Amount);
                        continue;
                    }

                    await _mintchainApiClient.ClaimEnergyAsync(_userSettings.AccessToken, new ClaimRequest
                    {
                        Uid = energy.Uid,
                        Amount = energy.Amount,
                        Includes = energy.Includes,
                        Type = energy.Type,
                        Id = $"{energy.Amount}_"
                    });

                    _logger.LogInformation("Claimed {Amount} energy for wallet {Address}", energy.Amount, _userSettings.Address);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while running the MintChain job");
            }
        }
    }
}
