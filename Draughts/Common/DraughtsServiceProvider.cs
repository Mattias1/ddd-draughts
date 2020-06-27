using Draughts.Services;
using Draughts.Controllers.Middleware;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Draughts.Common.Events;
using Draughts.EventHandlers;
using System;

namespace Draughts.Common {
    public static class DraughtsServiceProvider {
        public const int ID_GENERATION_INTERVAL = 10;

        public static void ConfigureServices(IServiceCollection services) {
            services.AddSingleton<IClock>(SystemClock.Instance);
            services.AddSingleton<IIdGenerator>(new LoHiGenerator(ID_GENERATION_INTERVAL, () => MiscDatabase.IdGenerationTable));

            services.AddSingleton<SynchronizePendingUserEventHandler>();
            services.AddSingleton<RegisterUserEventHandler>();
            services.AddSingleton<IEventQueue, EventQueue>();

            services.AddSingleton<IAuthService, AuthService>();

            services.AddSingleton<IAuthUserRepository, AuthUserRepository>();
            services.AddSingleton<IRoleRepository, RoleRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddSingleton<IAuthUserFactory, AuthUserFactory>();
            services.AddSingleton<IUserFactory, UserFactory>();

            services.AddScoped<JwtActionFilter>();
            services.AddScoped<AuthContextActionFilter>();
        }

        public static void RegisterEventHandlers(IServiceProvider serviceProvider) {
            var eventQueue = serviceProvider.GetService<IEventQueue>();

            eventQueue.Register(serviceProvider.GetService<SynchronizePendingUserEventHandler>());
            eventQueue.Register(serviceProvider.GetService<RegisterUserEventHandler>());
        }
    }
}
