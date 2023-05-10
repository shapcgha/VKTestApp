using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VKApp.DB;
using VKApp.models;
using VKApp.Services;
using Xunit;

namespace VKApp.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task RegisterUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var context = new ApplicationContext(options);
            var user = new User { Login = "testuser", Password = "test", UserGroupId = 2 };
            var cache = new MemoryCache(new MemoryCacheOptions());

            var service = new UserService(context, cache);

            // Act
            var result = await service.RegisterUser(user);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Register2Admin()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var context = new ApplicationContext(options);
            var admin1 = new User { Login = "testuser1", Password = "test", UserGroupId = 1 };
            var admin2 = new User { Login = "testuser2", Password = "test", UserGroupId = 1 };
            var cache = new MemoryCache(new MemoryCacheOptions());

            var service = new UserService(context, cache);

            // Act
            var result1 = await service.RegisterUser(admin1);
            var result2 = await service.RegisterUser(admin2);
            // Assert
            Assert.NotNull(result1);
            Assert.Null(result2);
        }

        [Fact]
        public async Task RegisterWithSimilarLogin()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var context = new ApplicationContext(options);
            var user2 = new User { Login = "testuser", Password = "test", UserGroupId = 2 };
            var user1 = new User { Login = "testuser", Password = "test", UserGroupId = 2 };
            var cache = new MemoryCache(new MemoryCacheOptions());

            var service = new UserService(context, cache);

            // Act
            var result1 = await service.RegisterUser(user1);
            var result2 = await service.RegisterUser(user2);
            Thread.Sleep(5000);
            var result3 = await service.RegisterUser(user2);

            // Assert
            Assert.NotNull(result1);
            Assert.Null(result2);
            Assert.NotNull(result3);
        }

        [Fact]
        public async Task GetUsers()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var context = new ApplicationContext(options);
            var user1 = new User { Login = "testuser1", Password = "test", UserGroupId = 2 };
            var user2 = new User { Login = "testuser2", Password = "test", UserGroupId = 2 };
            var user3 = new User { Login = "testuser3", Password = "test", UserGroupId = 2 };
            var cache = new MemoryCache(new MemoryCacheOptions());

            var service = new UserService(context, cache);

            // Act
            var result0 = await service.GetAllUsersAsync(1, 0);

            await service.RegisterUser(user1);
            await service.RegisterUser(user2);
            await service.RegisterUser(user3);

            var result1 = await service.GetAllUsersAsync(1, 0);
            var result2 = await service.GetAllUsersAsync(1, 2);
            var result3 = await service.GetUserByIdAsync(1);
            var result4 = await service.GetUserByIdAsync(4);

            // Assert
            Assert.Equal(result0.Count(), 0);
            Assert.Equal(result1.Count(), 3);
            Assert.Equal(result2.Count(), 2);
            Assert.NotNull(result3);
            Assert.Null(result4);
        }

        [Fact]
        public async Task DeleteUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var context = new ApplicationContext(options);
            var user1 = new User { Login = "testuser1", Password = "test", UserGroupId = 2 };
            var cache = new MemoryCache(new MemoryCacheOptions());

            var service = new UserService(context, cache);

            // Act

            var user = await service.RegisterUser(user1);
            var result1 = (await service.GetUserByIdAsync(user.Id)).UserState.Code;
            await service.DeleteUserByIdAsync(user.Id);
            var result2 = (await service.GetUserByIdAsync(user.Id)).UserState.Code;

            // Assert
            Assert.NotNull(result1);
            Assert.Equal("Active", result1);
            Assert.NotNull(result2);
            Assert.Equal("Blocked", result2);
        }

    }
}
