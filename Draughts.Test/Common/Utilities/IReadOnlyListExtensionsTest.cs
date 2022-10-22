using Draughts.Common.Utilities;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Common.Utilities;

public sealed class IReadOnlyListExtensionsTest {
    [Fact]
    public void TestUnpackDuo() {
        var numbers = new int[] { 42, 1337 };

        var (first, second) = numbers.UnpackDuo();

        first.Should().Be(42);
        second.Should().Be(1337);
    }

    [Fact]
    public void TestUnpackTrio() {
        var numbers = new int[] { 8, 7, 56 };

        var (first, second, third) = numbers.UnpackTrio();

        first.Should().Be(8);
        second.Should().Be(7);
        third.Should().Be(56);
    }

    [Fact]
    public void TestUnpackQuad() {
        var chars = new char[] { 'c', 'h', 'a', 'r' };

        var (first, second, third, fourth) = chars.UnpackQuad();

        first.Should().Be('c');
        second.Should().Be('h');
        third.Should().Be('a');
        fourth.Should().Be('r');
    }

    [Fact]
    public void ThrowWhenTooFewParams() {
        var numbers = new int[] { 42 };
        var unpackFunc = () => numbers.UnpackDuo();
        unpackFunc.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ThrowWhenTooManyParams() {
        var numbers = new int[] { 7, 42, 1337, 9001, 32 };
        var unpackFunc = () => numbers.UnpackDuo();
        unpackFunc.Should().Throw<InvalidOperationException>();
    }
}
