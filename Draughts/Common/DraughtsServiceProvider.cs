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
using Draughts.Domain.AuthContext.Services;
using Draughts.Domain.GameContext.Services;

namespace Draughts.Common {
    public static class DraughtsServiceProvider {
        private const int HI_LO_INTERVAL_SIZE_LARGE = 100;
        private const int HI_LO_INTERVAL_SIZE_SMALL = 10;

        public static void ConfigureServices(IServiceCollection services, bool useInMemoryDatabase,
                int hiloLargeIntervalSize = HI_LO_INTERVAL_SIZE_LARGE,
                int hiloSmallIntervalSize = HI_LO_INTERVAL_SIZE_SMALL) {
            ConfigureApplicationMiddleware(services);
            ConfigureEventHandlers(services);
            ConfigureRepositories(services, useInMemoryDatabase, hiloLargeIntervalSize, hiloSmallIntervalSize);

            ConfigureApplicationServices(services);

            ConfigureDomainServices(services);
        }

        private static void ConfigureApplicationMiddleware(IServiceCollection services) {
            services.AddSingleton<IClock>(SystemClock.Instance);

            services.AddScoped<JwtActionFilter>();
            services.AddScoped<AuthContextActionFilter>();
            services.AddScoped<ExceptionLoggerActionFilter>();
        }

        private static void ConfigureEventHandlers(IServiceCollection services) {
            services.AddSingleton<SynchronizePendingUserEventHandler>();
            services.AddSingleton<FinishUserRegistrationEventHandler>();
            services.AddSingleton<ModPanelRoleEventHandler>();
        }

        private static void ConfigureRepositories(IServiceCollection services, bool useInMemoryDatabase, int hiloLargeIntervalSize, int hiloSmallIntervalSize) {
            if (useInMemoryDatabase) {
                services.AddSingleton<IIdGenerator>(HiLoIdGenerator.InMemoryHiloGIdGenerator(
                    hiloLargeIntervalSize, hiloSmallIntervalSize, hiloSmallIntervalSize));
                services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

                services.AddSingleton<IAuthUserRepository, InMemoryAuthUserRepository>();
                services.AddSingleton<IRoleRepository, InMemoryRoleRepository>();
                services.AddSingleton<IAdminLogRepository, InMemoryAdminLogRepository>();
                services.AddSingleton<IUserRepository, InMemoryUserRepository>();
                services.AddSingleton<IGameRepository, InMemoryGameRepository>();
                services.AddSingleton<IGameStateRepository, InMemoryGameStateRepository>();
            }
            else {
                services.AddSingleton<IIdGenerator>(HiLoIdGenerator.DbHiloGIdGenerator(
                    hiloLargeIntervalSize, hiloSmallIntervalSize, hiloSmallIntervalSize));
                services.AddSingleton<IUnitOfWork, DbUnitOfWork>();

                services.AddSingleton<IAuthUserRepository, DbAuthUserRepository>();
                services.AddSingleton<IRoleRepository, DbRoleRepository>();
                services.AddSingleton<IAdminLogRepository, DbAdminLogRepository>();
                services.AddSingleton<IUserRepository, DbUserRepository>();
                services.AddSingleton<IGameRepository, DbGameRepository>();
                services.AddSingleton<IGameStateRepository, DbGameStateRepository>();
            }
        }

        private static void ConfigureApplicationServices(IServiceCollection services) {
            services.AddSingleton<AuthService>();
            services.AddSingleton<EditRoleService>();
            services.AddSingleton<RoleUsersService>();
            services.AddSingleton<GameService>();
            services.AddSingleton<PlayGameService>();
            services.AddSingleton<UserRegistrationService>();
        }

        private static void ConfigureDomainServices(IServiceCollection services) {
            services.AddSingleton<IUserRegistrationDomainService, UserRegistrationDomainService>();
            services.AddSingleton<IUserRoleDomainService, UserRoleDomainService>();
            services.AddSingleton<IPlayGameDomainService, PlayGameDomainService>();
            services.AddSingleton<IGameFactory, GameFactory>();
        }

        public static void RegisterEventHandlers(IServiceProvider serviceProvider) {
            var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

            unitOfWork.Register(serviceProvider.GetRequiredService<SynchronizePendingUserEventHandler>());
            unitOfWork.Register(serviceProvider.GetRequiredService<FinishUserRegistrationEventHandler>());
            unitOfWork.Register(serviceProvider.GetRequiredService<ModPanelRoleEventHandler>());
        }
    }
}
