using Draughts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace Draughts.Common {
    public static class DraughtsServiceProvider {
        public static void ConfigureServices(IServiceCollection services) {
            services.AddSingleton<IClock>(SystemClock.Instance);

            services.AddSingleton<IAuthUserRepository, AuthUserRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
        }
    }
}
