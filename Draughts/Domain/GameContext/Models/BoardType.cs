using Draughts.Common;
using Draughts.Common.OoConcepts;
using Draughts.Common.Utilities;
using System;
using System.Collections.Generic;

namespace Draughts.Domain.GameContext.Models;

public interface IBoardType : IEquatable<IBoardType> {
    int DisplaySize { get; }
    int LongSize { get; }

    (int x, int y) CoordinateFor(SquareId squareId);
    SquareId SquareIdFor(int x, int y);
    (int x, int cssX, int cssY) AnnotationCoordinatesForRow(int row);
    (int y, int cssX, int cssY) AnnotationCoordinatesForCol(int col);
    (int cssX, int cssY) CssCoordinatesFor(int x, int y);

    bool IsInRange(int x, int y);
    bool IsPlayable(int x, int y);

    bool IsLastRow(Color? forColor, int x, int y);
    Piece[] InitialPieces();
    int AmountOfSquares();

    Direction[] AllDirections();
}

public sealed class SquareBoardType : ValueObject<SquareBoardType>, IBoardType {
    // |_|4|_|4|       |_|1|_|2|       N
    // |.|_|.|_|       |3|_|4|_|    W  +  E
    // |_|.|_|.|       |_|5|_|6|       S
    // |5|_|5|_|       |7|_|8|_|
    public int DisplaySize { get; }
    public int LongSize { get; }

    public SquareBoardType(int displaySize) {
        DisplaySize = displaySize;
        LongSize = displaySize;
    }

    public (int x, int y) CoordinateFor(SquareId squareId) {
        int y = (squareId.Value - 1) * 2 / LongSize;
        int x = (squareId.Value * 2 - 1) % LongSize - y % 2;
        return (x, y);
    }

    public SquareId SquareIdFor(int x, int y) {
        if (!IsPlayable(x, y)) {
            throw new ManualValidationException($"This position ({x}, {y}) is not playable.");
        }
        return new SquareId((x + 2 + y * LongSize) / 2);
    }

    public (int x, int cssX, int cssY) AnnotationCoordinatesForRow(int row) {
        int x = IsPlayable(0, row) ? 0 : 1;
        var (cssX, cssY) = CssCoordinatesFor(-1, row);
        return (x, cssX, cssY);
    }

    public (int y, int cssX, int cssY) AnnotationCoordinatesForCol(int col) {
        int y = IsPlayable(col, LongSize - 1) ? LongSize - 1 : LongSize - 2;
        var (cssX, cssY) = CssCoordinatesFor(col, LongSize);
        return (y, cssX, cssY);
    }

    public (int cssX, int cssY) CssCoordinatesFor(int x, int y) => (60 * x, 60 * y);

    public bool IsInRange(int x, int y) => x >= 0 && x < LongSize && y >= 0 && y < LongSize;

    public bool IsPlayable(int x, int y) => (x + y) % 2 == 1;

    public bool IsLastRow(Color? forColor, int x, int y) => forColor == Color.Black ? y == DisplaySize - 1 : y == 0;

    public Piece[] InitialPieces() {
        int nrOfStartingPieces = LongSize * (LongSize - 2) / 4;
        var pieces = new Piece[nrOfStartingPieces + LongSize + nrOfStartingPieces];
        for (int i = 0; i < nrOfStartingPieces; i++) {
            pieces[i] = Piece.BlackMan;
            pieces[pieces.Length - i - 1] = Piece.WhiteMan;
        }
        for (int i = 0; i < LongSize; i++) {
            pieces[i + nrOfStartingPieces] = Piece.Empty;
        }
        return pieces;
    }

    public int AmountOfSquares() => LongSize * LongSize / 2;

    public Direction[] AllDirections() => Direction.SQUARE_ALL;

    public bool Equals(IBoardType? obj) => obj is SquareBoardType other && EqualsCore(other);

    protected override IEnumerable<object> GetEqualityComponents() {
        yield return DisplaySize;
    }

    public static bool operator ==(SquareBoardType? left, IBoardType? right) => ComparisonUtils.NullSafeEquals(left, right);
    public static bool operator !=(SquareBoardType? left, IBoardType? right) => ComparisonUtils.NullSafeNotEquals(left, right);
}

public sealed class HexagonalBoardType : ValueObject<HexagonalBoardType>, IBoardType {
    //   |.|.|.|    Side view    |03|07|12|__|__|      E
    //  |4|.|.|5|               |02|06|11|16|__|    N  +  S
    // |4|4|.|5|5|  <- You     |01|05|10|15|19|        W
    //  |4|.|.|5|             |__|04|09|14|18|
    //   |.|.|.|             |__|__|08|13|17|
    public int DisplaySize { get; }
    public int LongSize { get; }

    public HexagonalBoardType(int displaySize) {
        DisplaySize = displaySize;
        LongSize = displaySize * 2 - 1;
    }

    public (int x, int y) CoordinateFor(SquareId squareId) {
        // For the hexagonal board coordinate logic, we assume that the hex is a (stretched) square,
        // and then add to compensate for the out-of-range squares
        int compensatedValue = squareId.Value;
        for (int i = 0; i < DisplaySize - 1; i++) { // North-west triangle
            if (compensatedValue > i * LongSize) {
                compensatedValue += LongSize - DisplaySize - i;
            }
        }
        for (int i = 1; i < DisplaySize - 1; i++) { // South-east triangle
            if (compensatedValue > LongSize * (DisplaySize + i) - i) {
                compensatedValue += i;
            }
        }
        int y = (compensatedValue - 1) / LongSize;
        int x = (compensatedValue - 1) % LongSize;
        return (x, y);
    }

    public SquareId SquareIdFor(int x, int y) {
        if (!IsPlayable(x, y)) {
            throw new ManualValidationException($"This position ({x}, {y}) is not playable.");
        }

        // For the hexagonal board coordinate logic, we assume that the hex is a (stretched) square,
        // and then subtract to compensate for the out-of-range squares
        int value = x + y * LongSize + 1;
        for (int i = 1; i < DisplaySize; i++) {
            if (y + 1 >= i) {
                value += i - DisplaySize;
            }
            if (y > LongSize - i) {
                value += i - DisplaySize;
            }
        }
        return new SquareId(value);
    }

    public (int x, int cssX, int cssY) AnnotationCoordinatesForRow(int row) {
        int x = row < DisplaySize ? DisplaySize - row - 1 : 0;
        var (cssX, cssY) = CssCoordinatesFor(x - 1, row);
        return (x, cssX, cssY);
    }

    public (int y, int cssX, int cssY) AnnotationCoordinatesForCol(int col) {
        int y = col < DisplaySize ? LongSize - 1 : LongSize + DisplaySize - col - 2;
        var (cssX, cssY) = CssCoordinatesFor(col, y + 1);
        return (y, cssX, cssY);
    }

    // The conditional is here because the pixel coordinates of the hexagons in the SVG are rounded to pixels...   :(
    public (int cssX, int cssY) CssCoordinatesFor(int x, int y) => (11 + 62 * x - (x < 5 ? 1 : 0), 72 * y + 36 * x - 36 * DisplaySize + 36 + 4);

    public bool IsInRange(int x, int y) => x >= 0 && x < LongSize && y >= 0 && y < LongSize;

    public bool IsPlayable(int x, int y) => x + y > DisplaySize - 2 && x + y < LongSize + DisplaySize - 1;

    public bool IsLastRow(Color? forColor, int x, int y) {
        return forColor == Color.Black
            ? y == LongSize - 1 || x + y == LongSize + DisplaySize - 2
            : y == 0 || x + y == DisplaySize - 1;
    }

    public Piece[] InitialPieces() {
        var pieces = new Piece[LongSize * LongSize - DisplaySize * (DisplaySize - 1)];
        int dir = 1;
        int index = 0;
        for (int rowSize = DisplaySize; rowSize >= DisplaySize; rowSize += dir) {
            if (rowSize == LongSize) { // Empty center row
                for (int i = 0; i < rowSize; i++) {
                    pieces[index++] = Piece.Empty;
                }
                dir = -1;
            } else if (dir > 0) { // First half of the rows
                for (int i = 0; i < rowSize; i++) {
                    pieces[index++] = i < DisplaySize - 1 ? Piece.BlackMan : Piece.Empty;
                }
            } else if (dir < 0) { // Last half of the rows
                for (int i = 0; i < rowSize; i++) {
                    pieces[index++] = rowSize - i > DisplaySize - 1 ? Piece.Empty : Piece.WhiteMan;
                }
            }
        }
        return pieces;
    }

    public int AmountOfSquares() => LongSize * LongSize - DisplaySize * (DisplaySize - 1);

    public Direction[] AllDirections() => Direction.HEX_ALL;

    public bool Equals(IBoardType? obj) => obj is HexagonalBoardType other && EqualsCore(other);

    protected override IEnumerable<object> GetEqualityComponents() {
        yield return DisplaySize;
    }

    public static bool operator ==(HexagonalBoardType? left, IBoardType? right) => ComparisonUtils.NullSafeEquals(left, right);
    public static bool operator !=(HexagonalBoardType? left, IBoardType? right) => ComparisonUtils.NullSafeNotEquals(left, right);
}
