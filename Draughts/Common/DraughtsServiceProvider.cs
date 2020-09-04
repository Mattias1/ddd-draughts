using Draughts.Services;
using Draughts.Controllers.Middleware;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Draughts.EventHandlers;
using System;
using Draughts.Repositories.Databases;

namespace Draughts.Common {
    public static class DraughtsServiceProvider {
        public const int ID_GENERATION_INTERVAL = 10;

        public static void ConfigureServices(IServiceCollection services) {
            services.AddSingleton<IClock>(SystemClock.Instance);
            services.AddSingleton<IIdGenerator>(new LoHiGenerator(ID_GENERATION_INTERVAL, () => MiscDatabase.IdGenerationTable));
            services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

            services.AddSingleton<SynchronizePendingUserEventHandler>();
            services.AddSingleton<FinishUserRegistrationEventHandler>();

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IGameService, GameService>();

            services.AddSingleton<IAuthUserRepository, InMemoryAuthUserRepository>();
            services.AddSingleton<IRoleRepository, InMemoryRoleRepository>();
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
            services.AddSingleton<IGameRepository, InMemoryGameRepository>();

            services.AddSingleton<IAuthUserFactory, AuthUserFactory>();
            services.AddSingleton<IEventFactory, EventFactory>();
            services.AddSingleton<IUserFactory, UserFactory>();
            services.AddSingleton<IGameFactory, GameFactory>();

            services.AddScoped<JwtActionFilter>();
            services.AddScoped<AuthContextActionFilter>();
        }

        public static void RegisterEventHandlers(IServiceProvider serviceProvider) {
            var unitOfWork = serviceProvider.GetService<IUnitOfWork>();

            unitOfWork.Register(serviceProvider.GetService<SynchronizePendingUserEventHandler>());
            unitOfWork.Register(serviceProvider.GetService<FinishUserRegistrationEventHandler>());
        }
    }
}
