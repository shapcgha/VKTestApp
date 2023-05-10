using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VKApp.DB;
using VKApp.models;

namespace VKApp.Services
{
    public class UserService
    {
        ApplicationContext db;
        IMemoryCache cache;
        public UserService(ApplicationContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }
        public async Task<User?> RegisterUser(User user)
        {
            // пытаемся получить данные из кэша
            cache.TryGetValue(user.Login, out string? registerLogin);
            // если данные не найдены в кэше
            if (registerLogin != null)
            {
                return null;
            }
            if (user.UserGroupId == 1 && db.Users.Any(u => u.UserGroupId == 1))
            {
                return null;
            }
            user.UserStateId = 1;
            user.Created_date = DateTime.UtcNow;
            db.Users.Add(user);
            await db.SaveChangesAsync();
            cache.Set(user.Login, user.Login, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(5)));
            return user;
        }

        public async Task<ICollection<User>> GetAllUsersAsync()
        {
            return await db.Users.Include(u => u.UserGroup).Include(u => u.UserState).ToListAsync();
        }

        public async Task<ICollection<User>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            if (pageSize == 0)
            {
                return await GetAllUsersAsync();
            }
            return await db.Users.Include(u => u.UserGroup).Include(u => u.UserState)
                .Skip(((pageNumber - 1) * pageSize)).Take(pageSize).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await db.Users.Include(u => u.UserGroup).Include(u => u.UserState).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> DeleteUserByIdAsync(int id)
        {
            var user = await db.Users.Include(u => u.UserGroup).Include(u => u.UserState).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return null;
            }

            user.UserStateId = 2;
            await db.SaveChangesAsync();
            return user;
        }
    }
}
