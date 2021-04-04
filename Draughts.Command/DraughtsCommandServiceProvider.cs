using Draughts.Command.Seeders;
using Draughts.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Draughts.Command {
    public static class DraughtsCommandServiceProvider {
        public static void ConfigureServices(IServiceCollection services) {
            DraughtsServiceProvider.ConfigureServices(services,
                useInMemoryDatabase: false, hiloLargeIntervalSize: 1, hiloSmallIntervalSize: 1);

            services.AddTransient<DraughtsConsole>();
            services.AddTransient<EssentialDataSeeder>();
            services.AddTransient<DummyDataSeeder>();
        }
    }
}
