using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using Draughts.Test.TestHelpers;
using SqlQueryBuilder.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Command.Seeders {
    class DummyDataSeeder {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public DummyDataSeeder(IAuthUserRepository authUserRepository, IGameRepository gameRepository,
            IPlayerRepository playerRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork,
            IUserRepository userRepository
        ) {
            _authUserRepository = authUserRepository;
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public void SeedData() {
            EnsureDatabasesContainOnlyEssentialData();

            SeedAvailableIds();

            var users = SeedUserDomain();
            SeedAuthUserDomain(users);
            SeedGameDomain(users);

            UpdateAvailableIds();
        }

        private void SeedAvailableIds() {
            using (var tranFlavor = DbContext.Get.MiscTransaction()) {
                var availableIds = DbContext.Get.Query(tranFlavor)
                    .SelectAllFrom("id_generation")
                    .List<DbIdGeneration>()
                    .ToDictionary(i => i.Subject, i => i.AvailableId);
                tranFlavor.Commit();

                if (availableIds.Count != 3) {
                    throw new InvalidOperationException("The id generation table should contain exactly three available ids.");
                }

                long miscId = availableIds[DbIdGeneration.SUBJECT_MISC];
                long gameId = availableIds[DbIdGeneration.SUBJECT_GAME];
                long userId = availableIds[DbIdGeneration.SUBJECT_USER];
                IdTestHelper.Seed(miscId, gameId, userId);

            }
        }

        private void EnsureDatabasesContainOnlyEssentialData() {
            _unitOfWork.WithUserTransaction(tran => {
                var users = _userRepository.List();
                if (users.Count != 1 && users[0].Username != "admin") {
                    throw new InvalidOperationException("User table should be empty except for the admin user.");
                }
                tran.Commit();
            });

            _unitOfWork.WithAuthUserTransaction(tran => {
                var authUsers = _authUserRepository.List();
                if (authUsers.Count != 1 && authUsers[0].Username != "admin") {
                    throw new InvalidOperationException("Auth user table should be empty except for the admin user.");
                }
                if (_roleRepository.Count() > 3) {
                    throw new InvalidOperationException("Role table contains more roles than expected.");
                }
                tran.Commit();
            });

            _unitOfWork.WithGameTransaction(tran => {
                if (_gameRepository.Count() > 0) {
                    throw new InvalidOperationException("Game table is not empty.");
                }
                if (_playerRepository.Count() > 0) {
                    throw new InvalidOperationException("Player table is not empty.");
                }
                tran.Commit();
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
                users.Add(UserTestHelper.User("Matty")
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
                users.Add(UserTestHelper.User("<script>alert('Hi, my name is Bobby');</script>").Build());
                users.Add(UserTestHelper.User("TestPlayerBlack").Build());
                users.Add(UserTestHelper.User("TestPlayerWhite").Build());

                foreach (var u in users) {
                    _userRepository.Save(u);
                }

                tran.Commit();
            });

            return users.AsReadOnly();
        }

        private void SeedAuthUserDomain(IReadOnlyList<User> users) {
            _unitOfWork.WithAuthUserTransaction(tran => {
                var roles = _roleRepository.List();
                var pendingRegistrationRole = roles.Single(r => r.Rolename == Role.PENDING_REGISTRATION_ROLENAME);
                var registeredUserRole = roles.Single(r => r.Rolename == Role.REGISTERED_USER_ROLENAME);
                var adminRole = roles.Single(r => r.Rolename == Role.ADMIN_ROLENAME);

                foreach (var u in users) {
                    var role = u.Username.Value.Contains("Bobby") ? pendingRegistrationRole : registeredUserRole;
                    var authUserBuilder = AuthUserTestHelper.FromUserAndRoles(u, role).WithPasswordHash("admin");
                    if (u.Username == "Matty") {
                        authUserBuilder.AddRole(adminRole);
                    }

                    _authUserRepository.Save(authUserBuilder.Build());
                }

                tran.Commit();
            });
        }

        private void SeedGameDomain(IReadOnlyList<User> users) {
            var user = users.Single(u => u.Username == "User");
            var mathy = users.Single(u => u.Username == "Mathy");

            _unitOfWork.WithGameTransaction(tran => {
                var player1 = PlayerTestHelper.FromUser(user).WithColor(Color.White).Build();
                var game1 = GameTestHelper.PendingInternationalGame(player1).Build();
                _gameRepository.Save(game1);
                _playerRepository.Save(player1, game1.Id);

                var player2 = PlayerTestHelper.FromUser(mathy).Build();
                var game2 = GameTestHelper.PendingInternationalGame(player2).Build();
                _gameRepository.Save(game2);
                _playerRepository.Save(player2, game2.Id);

                var player3 = PlayerTestHelper.FromUser(user).Build();
                var game3 = GameTestHelper.PendingGame(GameSettings.EnglishAmerican, player3).Build();
                _gameRepository.Save(game3);
                _playerRepository.Save(player3, game3.Id);

                var player4 = PlayerTestHelper.FromUser(mathy).Build();
                var game4 = GameTestHelper.PendingGame(GameSettings.Mini, player4).Build();
                _gameRepository.Save(game4);
                _playerRepository.Save(player4, game4.Id);

                tran.Commit();
            });
        }

        private void UpdateAvailableIds() {
            using (var tranFlavor = DbContext.Get.MiscTransaction()) {
                UpdateAvailableId(tranFlavor, DbIdGeneration.SUBJECT_MISC, IdTestHelper.Next());
                UpdateAvailableId(tranFlavor, DbIdGeneration.SUBJECT_GAME, IdTestHelper.NextForGame());
                UpdateAvailableId(tranFlavor, DbIdGeneration.SUBJECT_USER, IdTestHelper.NextForUser());

                tranFlavor.Commit();
            }
        }

        private void UpdateAvailableId(ISqlTransactionFlavor tranFlavor, string subject, long id) {
            var row = new DbIdGeneration(subject, id);
            DbContext.Get.Query(tranFlavor).Update("id_generation").SetFrom(row).Where("subject").Is(subject).Execute();
        }
    }
}
