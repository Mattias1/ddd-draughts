using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime.Testing;
using Xunit;

namespace Draughts.Test.Common;

public sealed class JsonWebTokenTest {
    public static readonly UserId UserId = new UserId(1);
    public static readonly string JWT_STRING = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJEcmF1Z2h0cyIsImV" +
        "4cCI6MTU3OTI2MjQwMCwidXNyIjoxLCJ1bmEiOiJVc2VyIiwicm9sIjpbMl19.EifVPA51buvZt8mveaQ9lTWAPawk0qdGz6LIM1nolhM";

    [Fact]
    public void TestGenerateJwtString() {
        var registeredUser = RoleTestHelper.RegisteredUser().WithId(2).Build();
        var authUser = BuildAuthUser(UserId, "User", registeredUser);

        var clock = FakeClock.FromUtc(2020, 01, 16, 12, 0, 0);

        var token = JsonWebToken.Generate(authUser, clock);
        string jwtString = token.ToJwtString();

        jwtString.Should().Be(JWT_STRING);
    }

    [Fact]
    public void ParseValidJwtString() {
        var clock = FakeClock.FromUtc(2020, 01, 16, 12, 0, 0);

        bool success = JsonWebToken.TryParseFromJwtString(JWT_STRING, clock, out var jwt);

        success.Should().BeTrue();
        jwt.Should().NotBeNull();
        jwt!.UserId.Should().Be(UserId);
    }

    [Fact]
    public void AbortParsingJwtStringWhenExpired() {
        var clock = FakeClock.FromUtc(2020, 12, 31);

        bool success = JsonWebToken.TryParseFromJwtString(JWT_STRING, clock, out var jwt);

        success.Should().BeFalse();
        jwt.Should().BeNull();
    }

    [Fact]
    public void AbortParsingJwtStringWhenInvalidHash() {
        var clock = FakeClock.FromUtc(2020, 01, 16, 12, 0, 0);

        string invalidJwtString = JWT_STRING.Substring(0, JWT_STRING.Length - 1) + 'A';
        bool success = JsonWebToken.TryParseFromJwtString(invalidJwtString, clock, out var jwt);

        success.Should().BeFalse();
        jwt.Should().BeNull();
    }

    private static AuthUser BuildAuthUser(UserId id, string name, params Role[] roles) {
        return AuthUserTestHelper.User(name).WithId(id).WithRoles(roles).Build();
    }
}
