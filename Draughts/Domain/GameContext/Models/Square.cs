using System.Diagnostics.CodeAnalysis;

namespace Draughts.Domain.GameContext.Models;

// Right now this class is mutable because the Board is.
// TODO: FIX THIS
public sealed class Square {
    private readonly Board _board;
    public SquareId Id { get; }
    public Piece Piece { get; internal set; }

    public Square(SquareId id, Piece piece, Board board) {
        Id = id;
        Piece = piece;
        _board = board;
    }

    public bool IsEmpty => Piece.IsEmpty;
    public bool IsNotEmpty => Piece.IsNotEmpty;
    public Color? ColorOfPiece => Piece.Color;
    public bool HasMan => Piece.IsMan;
    public bool HasKing => Piece.IsKing;
    public bool HasLivingPiece => Piece.IsAlive;
    public bool HasDeadPiece => Piece.IsDead;

    public (int x, int y) ToPosition() => Id.ToPosition(_board.Type);

    public bool TryGetBorder(Direction direction, [NotNullWhen(returnValue: true)] out Square? square) {
        square = GetBorder(direction);
        return square is not null;
    }

    public Square? GetBorder(Direction direction) {
        var (currentX, currentY) = ToPosition();
        int borderX = currentX + direction.DX;
        int borderY = currentY + direction.DY;
        return _board.Type.IsInRange(borderX, borderY) ? _board[borderX, borderY] : null;
    }

    public (Square? target, Square? jump) GetTargetAndJump(Direction direction) {
        var target = GetBorder(direction);
        var jump = target?.GetBorder(direction);
        return (target, jump);
    }

    public override string ToString() => $"{Id}: {(IsEmpty ? "empty" : Piece.CssClasses)}";
}
