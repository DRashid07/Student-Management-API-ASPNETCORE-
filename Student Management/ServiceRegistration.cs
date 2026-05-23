using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Student_Management.Data;
using Student_Management.Models;
using System.Text;

namespace Student_Management
{
    public static class ServiceRegistration
    {
        public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"] ?? "superSecretKey@345")),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        Console.WriteLine($"[JWT-FAIL] {ctx.Exception.GetType().Name}: {ctx.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = ctx =>
                    {
                        Console.WriteLine($"[JWT-CHALLENGE] error={ctx.Error} desc={ctx.ErrorDescription}");
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = ctx =>
                    {
                        var auth = ctx.Request.Headers["Authorization"].ToString();
                        Console.WriteLine($"[JWT-RECV] Authorization header: '{auth}'");
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
