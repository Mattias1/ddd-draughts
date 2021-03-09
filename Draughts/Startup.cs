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

namespace Draughts {
    public class Startup {
        public IConfiguration Configuration { get; }

        protected virtual bool UseInMemoryDatabase => false;

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews(options => {
                options.Filters.Add(typeof(JwtActionFilter));
                options.Filters.Add(typeof(ExceptionLoggerActionFilter));
            });

            DraughtsServiceProvider.ConfigureServices(services, UseInMemoryDatabase);

            ConfigureRazorViewLocations(services);
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
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

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=StaticPages}/{action=Home}/{id?}");
            });
        }
    }

    public class DbStartup : Startup {
        protected override bool UseInMemoryDatabase => false;

        public DbStartup(IConfiguration configuration) : base(configuration) { }
    }

    public class InMemoryStartup : Startup {
        protected override bool UseInMemoryDatabase => true;

        public InMemoryStartup(IConfiguration configuration) : base(configuration) { }
    }
}
