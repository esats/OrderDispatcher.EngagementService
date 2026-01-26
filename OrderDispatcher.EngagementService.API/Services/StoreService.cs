using Dapper;
using Microsoft.EntityFrameworkCore;
using OrderDispatcher.EngagementService.API.Entities;
using OrderDispatcher.EngagementService.API.Models;
using System.Data;

namespace OrderDispatcher.EngagementService.API.Services
{
    public class StoreService
    {
        private const int StoreUserType = 3;
        private readonly EngagementDbContext _db;

        public StoreService(EngagementDbContext db)
        {
            _db = db;
        }

        public async Task<Response<List<Profile>>> GetAllStoresAsync(CancellationToken ct)
        {
            var response = new Response<List<Profile>>();

            try
            {
                await using var connection = _db.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(ct);
                }

                var stores = await connection.QueryAsync<Profile>(
                    @"SELECT Id, UserId, FirstName, LastName, PhoneNumber, Email, UserName, ImageMasterId, CreatedAtUtc, UpdatedAtUtc
                      FROM Profiles
                      WHERE UserType = @UserType
                      ORDER BY UpdatedAtUtc DESC",
                    new { UserType = StoreUserType });

                response.Value = stores.ToList();
                response.IsSuccess = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Exception = ex;
                response.IsSuccess = false;
                response.Message = "An unexpected error occurred.";
                return response;
            }
        }
    }
}
