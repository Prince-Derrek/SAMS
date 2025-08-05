using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SamsApi.Authorization;
using SamsApi.Data;
using SamsApi.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;


namespace SamsApi.UnitTests.Authorization
{
    public class PolicyHandlerTest
    {
        [Fact]
        public async Task Should_Succeed_When_UserHasRequiredRolePolicy()
        {
            // Arrange
            var requirement = new PolicyRequirement("CanViewUsers");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb_Policy_Success")
                .Options;
            var dbContext = new AppDbContext(options);

            // Seed Role, Policy, and RolePolicy
            var role = new Role { Id = 1, Name = "Admin" };
            var policy = new Policy { Id = 1, Name = "CanViewUsers" };
            var rolePolicy = new RolePolicy { RoleId = 1, PolicyId = 1, Role = role, Policies = policy };

            dbContext.Roles.Add(role);
            dbContext.Policies.Add(policy);
            dbContext.RolePolicies.Add(rolePolicy);
            dbContext.SaveChanges();

            // Create user with matching role claim
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, "Admin")
                }, "mock"));

            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null
            );

            var loggerMock = new Mock<ILogger<PolicyHandler>>();
            var handler = new PolicyHandler(dbContext, loggerMock.Object);

            // Act
            await handler.HandleAsync(context);

            // Assert
            context.HasSucceeded.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Fail_When_UserDoesNotHaveRequiredRolePolicy()
        {
            // Arrange
            var requirement = new PolicyRequirement("CanViewUsers");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb_Policy_Fail")
                .Options;
            var dbContext = new AppDbContext(options);

            // Seed Role and Policy but mismatch role claim
            var role = new Role { Id = 1, Name = "Admin" };
            var policy = new Policy { Id = 1, Name = "CanViewUsers" };
            var rolePolicy = new RolePolicy { RoleId = 1, PolicyId = 1, Role = role, Policies = policy };

            dbContext.Roles.Add(role);
            dbContext.Policies.Add(policy);
            dbContext.RolePolicies.Add(rolePolicy);
            dbContext.SaveChanges();

            // Create user with a different role claim
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, "User") // Doesn't match
                }, "mock"));

            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                user,
                null
            );

            var loggerMock = new Mock<ILogger<PolicyHandler>>();
            var handler = new PolicyHandler(dbContext, loggerMock.Object);

            // Act
            await handler.HandleAsync(context);

            // Assert
            context.HasSucceeded.Should().BeFalse();
        }

    }
}
