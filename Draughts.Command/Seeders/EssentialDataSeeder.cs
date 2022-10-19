using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using Draughts.Test.TestHelpers;
using System;

namespace Draughts.Command.Seeders;

public sealed class EssentialDataSeeder {
    private readonly AuthUserRepository _authUserRepository;
    private readonly RoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserRepository _userRepository;

    public EssentialDataSeeder(AuthUserRepository authUserRepository, RoleRepository roleRepository,
        IUnitOfWork unitOfWork, UserRepository userRepository
    ) {
        _authUserRepository = authUserRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public void SeedData() {
        EnsureDatabasesAreEmpty();

        SeedAvailableIds();
        var admin = SeedUserDomain();
        SeedAuthUserDomain(admin);
    }

    private void EnsureDatabasesAreEmpty() {
        _unitOfWork.WithUserTransaction(tran => {
            if (_userRepository.Count() > 0) {
                throw new InvalidOperationException("User table is not empty.");
            }
        });
        _unitOfWork.WithAuthTransaction(tran => {
            if (_authUserRepository.Count() > 0) {
                throw new InvalidOperationException("Auth user table is not empty.");
            }
            if (_roleRepository.Count() > 0) {
                throw new InvalidOperationException("Role table is not empty.");
            }
        });
        using (var tranFlavor = DbContext.Get.BeginMiscTransaction()) {
            if (DbContext.Get.Query(tranFlavor).CountAllFrom("id_generation").SingleLong() != 0) {
                throw new InvalidOperationException("Id generation table is not empty.");
            }
            tranFlavor.Commit();
        }
    }

    private void SeedAvailableIds() {
        using (var tranFlavor = DbContext.Get.BeginMiscTransaction()) {
            DbContext.Get.Query(tranFlavor)
                .InsertInto("id_generation")
                .Columns("subject", "available_id")
                .Values(DbIdGeneration.SUBJECT_MISC, 1)
                .Values(DbIdGeneration.SUBJECT_GAME, 1)
                .Values(DbIdGeneration.SUBJECT_USER, 1)
                .Execute();

            tranFlavor.Commit();
        }
    }

    private User SeedUserDomain() {
        var adminUser = UserTestHelper.User(Username.ADMIN).Build();

        _unitOfWork.WithUserTransaction(tran => {
            _userRepository.Save(adminUser);

            var createdAdminId = _userRepository.FindByName(Username.ADMIN).Id;
            if (createdAdminId != UserId.ADMIN) {
                throw new InvalidOperationException("The admin user has an unexpected id. "
                    + $"Expected {UserId.ADMIN} but was {createdAdminId}.");
            }
        });

        return adminUser;
    }

    private void SeedAuthUserDomain(User admin) {
        var pendingRegistrationRole = RoleTestHelper.PendingRegistration().Build();
        var registeredUserRole = RoleTestHelper.RegisteredUser().Build();
        var adminRole = RoleTestHelper.Admin().Build();

        var adminAuthUser = AuthUserTestHelper.FromUserAndRoles(admin, registeredUserRole, adminRole).Build();

        _unitOfWork.WithAuthTransaction(tran => {
            _roleRepository.Save(pendingRegistrationRole);
            _roleRepository.Save(registeredUserRole);
            _roleRepository.Save(adminRole);

            _authUserRepository.Save(adminAuthUser);
        });
    }
}
