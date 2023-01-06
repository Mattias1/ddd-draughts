using Draughts.Common;
using Draughts.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Draughts.Domain.GameContext.Models;

// This class is like a mutable value object. It could be immutable, but that'd be not very performant. Maybe. Hmm. :/
// It's not an entity either. It doesn't really have an identity.
public sealed class Board {
    private readonly Square[] _squares; // Well, or hexes really, but close enough
    public IBoardType Type { get; }
    public int DisplaySize => Type.DisplaySize;
    public int LongSize => Type.LongSize;

    /// <summary>
    /// The square at (x, y).
    /// The top left square is (0, 0).
    /// </summary>
    public Square? this[int x, int y] => Type.IsPlayable(x, y) ? this[SquareId.FromPosition(x, y, Type)] : null;
    public Square this[SquareId n] {
        get => _squares[n.Value - 1];
        private set => _squares[n.Value - 1] = value;
    }

    public int NrOfPlayableSquares => _squares.Length;

    private Board(IBoardType type, Piece[] pieces) {
        Type = type;
        var squares = new Square[pieces.Length];
        for (int i = 0; i < squares.Length; i++) {
            squares[i] = new Square(new SquareId(i + 1), pieces[i], this);
        }
        _squares = squares;
    }

    public Move PerformNewMove(SquareId from, SquareId to, GameSettings settings, out bool canCaptureMore) {
        var currentTurn = this[from].ColorOfPiece ?? throw new ManualValidationException($"Invalid move ({from}, {to}).");
        var possibleMoves = PossibleMoveCalculator.ForNewTurn(this, currentTurn, settings).Calculate();
        return PerformMove(from, to, possibleMoves, out canCaptureMore);
    }

    public Move PerformChainCaptureMove(SquareId from, SquareId to, GameSettings settings, out bool canCaptureMore) {
        var possibleMoves = PossibleMoveCalculator.ForChainCaptures(this, from, settings).Calculate();
        return PerformMove(from, to, possibleMoves, out canCaptureMore);
    }

    private Move PerformMove(SquareId from, SquareId to, IReadOnlyList<PossibleMove> possibleMoves, out bool canCaptureMore) {
        var move = possibleMoves.SingleOrDefault(m => m.From == from && m.To == to);
        if (move is null) {
            throw new ManualValidationException($"Invalid move ({from}, {to}).");
        }

        canCaptureMore = move.MoreCapturesAvailable;
        PerformMoveUnsafe(from, to, move.Victim);

        if (!canCaptureMore && HasManOnLastRow(to)) {
            PromoteUnsafe(to);
        }

        if (!canCaptureMore) {
            CleanUpBodies();
        }

        return move;
    }

    public void PerformMoveUnsafe(SquareId from, SquareId to, SquareId? victim) {
        this[to].Piece = this[from].Piece;
        this[from].Piece = Piece.Empty;
        if (victim is not null) {
            this[victim].Piece = this[victim].Piece.Killed();
        }
    }

    public void UndoMoveUnsafe(SquareId from, SquareId to, SquareId? victim, Piece capturedPiece) {
        this[from].Piece = this[to].Piece;
        this[to].Piece = Piece.Empty;
        if (victim is not null) {
            this[victim].Piece = capturedPiece;
        }
    }

    private void PromoteUnsafe(SquareId square) {
        this[square].Piece = this[square].Piece.Promoted();
    }

    private bool HasManOnLastRow(SquareId squareId) {
        var square = this[squareId];
        if (square.IsEmpty || square.HasKing) {
            return false;
        }
        (int x, int y) = square.ToPosition();
        return Type.IsLastRow(square.ColorOfPiece, x, y);
    }

    private void CleanUpBodies() {
        _squares.Where(s => s.HasDeadPiece).ForEach(s => s.Piece = Piece.Empty);
    }

    public int NrOfPiecesPerColor(Color color) => _squares.Count(p => p.ColorOfPiece == color);

    public override string ToString() => ToLongString("", "");
    public string ToLongString(string separator = "\n", string empty = " ") {
        var sb = new StringBuilder(_squares.Length * 2 + LongSize);
        for (int y = 0; y < LongSize; y++) {
            for (int x = 0; x < LongSize; x++) {
                sb.Append(this[x, y]?.Piece.ToHexString() ?? empty);
            }
            if (y != LongSize - 1) {
                sb.Append(separator);
            }
        }
        return sb.ToString();
    }

    public Board Copy() => new Board(Type, _squares.Select(s => s.Piece).ToArray());

    public static Board FromString(IBoardType boardType, string input, string separator = "\n", string empty = " ") {
        var pieces = input.ToCharArray().Select(c => c.ToString())
            .Where(s => s != empty && s != separator)
            .Select(s => Piece.FromHexString(s))
            .ToArray();
        if (pieces.Length != boardType.AmountOfSquares()) {
            throw new InvalidOperationException($"Invalid board, expected {boardType.AmountOfSquares()} pieces, "
                + $"got {pieces.Length}.");
        }
        return new Board(boardType, pieces);
    }

    public static Board InitialSetup(IBoardType boardType) {
        return new Board(boardType, boardType.InitialPieces());
    }

    public override bool Equals(object? obj) => Equals(obj as Board);
    public bool Equals(Board? other) {
        return other is not null && _squares.Select(s => s.Piece).SequenceEqual(other._squares.Select(s => s.Piece));
    }

    public override int GetHashCode() => ComparisonUtils.GetHashCode(_squares);

    public static bool operator ==(Board? left, Board? right) => ComparisonUtils.NullSafeEquals(left, right);
    public static bool operator !=(Board? left, Board? right) => ComparisonUtils.NullSafeNotEquals(left, right);
}
