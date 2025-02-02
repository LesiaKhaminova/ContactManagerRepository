﻿using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstApplicationTests
{
    public class CustomWebApplicationFactory: WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
               var descriptor = services.SingleOrDefault
                (temp => temp.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if(descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddDbContext<ApplicationDbContext>(OptionsBuilderConfigurationExtensions =>
                {
                    OptionsBuilderConfigurationExtensions.UseInMemoryDatabase("DatabaseForTesting");
                });

            });
        }

    }
}
