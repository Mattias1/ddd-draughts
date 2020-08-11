using Draughts.Common.Events;
using Draughts.Common.Utils;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Repositories.Database;
using Draughts.Repositories.Databases;
using Draughts.Test.Fakes;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime.Testing;
using System;

namespace Draughts.Test.Repositories.Databases {
    [TestClass]
    public class InMemoryUnitOfWorkTest {
        private FakeClock _clock;
        private FakeDomainEventHandler _fakeDomainEventHandler;
        private IUnitOfWork _unitOfWork;

        public InMemoryUnitOfWorkTest() {
            _clock = FakeClock.FromUtc(2020, 02, 29);
            _fakeDomainEventHandler = new FakeDomainEventHandler();
            _unitOfWork = new InMemoryUnitOfWork(_clock);

            AuthUserDatabase.AuthUsersTable.Clear();
            AuthUserDatabase.RolesTable.Clear();
            AuthUserDatabase.DomainEventsTable.Clear();
        }

        [TestMethod]
        public void BeginTransaction_DoesntPersistAnything_WhenNotCommitting() {
            using (var tran = _unitOfWork.BeginTransaction(TransactionDomain.AuthUser)) {
                StoreTestRole(1);

                // No commit
            }

            AuthUserDatabase.RolesTable.Should().BeEmpty();
            AuthUserDatabase.TempRolesTable.Should().BeEmpty();
        }

        [TestMethod]
        public void BeginTransaction_PersistsUntillFlush_WhenFlushingButNotCommitting() {
            using (var tran = _unitOfWork.BeginTransaction(TransactionDomain.AuthUser)) {
                StoreTestRole(1);

                tran.Flush();

                StoreTestRole(2);

                tran.Rollback(); // No commit
            }

            AuthUserDatabase.RolesTable.Should().Contain(r => r.Id == 1);
            AuthUserDatabase.RolesTable.Should().HaveCount(1);
            AuthUserDatabase.TempRolesTable.Should().BeEmpty();
        }

        [TestMethod]
        public void BeginTransaction_PersistsEverything_WhenCommitting() {
            using (var tran = _unitOfWork.BeginTransaction(TransactionDomain.AuthUser)) {
                StoreTestRole(1);
                StoreTestRole(2);

                tran.Commit();
            }

            AuthUserDatabase.RolesTable.Should().Contain(r => r.Id == 1);
            AuthUserDatabase.RolesTable.Should().Contain(r => r.Id == 2);
            AuthUserDatabase.RolesTable.Should().HaveCount(2);
            AuthUserDatabase.TempRolesTable.Should().BeEmpty();
        }

        [TestMethod]
        public void BeginTransaction_Throw_WhenATransactionWithThisDomainIsAlreadyStarted() {
            using (var tran = _unitOfWork.BeginTransaction(TransactionDomain.AuthUser)) {
                Action beginTransAction = () => _unitOfWork.BeginTransaction(TransactionDomain.AuthUser);
                beginTransAction.Should().Throw<InvalidOperationException>();
            }
        }

        [TestMethod]
        public void Store_ShouldThrow_WhenNoTransactionIsOpen() {
            Action storeAction = () => StoreTestRole(1);

            storeAction.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void WithTransaction_ShouldNotFireEvent_WhenRollbacking() {
            _unitOfWork.Register(_fakeDomainEventHandler);

            _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                RaiseEvent(1);

                // No commit
            });

            _fakeDomainEventHandler.HandledEvents.Should().BeEmpty();
        }

        [TestMethod]
        public void WithTransaction_ShouldFireEvent_WhenCommitting() {
            _unitOfWork.Register(_fakeDomainEventHandler);

            _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                RaiseEvent(1);

                tran.Commit();
            });

            _fakeDomainEventHandler.HandledEvents.Should().Contain(evt => evt.Id == 1);
            _fakeDomainEventHandler.HandledEvents.Should().HaveCount(1);
        }

        [TestMethod]
        public void WithTransaction_ShouldFireEventOnlyOnce_WhenCommittingAndReFiringAll() {
            _unitOfWork.Register(_fakeDomainEventHandler);

            _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                RaiseEvent(1);

                tran.Commit();
            });
            _unitOfWork.FireAll();

            _fakeDomainEventHandler.HandledEvents.Should().Contain(evt => evt.Id == 1);
            _fakeDomainEventHandler.HandledEvents.Should().HaveCount(1);
        }

        [TestMethod]
        public void Raise_ShouldThrow_WhenNoTransactionIsOpen() {
            _unitOfWork.Register(_fakeDomainEventHandler);

            Action raiseAction = () => RaiseEvent(1);

            raiseAction.Should().Throw<InvalidOperationException>();
        }

        private void RaiseEvent(long id) {
            var role = RoleTestHelper.PendingRegistration().Build();
            var evt = new RoleCreated(role, new DomainEventId(id), _clock.UtcNow());
            _unitOfWork.Raise(evt);
        }

        private void StoreTestRole(long id) {
            var role = BuildTestRole(id);
            _unitOfWork.Store(role, AuthUserDatabase.TempRolesTable);
        }

        private InMemoryRole BuildTestRole(long id) {
            return new InMemoryRole { Id = id, Permissions = new string[0], Rolename = $"Test {id}" };
        }
    }
}
