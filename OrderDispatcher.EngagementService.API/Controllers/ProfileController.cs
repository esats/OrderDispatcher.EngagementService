using Microsoft.AspNetCore.Mvc;
using OrderDispatcher.EngagementService.API.Base;
using OrderDispatcher.EngagementService.API.Entities;
using OrderDispatcher.EngagementService.API.Models;
using OrderDispatcher.EngagementService.API.Services;

namespace OrderDispatcher.EngagementService.API.Controllers
{
    [ApiController]
    [Route("api/engagement/profile")]
    public class ProfileController : APIControllerBase
    {
        private readonly ProfileService _profileService;

        public ProfileController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost("save")]
        public async Task<Response<Profile>> Save([FromBody] ProfileSaveModel request)
        {
            var userId = GetUser();
            return await _profileService.UpdateAsync(request, userId);
        }

        [HttpGet("getOne/{userId}")]
        public async Task<Response<Profile>> GetOne([FromRoute] string userId, CancellationToken ct)
        {
            return await _profileService.GetOneAsync(userId, ct);
        }

        [HttpPost("saveAddress")]
        public async Task<Response<Address>> SaveAddress([FromBody] AddressSaveModel request)
        {
            var userId = GetUser();
            return await _profileService.SaveAddressAsync(request, userId);
        }

        [HttpGet("getAddress/{addressId:int}")]
        public async Task<Response<Address>> GetAddress([FromRoute] int addressId, CancellationToken ct)
        {
            var userId = GetUser();
            return await _profileService.GetAddressAsync(addressId, userId, ct);
        }

        [HttpGet("getAllAddresses")]
        public async Task<Response<List<Address>>> GetAllAddresses(CancellationToken ct)
        {
            var userId = GetUser();
            return await _profileService.GetAddressesAsync(userId, ct);
        }

        [HttpPost("getStoresProfile")]
        public async Task<List<Profile>> GetStoreProfiles(IReadOnlyCollection<string> storeIds)
        {
            return await _profileService.GetStoreProfiles(storeIds);
        }
    }
}
