using SamsApi.Models;

namespace SamsApi.Data
{
    public class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                var AdminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");

                if (AdminRole == null)
                    throw new Exception("Missing Roles. Ensure Roles are seeded");

                var users = new User
                {
                    Id = Guid.NewGuid(),
                    UserSecret = GenerateSecret(),
                    UserName = "TestAdmin",
                    Role = AdminRole,
                    RoleId = 1,
                    isActive = true,
                    CreatedAt = DateTime.UtcNow.AddHours(3)
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }

            string GenerateSecret()
            {
                return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                    .Replace("=", "")
                    .Replace("+", "")
                    .Substring(0, 16);
            }
        }
    }
}
