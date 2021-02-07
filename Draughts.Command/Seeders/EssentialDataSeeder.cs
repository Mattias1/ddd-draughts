using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using Draughts.Test.TestHelpers;
using System;

namespace Draughts.Command.Seeders {
    class EssentialDataSeeder {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public EssentialDataSeeder(IAuthUserRepository authUserRepository, IRoleRepository roleRepository,
            IUnitOfWork unitOfWork, IUserRepository userRepository
        ) {
            _authUserRepository = authUserRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public void SeedData() {
            EnsureDatabasesAreEmpty();

            IdTestHelper.Seed(1);

            SeedUserDomain(out var admin);
            SeedAuthUserDomain(admin);

            UpdateAvailableId();
        }

        private void EnsureDatabasesAreEmpty() {
            _unitOfWork.WithUserTransaction(tran => {
                if (_userRepository.Count() > 0) {
                    throw new InvalidOperationException("User table is not empty.");
                }
                tran.Commit();
            });
            _unitOfWork.WithAuthUserTransaction(tran => {
                if (_authUserRepository.Count() > 0) {
                    throw new InvalidOperationException("Auth user table is not empty.");
                }
                if (_roleRepository.Count() > 0) {
                    throw new InvalidOperationException("Role table is not empty.");
                }
                tran.Commit();
            });
        }

        private void SeedUserDomain(out User admin) {
            var adminUser = UserTestHelper.User("admin").Build();

            _unitOfWork.WithUserTransaction(tran => {
                _userRepository.Save(adminUser);

                tran.Commit();
            });

            admin = adminUser;
        }

        private void SeedAuthUserDomain(User admin) {
            var pendingRegistrationRole = RoleTestHelper.PendingRegistration().Build();
            var registeredUserRole = RoleTestHelper.RegisteredUser().Build();
            var adminRole = RoleTestHelper.Admin().Build();

            var adminAuthUser = AuthUserTestHelper.FromUserAndRoles(admin, registeredUserRole, adminRole).Build();

            _unitOfWork.WithAuthUserTransaction(tran => {
                _roleRepository.Save(pendingRegistrationRole);
                _roleRepository.Save(registeredUserRole);
                _roleRepository.Save(adminRole);

                _authUserRepository.Save(adminAuthUser);

                tran.Commit();
            });
        }

        private void UpdateAvailableId() {
            using (var tranFlavor = DbContext.Get.MiscTransaction()) {
                if (DbContext.Get.Query(tranFlavor).Select().CountAll().From("id_generation").SingleLong() != 0) {
                    throw new InvalidOperationException("Id generation table is not empty.");
                }

                var idGeneration = new DbIdGeneration { AvailableId = IdTestHelper.Next() };
                DbContext.Get.Query(tranFlavor).InsertInto("id_generation").InsertFrom(idGeneration).Execute();

                tranFlavor.Commit();
            }
        }
    }
}
