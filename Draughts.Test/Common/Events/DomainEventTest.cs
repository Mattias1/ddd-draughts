using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using Xunit;
using NodaTime;
using NodaTime.Testing;

namespace Draughts.Test.Common.Events {
    public class DomainEventTest {
        private readonly IClock _clock;

        public DomainEventTest() {
            _clock = FakeClock.FromUtc(2020, 02, 29);
        }

        [Fact]
        public void EqualWhenIdsAreEqual() {
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

        [Fact]
        public void NotEqualWhenIdsAreDifferent() {
            var role = RoleTestHelper.PendingRegistration().WithId(11).Build();
            var left = new RoleCreated(role, new DomainEventId(1), _clock.UtcNow());
            var right = new RoleCreated(role, new DomainEventId(2), _clock.UtcNow());

            left.Equals((object)right).Should().BeFalse();
            left.Equals(right).Should().BeFalse();
            (left == right).Should().BeFalse();
            (left != right).Should().BeTrue();
        }

        [Fact]
        public void NotEqualWhenTheOtherIsNull() {
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

        [Fact]
        public void EqualWhenBothAreNull() {
            RoleCreated? left = null;
            RoleCreated? right = null;

            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
        }

        [Fact]
        public void RegisterFailedAttemptShouldUpdateDateAndNrOfAttempts() {
            var role = RoleTestHelper.PendingRegistration().Build();
            var evt = new RoleCreated(role, new DomainEventId(IdTestHelper.Next()), _clock.UtcNow().PlusHours(-1));

            evt.RegisterFailedAttempt(_clock.UtcNow());

            evt.LastAttemptedAt.Should().Be(_clock.UtcNow());
            evt.NrOfAttempts.Should().Be(1);
        }
    }
}
