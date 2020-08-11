using Draughts.Common.Events;
using Draughts.Common.Utils;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Testing;

namespace Draughts.Test.Common.Events {
    [TestClass]
    public class DomainEventTest {
        private readonly IClock _clock;

        public DomainEventTest() {
            _clock = FakeClock.FromUtc(2020, 02, 29);
        }

        [TestMethod]
        public void Events_ShouldBeEqual_WhenIdsAreEqual() {
            var pendingRole = RoleTestHelper.PendingRegistration().WithId(11).Build();
            var left = new RoleCreated(pendingRole, new DomainEventId(1), _clock.UtcNow());

            var registeredRole = RoleTestHelper.RegisteredUser().WithId(12).Build();
            var right = new RoleCreated(registeredRole, new DomainEventId(1), _clock.UtcNow());

            left.Equals((object)right).Should().BeTrue();
            left.Equals(right).Should().BeTrue();
            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
            left.GetHashCode().Should().Be(right.GetHashCode());
        }

        [TestMethod]
        public void Events_ShouldNotBeEqual_WhenIdsAreDifferent() {
            var role = RoleTestHelper.PendingRegistration().WithId(11).Build();
            var left = new RoleCreated(role, new DomainEventId(1), _clock.UtcNow());
            var right = new RoleCreated(role, new DomainEventId(2), _clock.UtcNow());

            left.Equals((object)right).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();
        }

        [TestMethod]
        public void Events_ShouldNotBeEqual_WhenTheOtherIsNull() {
            var role = RoleTestHelper.PendingRegistration().WithId(11).Build();
            var left = new RoleCreated(role, new DomainEventId(1), _clock.UtcNow());
            RoleCreated? right = null;

            left.Equals(right as object).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();

            (right == left).Should().BeFalse();
            (right != left).Should().BeTrue();
        }

        [TestMethod]
        public void Events_ShouldBeEqual_WhenBothAreNull() {
            RoleCreated? left = null;
            RoleCreated? right = null;

            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
        }

        [TestMethod]
        public void RegisterFailedAttempt_ShouldUpdateDateAndNrOfAttempts() {
            var role = RoleTestHelper.PendingRegistration().Build();
            var evt = new RoleCreated(role, new DomainEventId(IdTestHelper.Next()), _clock.UtcNow().PlusHours(-1));

            evt.RegisterFailedAttempt(_clock.UtcNow());

            evt.LastAttemptedAt.Should().Be(_clock.UtcNow());
            evt.NrOfAttempts.Should().Be(1);
        }
    }
}
