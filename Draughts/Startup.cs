using Draughts.Common;
using Draughts.Application.Shared.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Draughts.Repositories.Database;
using Draughts.Application.Shared;
using System;
using SignalRWebPack.Hubs;

namespace Draughts {
    public class Startup {
        protected virtual bool UseInMemoryDatabase => false;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews(options => {
                options.Filters.Add(typeof(JwtActionFilter));
                options.Filters.Add(typeof(ExceptionLoggerActionFilter));
            });

            DraughtsServiceProvider.ConfigureServices(services, UseInMemoryDatabase);

            ConfigureRazorViewLocations(services);
            services.AddSignalR();
        }

        private void ConfigureRazorViewLocations(IServiceCollection services) {
            services.Configure<RazorViewEngineOptions>(o => {
                // {2} is area, {1} is controller, {0} is the action
                o.ViewLocationFormats.Clear();
                o.ViewLocationFormats.Add("/Application/{1}/Views/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("/Application/Shared/Views/{0}" + RazorViewEngine.ViewExtension);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration) {
            ConfigureAppSettings(configuration);

            // Use a custom error page
            app.UseStatusCodePagesWithReExecute("/error", "?status={0}");

            // Catch exceptions and show the stacktrace / custom error page
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler(exApp => exApp.Run(ctx => {
                    ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return Task.CompletedTask;
                }));
            }

            DraughtsServiceProvider.RegisterEventHandlers(app.ApplicationServices);

            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=StaticPages}/{action=Home}/{id?}");
                endpoints.MapHub<WebsocketHub>("/hub");
            });
        }

        private static void ConfigureAppSettings(IConfiguration configuration) {
            var settings = configuration.Get<AppSettings?>();
            if (settings?.DbPassword is null || settings.BaseUrl is null) {
                throw new InvalidOperationException("Invalid appsettings.json file");
            }

            DbContext.Init(settings.DbPassword);
            Utils.BaseUrl = settings.BaseUrl;
        }
    }

    public class DbStartup : Startup {
        protected override bool UseInMemoryDatabase => false;
    }

    public class InMemoryStartup : Startup {
        protected override bool UseInMemoryDatabase => true;
    }
}
