using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Draughts.Test.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Command.Seeders;

public sealed class DummyDataSeeder {
    private readonly AuthUserRepository _authUserRepository;
    private readonly GameRepository _gameRepository;
    private readonly GameStateRepository _gameStateRepository;
    private readonly RoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserRepository _userRepository;

    public DummyDataSeeder(AuthUserRepository authUserRepository, GameRepository gameRepository,
        GameStateRepository gameStateRepository, RoleRepository roleRepository,
        IUnitOfWork unitOfWork, UserRepository userRepository
    ) {
        _authUserRepository = authUserRepository;
        _gameRepository = gameRepository;
        _gameStateRepository = gameStateRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public void SeedData() {
        EnsureDatabasesContainOnlyEssentialData();

        var users = SeedUserDomain();
        SeedAuthDomain(users);
        SeedGameDomain(users);
    }

    private void EnsureDatabasesContainOnlyEssentialData() {
        _unitOfWork.WithUserTransaction(tran => {
            var users = _userRepository.List();
            if (users.Count != 1 && users[0].Username != "admin") {
                throw new InvalidOperationException("User table should be empty except for the admin user.");
            }
        });

        _unitOfWork.WithAuthTransaction(tran => {
            var authUsers = _authUserRepository.List();
            if (authUsers.Count != 1 && authUsers[0].Username != "admin") {
                throw new InvalidOperationException("Auth user table should be empty except for the admin user.");
            }
            if (_roleRepository.Count() > 3) {
                throw new InvalidOperationException("Role table contains more roles than expected.");
            }
        });

        _unitOfWork.WithGameTransaction(tran => {
            if (_gameRepository.Count() > 0) {
                throw new InvalidOperationException("Game table is not empty.");
            }
            if (_gameStateRepository.Count() > 0) {
                throw new InvalidOperationException("Game state table is not empty.");
            }
        });
    }

    private IReadOnlyList<User> SeedUserDomain() {
        var users = new List<User>();
        _unitOfWork.WithUserTransaction(tran => {
            users.Add(UserTestHelper.User("User")
                .WithRating(1700)
                .WithRank(Rank.Ranks.WarrantOfficer)
                .Build());
            users.Add(UserTestHelper.User("TheColonel")
                .WithRating(3456)
                .WithRank(Rank.Ranks.Colonel)
                .Build());
            users.Add(UserTestHelper.User(Username.MATTY)
                .WithRating(2345)
                .WithRank(Rank.Ranks.Lieutenant)
                .Build());
            users.Add(UserTestHelper.User("Mathy")
                .WithRating(800)
                .WithRank(Rank.Ranks.LanceCorporal)
                .Build());
            users.Add(UserTestHelper.User("JackDeHaas")
                .WithRating(9001)
                .WithRank(Rank.Ranks.FieldMarshal)
                .Build());
            users.Add(UserTestHelper.User("PendingPlayer").Build());
            users.Add(UserTestHelper.User("TestPlayerBlack").Build());
            users.Add(UserTestHelper.User("TestPlayerWhite").Build());

            foreach (var u in users) {
                _userRepository.Save(u);
            }
        });

        return users.AsReadOnly();
    }

    private void SeedAuthDomain(IReadOnlyList<User> users) {
        _unitOfWork.WithAuthTransaction(tran => {
            var roles = _roleRepository.List();
            var pendingRegistrationRole = roles.Single(r => r.Rolename == Role.PENDING_REGISTRATION_ROLENAME);
            var registeredUserRole = roles.Single(r => r.Rolename == Role.REGISTERED_USER_ROLENAME);
            var adminRole = roles.Single(r => r.Rolename == Role.ADMIN_ROLENAME);

            foreach (var u in users) {
                var role = u.Username == "PendingPlayer" ? pendingRegistrationRole : registeredUserRole;
                var authUserBuilder = AuthUserTestHelper.FromUserAndRoles(u, role).WithPasswordHash("admin");
                if (u.Username == Username.MATTY) {
                    authUserBuilder.AddRole(adminRole);
                }

                _authUserRepository.Save(authUserBuilder.Build());
            }
        });
    }

    private void SeedGameDomain(IReadOnlyList<User> users) {
        var user = users.Single(u => u.Username == "User");
        var mathy = users.Single(u => u.Username == "Mathy");

        _unitOfWork.WithGameTransaction(tran => {
            var player1 = PlayerTestHelper.FromUser(user).WithColor(Color.White).Build();
            var game1 = GameTestHelper.PendingInternationalGame(player1).Build();
            SaveWithInitialGameState(game1);

            var player2 = PlayerTestHelper.FromUser(mathy).Build();
            var game2 = GameTestHelper.PendingInternationalGame(player2).Build();
            SaveWithInitialGameState(game2);

            var player3 = PlayerTestHelper.FromUser(user).Build();
            var game3 = GameTestHelper.PendingGame(GameSettings.EnglishAmerican, player3).Build();
            SaveWithInitialGameState(game3);

            var player4 = PlayerTestHelper.FromUser(mathy).Build();
            var game4 = GameTestHelper.PendingGame(GameSettings.Mini, player4).Build();
            SaveWithInitialGameState(game4);

            var player5 = PlayerTestHelper.FromUser(mathy).Build();
            var game5 = GameTestHelper.PendingGame(GameSettings.MiniHex, player5).Build();
            SaveWithInitialGameState(game5);
        });
    }

    private void SaveWithInitialGameState(Game game) {
        var gameState = GameState.InitialState(game.Id, game.Settings.BoardType);
        _gameRepository.Save(game);
        _gameStateRepository.Save(gameState);
    }
}
