using Draughts.Common;
using Draughts.Common.OoConcepts;
using Draughts.Common.Utilities;
using System;
using System.Collections.Generic;

namespace Draughts.Domain.GameContext.Models;

public interface IBoardType : IEquatable<IBoardType> {
    // |_|4|_|4|       |_|1|_|2|       N
    // |.|_|.|_|       |3|_|4|_|    W  +  E
    // |_|.|_|.|       |_|5|_|6|       S
    // |5|_|5|_|       |7|_|8|_|
    int DisplaySize { get; }
    int LongSize { get; }

    (int x, int y) CoordinateFor(SquareId squareId);
    SquareId SquareIdFor(int x, int y);

    bool IsInRange(int x, int y);
    bool IsPlayable(int x, int y);

    bool IsLastRow(Color? forColor, int x, int y);
    int XCoordinateForFirstSquareOfRow(int row);
    int YCoordinateForLastSquareOfCol(int col);
    Piece[] InitialPieces();
    int AmountOfSquares();

    Direction[] AllDirections();
}

public sealed class SquareBoardType : ValueObject<SquareBoardType>, IBoardType {
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

    public bool IsInRange(int x, int y) => x >= 0 && x < LongSize && y >= 0 && y < LongSize;

    public bool IsPlayable(int x, int y) => (x + y) % 2 == 1;

    public bool IsLastRow(Color? forColor, int x, int y) => forColor == Color.Black ? y == DisplaySize - 1 : y == 0;

    public int XCoordinateForFirstSquareOfRow(int row) => IsPlayable(0, row) ? 0 : 1;
    public int YCoordinateForLastSquareOfCol(int col) => IsPlayable(col, LongSize - 1) ? LongSize - 1 : LongSize - 2;

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
        for (int i = 0; i < DisplaySize - 1; i++) {
            int amountToAdd = LongSize - DisplaySize - i;
            if (compensatedValue > i * LongSize) {
                compensatedValue += amountToAdd;
            }
            if (compensatedValue >= LongSize * (LongSize - i)) {
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

    public bool IsInRange(int x, int y) => x >= 0 && x < LongSize && y >= 0 && y < LongSize;

    public bool IsPlayable(int x, int y) => x + y > 1 && x + y < 4 * DisplaySize - 5;

    public bool IsLastRow(Color? forColor, int x, int y) {
        return forColor == Color.Black
            ? y == LongSize - 1 || x + y == LongSize + DisplaySize - 2
            : y == 0 || x + y == DisplaySize - 1;
    }

    public int XCoordinateForFirstSquareOfRow(int row) => row < DisplaySize ? DisplaySize - row - 1 : 0;
    public int YCoordinateForLastSquareOfCol(int col) {
        return col < DisplaySize ? LongSize - 1 : LongSize + DisplaySize - col - 2;
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
