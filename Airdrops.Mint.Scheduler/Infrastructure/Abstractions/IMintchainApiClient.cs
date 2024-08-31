using Airdrops.Mint.Scheduler.Models.Dtos;
using Refit;

namespace Airdrops.Mint.Scheduler.Infrastructure.Abstractions
{
    public interface IMintchainApiClient
    {
        [Get("/api/tree/energy-list")]
        Task<EnergyResponse> FetchEnergyAsync([Authorize("Bearer")] string token);

        [Get("/api/tree/user-info")]
        Task<UserInfoResponse> FetchUserInfoAsync([Authorize("Bearer")] string token);

        [Post("/api/tree/claim")]
        Task<ClaimResponse> ClaimEnergyAsync([Authorize("Bearer")] string token, [Body] ClaimRequest claimRequest);

        [Post("/api/tree/inject")]
        Task<InjectResponse> InjectEnergyAsync([Authorize("Bearer")] string token, [Body] InjectRequest injectRequest);
    }
}
