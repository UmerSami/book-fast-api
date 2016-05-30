﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BookFast.Api.Infrastructure;
using BookFast.Contracts.Framework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BookFast.Api.Infrastructure.Authentication;
using System.Security.Claims;

namespace BookFast.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            var modules = new List<ICompositionModule>
                          {
                              new Composition.CompositionModule(),
                              new Business.Composition.CompositionModule(),
                              new Data.Composition.CompositionModule(),
                              new Search.Composition.CompositionModule()
                          };

            foreach (var module in modules)
            {
                module.AddServices(services, Configuration);
            }
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<Infrastructure.Authentication.AuthenticationOptions> authOptions, IOptions<B2CAuthenticationOptions> b2cAuthOptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();        

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AuthenticationScheme = Constants.CustomerAuthenticationScheme,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,

                MetadataAddress = $"{b2cAuthOptions.Value.Authority}/.well-known/openid-configuration?p={b2cAuthOptions.Value.SignInPolicy}",
                Audience = b2cAuthOptions.Value.Audience,

                Events = new JwtBearerEvents
                {
                    OnTokenValidated = ctx =>
                    {
                        var nameClaim = ctx.Ticket.Principal.FindFirst("name");
                        if (nameClaim != null)
                        {
                            var claimsIdentity = (ClaimsIdentity)ctx.Ticket.Principal.Identity;
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, nameClaim.Value)); 
                        }
                        return Task.FromResult(0);
                    },
                    OnAuthenticationFailed = ctx =>
                    {
                        ctx.SkipToNextMiddleware();
                        return Task.FromResult(0);
                    }
                }
            });

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AuthenticationScheme = Constants.OrganizationalAuthenticationScheme,
                AutomaticAuthenticate = true,
                AutomaticChallenge = false,

                Authority = authOptions.Value.Authority,
                Audience = authOptions.Value.Audience,

                Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        ctx.SkipToNextMiddleware();
                        return Task.FromResult(0);
                    }
                }
            });

            app.UseSecurityContext();
            app.UseMvc();

            app.UseSwaggerGen();
        }
    }
}
