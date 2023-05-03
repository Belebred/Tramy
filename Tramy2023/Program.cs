
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

using Tramy.Backend.Auth;
using System.Reflection;
using Tramy.Backend.Data.DBServices;
using Tramy.Backend.Hubs;
using Tramy.Common.Users;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Tramy.Common.Locations;
using Tramy.Backend.Extensions;

namespace Tramy.Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            builder.Services.ConfigureDB();

            var app = builder.Build();

            await app.CreateInitialData();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();
            app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthorization();

            app.Run();
        }

        

        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            var services = builder.Services;
            services.AddCors();
            services.AddProblemDetails();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = AuthOptions.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                    options.SaveToken = true;
                });


            services.AddHttpClient();

            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tramy Backend and Common", Version = "v1" });
                var filePath = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, ".xml");
                c.IncludeXmlComments(filePath);
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tramy.Common.xml");
                c.IncludeXmlComments(filePath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            services.AddSwaggerGenNewtonsoftSupport();

            //add MongoDB services
            services.AddSingleton<SystemEventService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<RoleService>();
            services.AddSingleton<DeviceService>();
            services.AddSingleton<OrganizationService>();
            services.AddSingleton<LocationService>();
            services.AddSingleton<LocationPartService>();
            services.AddSingleton<BuildingService>();
            services.AddSingleton<FloorService>();
            services.AddSingleton<MeasuredPointService>();
            services.AddSingleton<NotificationService>();
            services.AddSingleton<ChatService>();
            services.AddSingleton<BunchService>();
            services.AddSingleton<TrackerService>();
            services.AddSingleton<UserEventService>();
            services.AddSingleton<HikeService>();
            services.AddSingleton<BaseLogService>();



            services.AddSignalR();

            services.AddSingleton<ChatHub>();
            services.AddSingleton<NotiHub>();

            //classes for ignore extra fields
            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }

    }
}