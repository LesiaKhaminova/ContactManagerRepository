using ContactsManager.Core.Domain.IdentityEntities;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFirstApplication.Filters.ActionFilters;
using RepositiruContracts;
using Repositories;
using ServiceContracts;
using Services;

namespace MyFirstApplication.SturtupExtensions
{
    public static class ConfigureServicesExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc().AddRazorRuntimeCompilation();
            services.AddTransient<ResponseHeaderActionFilter>();
            services.AddControllersWithViews(options =>
            {
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
                options.Filters.Add(new ResponseHeaderActionFilter(logger) { _key = "My-Key-From-Global", _value = "My-Value-From-Global", Order = 2 });
            });
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<PersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewExcelFields>();

            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();

            //Enable Identity in this project
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 3;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            services.AddAuthorization(options => {
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.AddPolicy("NotAuthorized", policy =>
                {
                    policy.RequireAssertion(context => {
                        return !context.User.Identity.IsAuthenticated;
                    });
                });
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LogoutPath = "/Account/Login";
            });

            services.AddHttpLogging(options =>
            {
                options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties |
                Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });
            return services;    
        }
    }
}
