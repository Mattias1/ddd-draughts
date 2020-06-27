using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Testing;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Test.Common {
    [TestClass]
    public class JsonWebTokenTest {
        public static readonly AuthUserId AuthUserId = new AuthUserId(1);
        public static readonly string JWT_STRING = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJEcmF1Z2h0cyIsImV" +
            "4cCI6MTU3OTI2MjQwMCwiYXV1IjoxLCJ1c3IiOjEsInVuYSI6IlVzZXIiLCJyb2wiOlsyXX0.ApProjWhz0l1FSbqyYvLxBoQjPGV_O" +
            "lbJfu_TSGZQoU";

        [TestMethod]
        public void TestGenerateJwtString() {
            var registeredUser = new Role(new RoleId(2), "Registered user", Permissions.PlayGame);
            var authUser = BuildAuthUser(AuthUserId, "User", registeredUser);

            var clock = new FakeClock(new LocalDateTime(2020, 01, 16, 12, 0).InUtc().ToInstant());

            var token = JsonWebToken.Generate(authUser, clock);
            string jwtString = token.ToJwtString();

            jwtString.Should().Be(JWT_STRING);
        }

        [TestMethod]
        public void TryParseJwtString_WhenAlright_ThenParseCorrectly() {
            var clock = new FakeClock(new LocalDateTime(2020, 01, 16, 12, 0).InUtc().ToInstant());

            bool success = JsonWebToken.TryParseFromJwtString(JWT_STRING, clock, out var jwt);

            success.Should().BeTrue();
            jwt.Should().NotBeNull();
            jwt!.AuthUserId.Should().Be(AuthUserId);
        }

        [TestMethod]
        public void TryParseJwtString_WhenExpired_ThenAbort() {
            var clock = new FakeClock(new LocalDateTime(2021, 01, 16, 12, 0).InUtc().ToInstant());

            bool success = JsonWebToken.TryParseFromJwtString(JWT_STRING, clock, out var jwt);

            success.Should().BeFalse();
            jwt.Should().BeNull();
        }

        [TestMethod]
        public void TryParseJwtString_WhenInvalidHash_ThenAbort() {
            var clock = new FakeClock(new LocalDateTime(2020, 01, 16, 12, 0).InUtc().ToInstant());

            string invalidJwtString = JWT_STRING.Substring(0, JWT_STRING.Length - 1) + 'A';
            bool success = JsonWebToken.TryParseFromJwtString(invalidJwtString, clock, out var jwt);

            success.Should().BeFalse();
            jwt.Should().BeNull();
        }

        private static AuthUser BuildAuthUser(long id, string name, params Role[] roles) {
            var hash = PasswordHash.Generate("admin", new AuthUserId(id), new Username(name));
            return new AuthUser(new AuthUserId(id), new UserId(id), new Username(name), hash, new Email($"{name}@example.com"), roles);
        }
    }
}
