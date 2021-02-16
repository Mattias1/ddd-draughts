using Draughts.Repositories;
using Draughts.Repositories.Database;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using System;
using Draughts.Repositories.Transaction;
using Draughts.Application.Shared.Middleware;
using Draughts.Application.Auth.Services;
using Draughts.Application.Lobby.Services;
using Draughts.Application.Shared.Services;
using Draughts.Application.Auth;
using Draughts.Application.PlayGame.Services;
using Draughts.Repositories.InMemory;

namespace Draughts.Common {
    public static class DraughtsServiceProvider {
        private static int HI_LO_INTERVAL_SIZE = 100;
        public static void ConfigureServices(IServiceCollection services, bool useInMemoryDatabase) {
            services.AddSingleton<IClock>(SystemClock.Instance);

            if (useInMemoryDatabase) {
                services.AddSingleton<IIdGenerator>(new InMemoryHiLoGenerator(HI_LO_INTERVAL_SIZE));
                services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

                services.AddSingleton<IAuthUserRepository, InMemoryAuthUserRepository>();
                services.AddSingleton<IRoleRepository, InMemoryRoleRepository>();
                services.AddSingleton<IUserRepository, InMemoryUserRepository>();
                services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
                services.AddSingleton<IGameRepository, InMemoryGameRepository>();
            }
            else {
                services.AddSingleton<IIdGenerator>(new DbHiLoGenerator(HI_LO_INTERVAL_SIZE));
                services.AddSingleton<IUnitOfWork, DbUnitOfWork>();

                services.AddSingleton<IAuthUserRepository, DbAuthUserRepository>();
                services.AddSingleton<IRoleRepository, DbRoleRepository>();
                services.AddSingleton<IUserRepository, DbUserRepository>();
                services.AddSingleton<IPlayerRepository, DbPlayerRepository>();
                services.AddSingleton<IGameRepository, DbGameRepository>();
            }

            services.AddSingleton<SynchronizePendingUserEventHandler>();
            services.AddSingleton<FinishUserRegistrationEventHandler>();

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IGameService, GameService>();
            services.AddSingleton<IPlayGameService, PlayGameService>();

            services.AddSingleton<IAuthUserFactory, AuthUserFactory>();
            services.AddSingleton<IEventFactory, EventFactory>();
            services.AddSingleton<IUserFactory, UserFactory>();
            services.AddSingleton<IGameFactory, GameFactory>();

            services.AddScoped<JwtActionFilter>();
            services.AddScoped<AuthContextActionFilter>();
        }

        public static void RegisterEventHandlers(IServiceProvider serviceProvider) {
            var unitOfWork = serviceProvider.GetService<IUnitOfWork>()!;

            unitOfWork.Register(serviceProvider.GetService<SynchronizePendingUserEventHandler>()!);
            unitOfWork.Register(serviceProvider.GetService<FinishUserRegistrationEventHandler>()!);
        }
    }
}
