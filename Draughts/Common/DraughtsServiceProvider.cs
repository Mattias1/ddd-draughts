using DalSoft.Hosting.BackgroundQueue.DependencyInjection;
using Draughts.Application.Auth;
using Draughts.Application.Auth.Services;
using Draughts.Application.Lobby.Services;
using Draughts.Application.ModPanel.Services;
using Draughts.Application.PlayGame.Services;
using Draughts.Application.Shared.Middleware;
using Draughts.Domain.AuthContext.Services;
using Draughts.Domain.GameContext.Services;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Draughts.Repositories.InMemory;
using Draughts.Repositories.Transaction;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Serilog;
using System;

namespace Draughts.Common;

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
        services.AddBackgroundQueue(e => Log.Logger.Error("Error in background task", e));

        services.AddScoped<JwtActionFilter>();
        services.AddScoped<AuthContextActionFilter>();
        services.AddScoped<ExceptionLoggerActionFilter>();
    }

    private static void ConfigureEventHandlers(IServiceCollection services) {
        services.AddSingleton<SynchronizePendingUserEventHandler>();
        services.AddSingleton<FinishUserRegistrationEventHandler>();
        services.AddSingleton<FinishedGameEventHandler>();
        services.AddSingleton<ModPanelRoleEventHandler>();
    }

    private static void ConfigureRepositories(IServiceCollection services, bool useInMemoryDatabase, int hiloLargeIntervalSize, int hiloSmallIntervalSize) {
        services.AddSingleton<IUnitOfWork, UnitOfWorkWrapper>();

        if (useInMemoryDatabase) {
            services.AddSingleton<IIdGenerator>(HiLoIdGenerator.InMemoryHiloGIdGenerator(
                hiloLargeIntervalSize, hiloSmallIntervalSize, hiloSmallIntervalSize));
            services.AddSingleton<IRepositoryUnitOfWork, InMemoryUnitOfWork>();

            services.AddSingleton<IAuthUserRepository, InMemoryAuthUserRepository>();
            services.AddSingleton<IRoleRepository, InMemoryRoleRepository>();
            services.AddSingleton<IAdminLogRepository, InMemoryAdminLogRepository>();
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            services.AddSingleton<IGameRepository, InMemoryGameRepository>();
            services.AddSingleton<IGameStateRepository, InMemoryGameStateRepository>();
            services.AddSingleton<IVotingRepository, InMemoryVotingRepository>();
        }
        else {
            services.AddSingleton<IIdGenerator>(HiLoIdGenerator.DbHiloGIdGenerator(
                hiloLargeIntervalSize, hiloSmallIntervalSize, hiloSmallIntervalSize));
            services.AddSingleton<IRepositoryUnitOfWork, DbUnitOfWork>();

            services.AddSingleton<IAuthUserRepository, DbAuthUserRepository>();
            services.AddSingleton<IRoleRepository, DbRoleRepository>();
            services.AddSingleton<IAdminLogRepository, DbAdminLogRepository>();
            services.AddSingleton<IUserRepository, DbUserRepository>();
            services.AddSingleton<IGameRepository, DbGameRepository>();
            services.AddSingleton<IGameStateRepository, DbGameStateRepository>();
            services.AddSingleton<IVotingRepository, DbVotingRepository>();
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
        services.AddSingleton<UserRegistrationDomainService>();
        services.AddSingleton<UserRoleDomainService>();
        services.AddSingleton<PlayGameDomainService>();
        services.AddSingleton<GameFactory>();
    }

    public static void RegisterEventHandlers(IServiceProvider serviceProvider) {
        var unitOfWork = serviceProvider.GetRequiredService<IRepositoryUnitOfWork>();

        unitOfWork.Register(serviceProvider.GetRequiredService<SynchronizePendingUserEventHandler>());
        unitOfWork.Register(serviceProvider.GetRequiredService<FinishUserRegistrationEventHandler>());
        unitOfWork.Register(serviceProvider.GetRequiredService<FinishedGameEventHandler>());
        unitOfWork.Register(serviceProvider.GetRequiredService<ModPanelRoleEventHandler>());
    }
}
