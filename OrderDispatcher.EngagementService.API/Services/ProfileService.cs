using Dapper;
using Microsoft.EntityFrameworkCore;
using OrderDispatcher.EngagementService.API.Entities;
using OrderDispatcher.EngagementService.API.Models;
using System.Data;

namespace OrderDispatcher.EngagementService.API.Services
{
    public class ProfileService
    {
        private readonly EngagementDbContext _db;

        public ProfileService(EngagementDbContext db)
        {
            _db = db;
        }

        public async Task<Response<Profile>> UpdateAsync(ProfileSaveModel request, string userId)
        {
            var response = new Response<Profile>();

            try
            {
                if (request is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid payload.";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    return response;
                }

                await using var connection = _db.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                var now = DateTime.UtcNow;
                var profile = await connection.QuerySingleOrDefaultAsync<Profile>(
                    @"SELECT Id, UserId, FirstName, LastName, PhoneNumber, ImageMasterId, CreatedAtUtc, UpdatedAtUtc
                      FROM Profiles
                      WHERE UserId = @UserId",
                    new { UserId = userId });

                if (profile is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Profile not found.";
                    return response;
                }

                var updateSql = @"UPDATE Profiles
                                  SET FirstName = @FirstName,
                                      LastName = @LastName,
                                      PhoneNumber = @PhoneNumber,
                                      ImageMasterId = @ImageMasterId,
                                      UpdatedAtUtc = @UpdatedAtUtc
                                  WHERE UserId = @UserId";

                await connection.ExecuteAsync(updateSql, new
                {
                    UserId = userId,
                    FirstName = request.FirstName?.Trim(),
                    LastName = request.LastName?.Trim(),
                    PhoneNumber = request.PhoneNumber?.Trim(),
                    ImageMasterId = request.ImageMasterId,
                    UpdatedAtUtc = now
                });

                profile.FirstName = request.FirstName?.Trim();
                profile.LastName = request.LastName?.Trim();
                profile.PhoneNumber = request.PhoneNumber?.Trim();
                profile.ImageMasterId = request.ImageMasterId;
                profile.UpdatedAtUtc = now;

                response.Value = profile;
                response.IsSuccess = true;
                if (string.IsNullOrWhiteSpace(response.Message))
                {
                    response.Message = "Profile updated.";
                }
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

        public async Task<Response<Profile>> GetOneAsync(string userId, CancellationToken ct)
        {
            var response = new Response<Profile>();

            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    return response;
                }

                await using var connection = _db.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(ct);
                }

                var profile = await connection.QuerySingleOrDefaultAsync<Profile>(
                    @"SELECT *
                      FROM Profiles
                      WHERE UserId = @UserId",
                    new { UserId = userId });

                response.Value = profile;
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

        public async Task<List<Profile>> GetStoreProfiles(IReadOnlyCollection<string> userIds)
        {
            var response = new List<Profile>();

            try
            {
                await using var connection = _db.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                var profiles = await connection.QueryAsync<Profile>(
                              @"SELECT *
                                      FROM Profiles
                                      WHERE UserId IN @UserIds",
                                              new { UserIds = userIds });


                return profiles.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Response<Address>> SaveAddressAsync(AddressSaveModel request, string userId)
        {
            var response = new Response<Address>();

            try
            {
                if (request is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid payload.";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(userId))
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    return response;
                }

                var title = request.Title?.Trim();
                var addressLine = request.Address?.Trim();

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(addressLine))
                {
                    response.IsSuccess = false;
                    response.Message = "Title and address are required.";
                    return response;
                }

                await using var connection = _db.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                var now = DateTime.UtcNow;
                var insertSql = @"INSERT INTO Addresses (UserId, Title, AddressLine, CreatedAtUtc, UpdatedAtUtc)
                                  VALUES (@UserId, @Title, @AddressLine, @CreatedAtUtc, @UpdatedAtUtc);
                                  SELECT CAST(SCOPE_IDENTITY() AS int);";

                var newId = await connection.ExecuteScalarAsync<int>(insertSql, new
                {
                    UserId = userId,
                    Title = title,
                    AddressLine = addressLine,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                });

                var address = new Address
                {
                    Id = newId,
                    UserId = userId,
                    Title = title!,
                    AddressLine = addressLine!,
                    CreatedAtUtc = now,
                    UpdatedAtUtc = now
                };

                response.Value = address;
                response.IsSuccess = true;
                response.Message = "Address saved.";
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

        public async Task<Response<Address>> GetAddressAsync(int addressId, string userId, CancellationToken ct)
        {
            var response = new Response<Address>();

            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    return response;
                }

                await using var connection = _db.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(ct);
                }

                var address = await connection.QuerySingleOrDefaultAsync<Address>(
                    @"SELECT Id, UserId, Title, AddressLine, CreatedAtUtc, UpdatedAtUtc
                      FROM Addresses
                      WHERE Id = @AddressId AND UserId = @UserId",
                    new { AddressId = addressId, UserId = userId });

                response.Value = address;
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

        public async Task<Response<List<Address>>> GetAddressesAsync(string userId, CancellationToken ct)
        {
            var response = new Response<List<Address>>();

            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    response.IsSuccess = false;
                    response.Message = "User not found.";
                    return response;
                }

                await using var connection = _db.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(ct);
                }

                var addresses = await connection.QueryAsync<Address>(
                    @"SELECT Id, UserId, Title, AddressLine, CreatedAtUtc, UpdatedAtUtc
                      FROM Addresses
                      WHERE UserId = @UserId
                      ORDER BY UpdatedAtUtc DESC",
                    new { UserId = userId });

                response.Value = addresses.ToList();
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
