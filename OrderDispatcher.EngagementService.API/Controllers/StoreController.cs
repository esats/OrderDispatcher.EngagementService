using Microsoft.AspNetCore.Mvc;
using OrderDispatcher.EngagementService.API.Base;
using OrderDispatcher.EngagementService.API.Entities;
using OrderDispatcher.EngagementService.API.Models;
using OrderDispatcher.EngagementService.API.Services;

namespace OrderDispatcher.EngagementService.API.Controllers
{
    [ApiController]
    [Route("api/engagement/store")]
    public class StoreController : APIControllerBase
    {
        private readonly StoreService _storeService;

        public StoreController(StoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet("getAll")]
        public async Task<Response<List<Profile>>> GetAll(CancellationToken ct)
        {
            return await _storeService.GetAllStoresAsync(ct);
        }
    }
}
