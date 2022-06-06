using Draughts.Domain.GameContext.Models;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class PieceTest {
    [Fact]
    public void TestIsMan() {
        Piece.Empty.IsMan.Should().BeFalse();
        Piece.BlackMan.IsMan.Should().BeTrue();
        Piece.WhiteMan.IsMan.Should().BeTrue();
        Piece.BlackKing.IsMan.Should().BeFalse();
        Piece.WhiteKing.IsMan.Should().BeFalse();
        Piece.BlackMan.Killed().IsMan.Should().BeTrue();
        Piece.WhiteKing.Killed().IsMan.Should().BeFalse();
    }

    [Fact]
    public void TestIsKing() {
        Piece.Empty.IsKing.Should().BeFalse();
        Piece.BlackMan.IsKing.Should().BeFalse();
        Piece.WhiteMan.IsKing.Should().BeFalse();
        Piece.BlackKing.IsKing.Should().BeTrue();
        Piece.WhiteKing.IsKing.Should().BeTrue();
        Piece.BlackKing.Killed().IsKing.Should().BeTrue();
        Piece.WhiteMan.Killed().IsKing.Should().BeFalse();
    }

    [Fact]
    public void TestColor() {
        Piece.Empty.Color.Should().BeNull();
        Piece.BlackMan.Color.Should().Be(Color.Black);
        Piece.WhiteMan.Color.Should().Be(Color.White);
        Piece.BlackKing.Color.Should().Be(Color.Black);
        Piece.WhiteKing.Color.Should().Be(Color.White);
        Piece.BlackMan.Killed().Color.Should().Be(Color.Black);
        Piece.WhiteKing.Killed().Color.Should().Be(Color.White);
    }

    [Fact]
    public void TestPromotion() {
        Piece.BlackMan.Promoted().Should().Be(Piece.BlackKing);
        Piece.WhiteMan.Promoted().Should().Be(Piece.WhiteKing);
        Piece.BlackKing.Promoted().Should().Be(Piece.BlackKing);
        Piece.WhiteKing.Promoted().Should().Be(Piece.WhiteKing);
    }

    [Fact]
    public void TestIsAlive() {
        Piece.Empty.IsAlive.Should().BeTrue();
        Piece.BlackKing.IsAlive.Should().BeTrue();
        Piece.WhiteMan.IsAlive.Should().BeTrue();
        Piece.BlackKing.Killed().IsAlive.Should().BeFalse();
        Piece.WhiteMan.Killed().IsAlive.Should().BeFalse();
    }

    [Fact]
    public void TestIsDead() {
        Piece.Empty.IsDead.Should().BeFalse();
        Piece.BlackMan.IsDead.Should().BeFalse();
        Piece.WhiteKing.IsDead.Should().BeFalse();
        Piece.BlackKing.Killed().IsDead.Should().BeTrue();
        Piece.WhiteMan.Killed().IsDead.Should().BeTrue();
    }

    [Fact]
    public void TestToHexString() {
        Piece.Empty.ToHexString().Should().Be("0");
        Piece.BlackMan.ToHexString().Should().Be("4");
        Piece.WhiteMan.ToHexString().Should().Be("5");
        Piece.BlackKing.ToHexString().Should().Be("6");
        Piece.WhiteKing.ToHexString().Should().Be("7");
        Piece.BlackMan.Killed().ToHexString().Should().Be("C");
        Piece.WhiteMan.Killed().ToHexString().Should().Be("D");
        Piece.BlackKing.Killed().ToHexString().Should().Be("E");
        Piece.WhiteKing.Killed().ToHexString().Should().Be("F");
    }

    [Fact]
    public void TestFromHexString() {
        Piece.FromHexString("0").Should().Be(Piece.Empty);
        Piece.FromHexString("4").Should().Be(Piece.BlackMan);
        Piece.FromHexString("5").Should().Be(Piece.WhiteMan);
        Piece.FromHexString("6").Should().Be(Piece.BlackKing);
        Piece.FromHexString("7").Should().Be(Piece.WhiteKing);
        Piece.FromHexString("C").Should().Be(Piece.BlackMan.Killed());
        Piece.FromHexString("D").Should().Be(Piece.WhiteMan.Killed());
        Piece.FromHexString("E").Should().Be(Piece.BlackKing.Killed());
        Piece.FromHexString("F").Should().Be(Piece.WhiteKing.Killed());
    }
}
