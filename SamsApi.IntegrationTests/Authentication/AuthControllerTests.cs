using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SamsApi;
using SamsApi.Data;
using SamsApi.DTOs;
using SamsApi.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace SamsApi.IntegrationTests.Authentication
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private Guid _seededUserId;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbName = Guid.NewGuid().ToString();

                    // Remove the real DB context
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Use a unique DB name for every test run
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(dbName);
                    });

                    // Build service provider
                    var sp = services.BuildServiceProvider();

                    // Seed the database
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    var role = new Role { Id = 1, Name = "Admin" };
                    var policy = new Policy { Id = 1, Name = "CanViewUsers" };
                    var rolePolicy = new RolePolicy { RoleId = 1, PolicyId = 1, Role = role, Policies = policy };

                    var userId = Guid.NewGuid();
                    var user = new SamsApi.Models.User
                    {
                        Id = userId,
                        UserName = "TestUser",
                        UserSecret = "TestSecret",
                        isActive = true,
                        RoleId = 1,
                        Role = role
                    };

                    db.Roles.Add(role);
                    db.Policies.Add(policy);
                    db.RolePolicies.Add(rolePolicy);
                    db.Users.Add(user);
                    db.SaveChanges();

                    // Store seeded user ID so we can use it in tests
                    _seededUserId = userId;
                });
            });
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var loginRequest = new LoginRequestDTO
            {
                Id = _seededUserId.ToString(),
                UserSecret = "TestSecret"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();
            loginResponse.Should().NotBeNull();
            loginResponse!.Token.Should().NotBeNullOrWhiteSpace();
            loginResponse!.expiresAt.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var loginRequest = new LoginRequestDTO
            {
                Id = Guid.NewGuid().ToString(), // Nonexistent
                UserSecret = "WrongSecret"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
