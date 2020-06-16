using Draughts.Controllers.Services;
using Draughts.Middleware;
using Draughts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace Draughts.Common {
    public static class DraughtsServiceProvider {
        public static void ConfigureServices(IServiceCollection services) {
            services.AddSingleton<IClock>(SystemClock.Instance);

            services.AddSingleton<IAuthService, AuthService>();

            services.AddSingleton<IAuthUserRepository, AuthUserRepository>();
            services.AddSingleton<IRoleRepository, RoleRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddScoped<JwtActionFilter>();
            services.AddScoped<AuthContextActionFilter>();
        }
    }
}
