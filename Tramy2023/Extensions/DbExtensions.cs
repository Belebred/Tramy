using MongoDB.Driver;
using Tramy.Backend.Data.DBServices;
using Tramy.Common.Locations;
using Tramy.Common.Users;

namespace Tramy.Backend.Extensions
{
    public static class DbExtensions
    {
        internal static IServiceCollection ConfigureDB(this IServiceCollection services)
        {
            services.AddOptions().Configure<IServiceProvider>((provider) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                try
                {
                    var mongoClient = new MongoClient(configuration["MongoConnection"]);
                    var mainDatabase = mongoClient.GetDatabase(configuration["MongoMainDB"]);

                    var locationCollection = mainDatabase.GetCollection<Location>(nameof(Location));
                    var indexLocationKeysDefinition = Builders<Location>.IndexKeys.Geo2DSphere(g => g.GeoPoint);
                    locationCollection.Indexes.CreateOne(indexLocationKeysDefinition);

                    var userCollection = mainDatabase.GetCollection<User>(nameof(User));
                    var indexUserKeysDefinition = Builders<User>.IndexKeys.Geo2DSphere(g => g.GeoPoint);
                    userCollection.Indexes.CreateOne(indexUserKeysDefinition);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SOS");
                }
            });
            return services;
        }

        /// <summary>
        /// Create initial data in DB
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        internal static async Task CreateInitialData(this IApplicationBuilder app)
        {
            var userService = app.ApplicationServices.GetRequiredService<UserService>();
            //check administrator account
            var user = await userService.GetByEmail("admin@admin.ru");
            if (user != null) return;
            //insert if not exists
            user = new User()
            {
                AccountType = AccountType.Administration,
                Email = "admin@admin.ru",
                FirstName = "Admin",
                LastName = "Admin",
                RegStatus = RegStatus.Active,
                Gender = Gender.Man,
                PasswordHash = UserService.CreateMd5("A12345!")
            };
            await userService.Insert(user);
            return;
        }
    }
}
