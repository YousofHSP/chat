using Common;
using Common.Exceptions;
using Common.Utilities;
using Data.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;
using Data;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebFramework.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
        }
        public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
                var encryptKey = Encoding.UTF8.GetBytes(jwtSettings.EncryptKey);
                
                
                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    RequireExpirationTime = false,
                    ValidateLifetime = false,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptKey),
                };
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("Authentication failed.", context.Exception);

                        throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed.",
                            HttpStatusCode.Unauthorized, context.Exception, null!);

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var signInManager =
                            context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                        var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                        if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
                        {
                            if (claimsIdentity.Claims?.Any() != true)
                                context.Fail("This token has no claims.");

                            var securityStamp =
                                claimsIdentity?.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                            if (!securityStamp.HasValue())
                                context.Fail("This token has no security stamp");

                            //Find user and token from database and perform your custom validation
                            var userId = claimsIdentity.GetUserId<int>();
                            var user = await userRepository.GetByIdAsync(context.HttpContext.RequestAborted, userId);

                            var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                            if (validatedUser == null)
                                context.Fail("Token security stamp is not valid.");

                            if (user.Status.Equals(UserStatus.Disable))
                                context.Fail("User is not active.");

                            await userRepository.UpdateLastLoginDateAsync(user, context.HttpContext.RequestAborted);
                        }
                    },
                    OnChallenge = context =>
                    {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("OnChallenge error", context.Error, context.ErrorDescription);

                        if (context.AuthenticateFailure != null)
                            throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.",
                                HttpStatusCode.Unauthorized, context.AuthenticateFailure, null!);
                        throw new AppException(ApiResultStatusCode.UnAuthorized,
                            "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);

                        //return Task.CompletedTask;
                    }
                };
            });
        }

        public static void AddElmah(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddElmah<SqlErrorLog>(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("Elmah");
                //options.OnPermissionCheck = httpContext => true;

            });

        }

        public static void AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });
        }

    }
}