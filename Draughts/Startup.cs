using Draughts.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Draughts {
    public class Startup {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllersWithViews();
            DraughtsServiceProvider.ConfigureServices(services);

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
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/StaticPages/Error");
            }
            app.UseStaticFiles();

            DraughtsServiceProvider.RegisterEventHandlers(app.ApplicationServices);

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=StaticPages}/{action=Home}/{id?}");
            });
        }
    }
}
