using Alkhaligya.DAL.Data.DbHelper;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Alkhaligya.DAL.Repositories
{
    public class UserFavoriteRepository : Repository<UserFavorite, int>, IUserFavoriteRepository
    {
        public UserFavoriteRepository(AlkhligyaContext context) : base(context)
        {
        }

        public async Task<List<UserFavorite>> GetUserFavoritesAsync(string userId)
        {
            return await _context.UserFavorites
                .Include(uf => uf.Product)
                .Where(uf => uf.UserId == userId && !uf.IsDeleted)
                .ToListAsync();
        }

        public async Task<UserFavorite> GetUserFavoriteProductAsync(string userId, int productId)
        {
            return await _context.UserFavorites
                .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.ProductId == productId && !uf.IsDeleted);
        }
    }
}