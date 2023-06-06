using Brainzz.Models;

namespace Brainzz.Data
{
    public class Data
    {
        public static void SeedData(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<Db>();

            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    Password = "admin123"
                });
                context.SaveChanges();
            }
        }
    }
}
