using System.Text;
using AspNetCoreRateLimit;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.Controllers;
using Contracts;
using Entitites.Models;
using LoggerService;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Service;
using Service.Contracts;
using Service.DataShaping;

namespace CompanyEmployees.Extensions
{
    public static class ServiceExtensions
    {
        // CORS configuration
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination");
                });
            });
        }

        // IIS configuration
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        // NLog configuration
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        // Repository manager configuration
        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        // Service manager configuration
        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
        }

        // Context configuration
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("sqlConnection"));
            });
        }

        // Custom csv formatter 
        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) => builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

        // Action filter configuration 
        public static void ConfigureActionFilters(this IServiceCollection services) =>
            services.AddScoped<ValidationFilterAttribute>();

        // Datashaper configuration
        public static void ConfigureDataShaper(this IServiceCollection services)
        {
            services.AddScoped(typeof(IDataShaper<>), typeof(DataShaper<>));
        }

        // Versioning configuration
        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
                opt.Conventions.Controller<CompaniesController>()
                        .HasApiVersion(new ApiVersion(1, 0));
                opt.Conventions.Controller<CompaniesV2Controller>()
                        .HasDeprecatedApiVersion(new ApiVersion(2, 0));
            });
        }

        // Caching configuration
        public static void ConfigureResponseCaching(this IServiceCollection services) =>
            services.AddResponseCaching();

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) => services.AddHttpCacheHeaders(
        (expirationOpt) =>
        {
            expirationOpt.MaxAge = 100;
            expirationOpt.CacheLocation = CacheLocation.Private;

        },
        (validationOpt) =>
        {
            validationOpt.MustRevalidate = true;
        });

        // Rate limit configuration
        public static void ConfigureRateLimitinOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>
            {
                new()
                {
                    Endpoint = "*",
                    Limit = 30,
                    Period = "5m",

                }
            };
            services.Configure<IpRateLimitOptions>(opt => { opt.GeneralRules = rateLimitRules; });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

        // Identity configuration
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
        }

        // JWT configuration
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = Environment.GetEnvironmentVariable("SECRET");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }
    }
}
