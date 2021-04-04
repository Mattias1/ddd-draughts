using Draughts.Application.Auth;
using Draughts.Application.Auth.Services;
using Draughts.Application.Lobby.Services;
using Draughts.Application.PlayGame.Services;
using Draughts.Application.Shared.Middleware;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using Draughts.Repositories.InMemory;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using System;
using Draughts.Application.ModPanel.Services;

namespace Draughts.Common {
    public static class DraughtsServiceProvider {
        private const int HI_LO_INTERVAL_SIZE_LARGE = 100;
        private const int HI_LO_INTERVAL_SIZE_SMALL = 10;

        public static void ConfigureServices(IServiceCollection services, bool useInMemoryDatabase,
                int hiloLargeIntervalSize = HI_LO_INTERVAL_SIZE_LARGE,
                int hiloSmallIntervalSize = HI_LO_INTERVAL_SIZE_SMALL) {
            services.AddSingleton<IClock>(SystemClock.Instance);

            if (useInMemoryDatabase) {
                services.AddSingleton<IIdGenerator>(HiLoIdGenerator.InMemoryHiloGIdGenerator(
                    hiloLargeIntervalSize, hiloSmallIntervalSize, hiloSmallIntervalSize));
                services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

                services.AddSingleton<IAuthUserRepository, InMemoryAuthUserRepository>();
                services.AddSingleton<IRoleRepository, InMemoryRoleRepository>();
                services.AddSingleton<IAdminLogRepository, InMemoryAdminLogRepository>();
                services.AddSingleton<IUserRepository, InMemoryUserRepository>();
                services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
                services.AddSingleton<IGameRepository, InMemoryGameRepository>();
            }
            else {
                services.AddSingleton<IIdGenerator>(HiLoIdGenerator.DbHiloGIdGenerator(
                    hiloLargeIntervalSize, hiloSmallIntervalSize, hiloSmallIntervalSize));
                services.AddSingleton<IUnitOfWork, DbUnitOfWork>();

                services.AddSingleton<IAuthUserRepository, DbAuthUserRepository>();
                services.AddSingleton<IRoleRepository, DbRoleRepository>();
                services.AddSingleton<IAdminLogRepository, DbAdminLogRepository>();
                services.AddSingleton<IUserRepository, DbUserRepository>();
                services.AddSingleton<IPlayerRepository, DbPlayerRepository>();
                services.AddSingleton<IGameRepository, DbGameRepository>();
            }

            services.AddSingleton<SynchronizePendingUserEventHandler>();
            services.AddSingleton<FinishUserRegistrationEventHandler>();
            services.AddSingleton<ModPanelRoleEventHandler>();

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IEditRoleService, EditRoleService>();
            services.AddSingleton<IRoleUsersService, RoleUsersService>();
            services.AddSingleton<IGameService, GameService>();
            services.AddSingleton<IPlayGameService, PlayGameService>();

            services.AddSingleton<IAuthUserFactory, AuthUserFactory>();
            services.AddSingleton<IUserFactory, UserFactory>();
            services.AddSingleton<IGameFactory, GameFactory>();

            services.AddScoped<JwtActionFilter>();
            services.AddScoped<AuthContextActionFilter>();
            services.AddScoped<ExceptionLoggerActionFilter>();
        }

        public static void RegisterEventHandlers(IServiceProvider serviceProvider) {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

            unitOfWork.Register(serviceProvider.GetRequiredService<SynchronizePendingUserEventHandler>());
            unitOfWork.Register(serviceProvider.GetRequiredService<FinishUserRegistrationEventHandler>());
            unitOfWork.Register(serviceProvider.GetRequiredService<ModPanelRoleEventHandler>());
        }
    }
}
