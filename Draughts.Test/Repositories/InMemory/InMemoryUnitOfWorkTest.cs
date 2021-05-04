using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserContext.Events;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.InMemory;
using Draughts.Repositories.Transaction;
using Draughts.Test.Fakes;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime.Testing;
using System;
using Xunit;

namespace Draughts.Test.Repositories.InMemory {
    public class InMemoryUnitOfWorkTest {
        private readonly FakeClock _clock;
        private readonly FakeDomainEventHandler _fakeDomainEventHandler;
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryUnitOfWorkTest() {
            _clock = FakeClock.FromUtc(2020, 02, 29);
            _fakeDomainEventHandler = new FakeDomainEventHandler();
            _unitOfWork = new InMemoryUnitOfWork(_clock, IdTestHelper.BuildFakeGenerator());

            AuthUserDatabase.Get.AuthUsersTable.Clear();
            AuthUserDatabase.Get.RolesTable.Clear();
            AuthUserDatabase.Get.DomainEventsTable.Clear();
        }

        [Fact]
        public void DontPersistAnythingWhenNotCommitting() {
            using (var tran = _unitOfWork.BeginTransaction(TransactionDomain.AuthUser)) {
                StoreTestRole(1);

                // No commit
            }

            AuthUserDatabase.Get.RolesTable.Should().BeEmpty();
        }

        [Fact]
        public void RollbackWhenThrowing() {
            try {
                _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                    StoreTestRole(1);
                    int zero = 0;
                    int result = 42 / zero;
                });
            }
            catch (DivideByZeroException) {
                _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                    StoreTestRole(2);

                    tran.Commit();
                });
            }

            AuthUserDatabase.Get.RolesTable.Should().Contain(r => r.Id == 2);
            AuthUserDatabase.Get.RolesTable.Should().HaveCount(1);
        }

        [Fact]
        public void PersistsEverythingWhenCommitting() {
            using (var tran = _unitOfWork.BeginTransaction(TransactionDomain.AuthUser)) {
                StoreTestRole(1);
                StoreTestRole(2);

                tran.Commit();
            }

            AuthUserDatabase.Get.RolesTable.Should().Contain(r => r.Id == 1);
            AuthUserDatabase.Get.RolesTable.Should().Contain(r => r.Id == 2);
            AuthUserDatabase.Get.RolesTable.Should().HaveCount(2);
        }

        [Fact]
        public void ThrowWhenATransactionWithThisDomainIsAlreadyStarted() {
            using (var tran = _unitOfWork.BeginTransaction(TransactionDomain.AuthUser)) {
                Action beginTransAction = () => _unitOfWork.BeginTransaction(TransactionDomain.AuthUser);
                beginTransAction.Should().Throw<InvalidOperationException>();
            }
        }

        [Fact]
        public void StoringThrowsWhenNoTransactionIsOpen() {
            Action storeAction = () => StoreTestRole(1);

            storeAction.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void DontFireEventWhenRollbacking() {
            _unitOfWork.Register(_fakeDomainEventHandler);

            _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                RaiseEvent(1);

                // No commit
            });

            _fakeDomainEventHandler.HandledEvents.Should().BeEmpty();
        }

        [Fact]
        public void FireEventWhenCommitting() {
            _unitOfWork.Register(_fakeDomainEventHandler);

            _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                RaiseEvent(1);

                tran.Commit();
            });

            _fakeDomainEventHandler.HandledEvents.Should().Contain(evt => evt.Id == 1);
            _fakeDomainEventHandler.HandledEvents.Should().HaveCount(1);
        }

        [Fact]
        public void FireEventOnlyOnceWhenCommittingAndReFiringAll() {
            _unitOfWork.Register(_fakeDomainEventHandler);

            _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                RaiseEvent(1);

                tran.Commit();
            });
            _unitOfWork.FireAll();

            _fakeDomainEventHandler.HandledEvents.Should().Contain(evt => evt.Id == 1);
            _fakeDomainEventHandler.HandledEvents.Should().HaveCount(1);
        }

        [Fact]
        public void RaisingEventThrowsWhenNoTransactionIsOpen() {
            _unitOfWork.Register(_fakeDomainEventHandler);

            Action raiseAction = () => RaiseEvent(1);

            raiseAction.Should().Throw<InvalidOperationException>();
        }

        private void RaiseEvent(long id) {
            var role = RoleTestHelper.PendingRegistration().Build();
            var evt = new RoleCreated(role, new UserId(1), new DomainEventId(id), _clock.UtcNow());
            _unitOfWork.Raise(evt);
        }

        private void StoreTestRole(long id) {
            var role = BuildTestRole(id);
            _unitOfWork.Store(role, tran => AuthUserDatabase.Temp(tran).RolesTable);
        }

        private DbRole BuildTestRole(long id) {
            return new DbRole { Id = id, Rolename = $"Test {id}" };
        }
    }
}
