﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using IdentityServer4.Quickstart.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using sts_tutorial.Data;
using sts_tutorials.Models;
using Microsoft.AspNetCore.Identity;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Linq;
using IdentityServer4.AspNetIdentity;
using sts_tutorials.Quickstart.Account;
using IdentityServer4.Services;

namespace sts_tutorials
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            Configuration = config;
            Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = true;
                options.AuthenticationDisplayName = "Windows";
            });



            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            var identityServer = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                // .AddTestUsers(TestUsers.Users)
                .AddAspNetIdentity<ApplicationUser>()
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                })
                .AddProfileService<ProfileService>()
                ;

            //services.AddAuthentication(IdentityConstants.ExternalScheme);

            //services.AddAuthentication()
            //    .AddIdentityServerAuthentication("api", options =>
            //    {
            //        options.Authority = "http://localhost:44317";
            //        options.RequireHttpsMetadata = false;
            //        options.ApiName = "api1";
            //    });


            //services.AddAuthentication()
            //    .AddGoogle(options =>
            //    {
            //        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

            //        // register your IdentityServer with Google at https://console.developers.google.com
            //        // enable the Google+ API
            //        // set the redirect URI to http://localhost:5000/signin-google
            //        options.ClientId = "copy client ID from Google here";
            //        options.ClientSecret = "copy client secret from Google here";
            //    });

            //var cors = new DefaultCorsPolicyService(_loggerFactory.CreateLogger<DefaultCorsPolicyService>())
            //{
            //    AllowedOrigins = { "http://foo", "https://bar" }
            //};
            //services.AddSingleton<ICorsPolicyService>(cors);
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                        .WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                    );
            });


            //services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            //{
            //    builder.AllowAnyOrigin()
            //           .AllowAnyMethod()
            //           .AllowAnyHeader();
            //}));

            if (Environment.IsDevelopment())
            {
                identityServer.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            // Shows UseCors with CorsPolicyBuilder.
            //app.UseCors(builder =>
            //   builder.WithOrigins("http://localhost:4200"));

            //app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            //app.UseCors("MyPolicy");

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            ////Registered before static files to always set header
            //app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains());
            //app.UseXContentTypeOptions();
            //app.UseReferrerPolicy(opts => opts.NoReferrer());
            //app.UseXXssProtection(options => options.EnabledWithBlockMode());
            //app.UseXfo(options => options.Deny());

            //app.UseCsp(opts => opts
            //    .BlockAllMixedContent()
            //    .StyleSources(s => s.Self())
            //    .StyleSources(s => s.UnsafeInline())
            //    .FontSources(s => s.Self())
            //    .FormActions(s => s.Self())
            //    .FrameAncestors(s => s.Self())
            //    .ImageSources(s => s.Self())
            //    .ScriptSources(s => s.Self())
            //);


            //app.Use(async (ctx, next) =>
            //{
            //    var schemes = ctx.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            //    var handlers = ctx.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            //    foreach (var scheme in await schemes.GetRequestHandlerSchemesAsync())
            //    {
            //        var handler = await handlers.GetHandlerAsync(ctx, scheme.Name) as IAuthenticationRequestHandler;
            //        if (handler != null && await handler.HandleRequestAsync())
            //        {
            //            // start same-site cookie special handling
            //            string location = null;
            //            if (ctx.Response.StatusCode == 302)
            //            {
            //                location = ctx.Response.Headers["location"];
            //            }
            //            else if (ctx.Request.Method == "GET" && !ctx.Request.Query["skip"].Any())
            //            {
            //                location = ctx.Request.Path + ctx.Request.QueryString + "&skip=1";
            //            }

            //            if (location != null)
            //            {
            //                ctx.Response.StatusCode = 200;
            //                var html = $@"
            //            <html><head>
            //                <meta http-equiv='refresh' content='0;url={location}' />
            //            </head></html>";
            //                await ctx.Response.WriteAsync(html);
            //            }
            //            // end same-site cookie special handling

            //            return;
            //        }
            //    }

            //    await next();
            //});

            app.UseCors("default");
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
