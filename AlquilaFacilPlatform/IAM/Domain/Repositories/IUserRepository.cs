using AlquilaFacilPlatform.IAM.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.IAM.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync (string email);
    Task<User?> FindByUsername(string username);
    Task<bool> ExistsByUsername(string username);
    Task<string?> GetUsernameByIdAsync(int userId);
    bool ExistsById(int userId);
}