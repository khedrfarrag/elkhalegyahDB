using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.Repositories.Base;

namespace Alkhaligya.DAL.Repositories
{
    public interface IUserFavoriteRepository : IRepository<UserFavorite, int>
    {
        Task<List<UserFavorite>> GetUserFavoritesAsync(string userId);
        Task<UserFavorite> GetUserFavoriteProductAsync(string userId, int productId);
    }
}