﻿using System.Collections.Generic;
using BookFast.Api.Infrastructure;
using BookFast.Contracts.Framework;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;

namespace BookFast.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; set; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var modules = new List<ICompositionModule>
                          {
                              new Composition.CompositionModule(),
                              new Business.Composition.CompositionModule(),
                              new Data.Composition.CompositionModule()
                          };

#if DNX451
            modules.Add(new Search.Composition.CompositionModule());
#endif

            foreach (var module in modules)
            {
                module.AddServices(services, Configuration);
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<AuthenticationOptions> authOptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();

            app.UseJwtBearerAuthentication(options =>
                                           {
                                               options.AutomaticAuthenticate = true;
                                               options.AutomaticChallenge = true;
                                               options.Authority = authOptions.Value.Authority;
                                               options.Audience = authOptions.Value.Audience;
                                           });

            app.UseMvc();

            app.UseSwaggerGen();
        }
        
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
