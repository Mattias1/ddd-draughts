using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.AuthContext;

public sealed class EditAccountInfoTest {
    private const string CURRENT_PASSWORD = "current_password";
    private const string NEW_EMAIL = "new_email@example.com";
    private const string NEW_PASSWORD = "new_password";

    [Fact]
    public void ThrowWhenProvidingIncorrectPassword() {
        var authUser = AuthUserTestHelper.User().WithPassword(CURRENT_PASSWORD).Build();
        Action updating = () => authUser.UpdateEmailOrPassword("incorrect_password", NEW_EMAIL, NEW_PASSWORD);

        updating.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void ThrowWhenNotUpdatingAnything() {
        var authUser = AuthUserTestHelper.User().WithPassword(CURRENT_PASSWORD).Build();
        Action updating = () => authUser.UpdateEmailOrPassword(CURRENT_PASSWORD, null, null);

        updating.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void UpdateOnlyEmail() {
        var authUser = AuthUserTestHelper.User().WithPassword(CURRENT_PASSWORD).Build();
        authUser.UpdateEmailOrPassword(CURRENT_PASSWORD, NEW_EMAIL, null);

        authUser.Email.Should().Be(new Email(NEW_EMAIL));
        authUser.PasswordHash.CanLogin(CURRENT_PASSWORD, authUser.Id, authUser.Username).Should().BeTrue();
    }

    [Fact]
    public void UpdateOnlyPassword() {
        var authUser = AuthUserTestHelper.User().WithPassword(CURRENT_PASSWORD).Build();
        authUser.UpdateEmailOrPassword(CURRENT_PASSWORD, null, NEW_PASSWORD);

        authUser.Email.Should().NotBe(new Email(NEW_EMAIL));
        authUser.PasswordHash.CanLogin(NEW_PASSWORD, authUser.Id, authUser.Username).Should().BeTrue();
    }

    [Fact]
    public void UpdateEmailAndPassword() {
        var authUser = AuthUserTestHelper.User().WithPassword(CURRENT_PASSWORD).Build();
        authUser.UpdateEmailOrPassword(CURRENT_PASSWORD, NEW_EMAIL, NEW_PASSWORD);

        authUser.Email.Should().Be(new Email(NEW_EMAIL));
        authUser.PasswordHash.CanLogin(NEW_PASSWORD, authUser.Id, authUser.Username).Should().BeTrue();
    }
}
