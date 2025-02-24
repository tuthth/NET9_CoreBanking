using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Settings;
using API.Middlewares;

namespace API.AppConfiguration
{
    public static class AppConfiguration
    {
        public static void MailSettings(this IServiceCollection services, IConfiguration _configuration)
        {
            services.AddOptions();
            var mailsettings = _configuration.GetSection("MailSettings");
            services.Configure<MailSetting>(mailsettings);
        }
        public static void AddJWT(this IServiceCollection services, IConfiguration configuration)
            {
                services.AddOptions();
                var appsettings = configuration.GetSection("AppSettings");
                services.Configure<JWTSetting>(appsettings);


                var secretKey = configuration["AppSettings:SecretKey"];
                var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opt =>
                    {
                        opt.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                            ClockSkew = TimeSpan.Zero
                        };
                    });
            }
        public static void AddCustomRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {

                options.AddConcurrencyLimiter(policyName: "con-default", options =>
                {
                    options.PermitLimit = 100;
                    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 7;
                });
                options.AddFixedWindowLimiter(policyName: "fixed-default", options =>
                {
                    options.PermitLimit = 100;
                    options.Window = TimeSpan.FromSeconds(12);
                    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 7;
                });
                options.AddSlidingWindowLimiter(policyName: "slide-default", options =>
                {
                    options.PermitLimit = 100;
                    options.Window = TimeSpan.FromSeconds(12);
                    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 7;
                });
                options.AddTokenBucketLimiter(policyName: "JWTLimiter", options =>
                {
                    options.TokenLimit = 1;
                    options.TokensPerPeriod = 1;
                    options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
                    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, rateLimitContext) =>
                {
                    var limiterKey = context.HttpContext.Request.Path;
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    Log.Warning($"Rate limit exceeded. Rejected for {limiterKey} at {DateTime.UtcNow} ");
                    await context.HttpContext.Response.WriteAsync("Rate limit exceeded.");
                };
            });
        }
        public static void AddMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<ValidateIPAdressMiddleware>();
            app.UseMiddleware<ValidateKeyExpireMiddleware>();
            app.UseMiddleware<ProcessingMiddleware>();
        }
    }
}
