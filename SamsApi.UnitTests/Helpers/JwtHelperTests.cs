using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SamsApi.Helpers;
using SamsApi.Models;
using Xunit;

namespace SamsApi.UnitTests.Helpers
{
    public class JwtHelperTests
    {
        [Fact]
        public void GenerateToken_ShouldReturnValidToken()
        {
            var inMemorySettings = new Dictionary<string, string> // dummy values
            {
                {"Jwt:Key", "dummy_secret_key_1234567890_abcd" },
                {"Jwt:Issuer", "TestIssuer" },
                {"Jwt:Audience", "TestAudience" },
                {"Jwt:DurationInMinutes", "60" }
            };

            var configuration = new ConfigurationBuilder() // Mock IConfiguration
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            var loggerMock = new Mock<ILogger<JwtHelper>>(); // Mock ILogger
            var helper = new JwtHelper(configuration, loggerMock.Object);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Role = new Role { Name = "Admin" }
            };

            var (token, expiresAt) = helper.GenerateToken(user);

            token.Should().NotBeNullOrWhiteSpace();
            token.Split('.').Length.Should().Be(3);
            expiresAt.Should().BeAfter(DateTime.UtcNow);
        }
    }
}
