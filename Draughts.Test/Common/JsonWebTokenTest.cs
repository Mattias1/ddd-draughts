using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace Draughts.Test.Common {
    public class JsonWebTokenTest {
        public static readonly AuthUserId AuthUserId = new AuthUserId(1);
        public static readonly string JWT_STRING = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJEcmF1Z2h0cyIsImV" +
            "4cCI6MTU3OTI2MjQwMCwiYXV1IjoxLCJ1c3IiOjEsInVuYSI6IlVzZXIiLCJyb2wiOlsyXX0.ApProjWhz0l1FSbqyYvLxBoQjPGV_O" +
            "lbJfu_TSGZQoU";

        [Fact]
        public void TestGenerateJwtString() {
            var registeredUser = RoleTestHelper.RegisteredUser().WithId(2).Build();
            var authUser = BuildAuthUser(AuthUserId, "User", registeredUser);

            var clock = new FakeClock(new LocalDateTime(2020, 01, 16, 12, 0).InUtc().ToInstant());

            var token = JsonWebToken.Generate(authUser, clock);
            string jwtString = token.ToJwtString();

            jwtString.Should().Be(JWT_STRING);
        }

        [Fact]
        public void ParseValidJwtString() {
            var clock = new FakeClock(new LocalDateTime(2020, 01, 16, 12, 0).InUtc().ToInstant());

            bool success = JsonWebToken.TryParseFromJwtString(JWT_STRING, clock, out var jwt);

            success.Should().BeTrue();
            jwt.Should().NotBeNull();
            jwt!.AuthUserId.Should().Be(AuthUserId);
        }

        [Fact]
        public void AbortParsingJwtStringWhenExpired() {
            var clock = new FakeClock(new LocalDateTime(2021, 01, 16, 12, 0).InUtc().ToInstant());

            bool success = JsonWebToken.TryParseFromJwtString(JWT_STRING, clock, out var jwt);

            success.Should().BeFalse();
            jwt.Should().BeNull();
        }

        [Fact]
        public void AbortParsingJwtStringWhenInvalidHash() {
            var clock = new FakeClock(new LocalDateTime(2020, 01, 16, 12, 0).InUtc().ToInstant());

            string invalidJwtString = JWT_STRING.Substring(0, JWT_STRING.Length - 1) + 'A';
            bool success = JsonWebToken.TryParseFromJwtString(invalidJwtString, clock, out var jwt);

            success.Should().BeFalse();
            jwt.Should().BeNull();
        }

        private static AuthUser BuildAuthUser(long id, string name, params Role[] roles) {
            return AuthUserTestHelper.User(name).WithId(id).WithUserId(id).WithRoles(roles).Build();
        }
    }
}
