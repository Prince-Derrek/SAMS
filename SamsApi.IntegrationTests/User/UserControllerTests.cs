using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SamsApi.Data;
using SamsApi.Helpers;
using SamsApi.Models;
using System.Net;
using System.Net.Http.Headers;

namespace SamsApi.IntegrationTests.Authorization
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UserControllerTests(WebApplicationFactory<Program> factory)
        {
            var dbName = Guid.NewGuid().ToString();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(dbName);
                    });

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    // Seed roles & policies
                    var roleWithAccess = new Role { Id = 1, Name = "Admin" };
                    var roleWithoutAccess = new Role { Id = 2, Name = "Viewer" };
                    var policyCanViewUsers = new Policy { Id = 1, Name = "CanViewUsers" };

                    var rolePolicy = new RolePolicy { RoleId = 1, PolicyId = 1, Role = roleWithAccess, Policies = policyCanViewUsers };

                    db.Roles.AddRange(roleWithAccess, roleWithoutAccess);
                    db.Policies.Add(policyCanViewUsers);
                    db.RolePolicies.Add(rolePolicy);

                    // Seed users
                    var authorizedUser = new SamsApi.Models.User
                    {
                        Id = Guid.NewGuid(),
                        UserName = "AuthorizedUser",
                        UserSecret = "Secret123",
                        isActive = true,
                        RoleId = 1,
                        Role = roleWithAccess
                    };

                    var unauthorizedUser = new SamsApi.Models.User
                    {
                        Id = Guid.NewGuid(),
                        UserName = "UnauthorizedUser",
                        UserSecret = "Secret456",
                        isActive = true,
                        RoleId = 2,
                        Role = roleWithoutAccess
                    };

                    db.Users.AddRange(authorizedUser, unauthorizedUser);
                    db.SaveChanges();
                });
            });
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOk_WhenUserHasPolicy()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var jwtHelper = scope.ServiceProvider.GetRequiredService<JwtHelper>();

            var authorizedUser = db.Users.Include(u => u.Role)
                                         .ThenInclude(r => r.RolePolicies)
                                         .ThenInclude(rp => rp.Policies)
                                         .First(u => u.Role.Name == "Admin");

            var (token, _) = jwtHelper.GenerateToken(authorizedUser);
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.GetAsync("/api/user");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnForbidden_WhenUserLacksPolicy()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var jwtHelper = scope.ServiceProvider.GetRequiredService<JwtHelper>();

            var unauthorizedUser = db.Users.Include(u => u.Role)
                                           .ThenInclude(r => r.RolePolicies)
                                           .ThenInclude(rp => rp.Policies)
                                           .First(u => u.Role.Name == "Viewer");

            var (token, _) = jwtHelper.GenerateToken(unauthorizedUser);
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await client.GetAsync("/api/user");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
