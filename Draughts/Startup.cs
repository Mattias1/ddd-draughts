using Draughts.Application.Hubs;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Middleware;
using Draughts.Common;
using Draughts.Repositories;
using Draughts.Repositories.Misc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlQueryBuilder.Options;
using System;
using System.Threading.Tasks;

namespace Draughts;

public sealed class Startup {
    public static string VIEW_LOCATION_TEMPLATE = "/Application/{1}/Views/{0}" + RazorViewEngine.ViewExtension;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
        services.AddControllersWithViews(options => {
            options.Filters.Add(typeof(JwtActionFilter));
            options.Filters.Add(typeof(ExceptionLoggerActionFilter));
        });

        DraughtsServiceProvider.ConfigureServices(services);

        ConfigureRazorViewLocations(services);
        services.AddSignalR();
    }

    private void ConfigureRazorViewLocations(IServiceCollection services) {
        services.Configure<RazorViewEngineOptions>(o => {
            // {2} is area, {1} is controller, {0} is the action
            o.ViewLocationFormats.Clear();
            o.ViewLocationFormats.Add(VIEW_LOCATION_TEMPLATE);
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
        } else {
            app.UseExceptionHandler(exApp => exApp.Run(async ctx => {
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await ctx.Response.WriteAsync("Whoops, an unexpected error occured (500).");
            }));
        }

        DraughtsServiceProvider.RegisterEventHandlers(app.ApplicationServices);

        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseMiddleware<HeartBeatMiddleware>();
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
        if (settings?.DbPassword is null || settings.DbServer is null || settings.DbPort is null
                || settings.BaseUrl is null) {
            throw new InvalidOperationException("Invalid appsettings.json file");
        }

        DbContext.Init(settings.DbServer, settings.DbPort.Value, settings.DbPassword);
        QueryBuilderOptions.SetupDapperWithSnakeCaseAndNodaTime();
        Utils.BaseUrl = settings.BaseUrl;
    }
}
