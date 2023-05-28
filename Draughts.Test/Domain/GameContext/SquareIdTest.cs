using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class SquareIdTest {
    private static readonly IBoardType SQUARE4 = new SquareBoardType(4);
    private static readonly IBoardType SQUARE8 = new SquareBoardType(8);
    private static readonly IBoardType HEX3 = new HexagonalBoardType(3);
    private static readonly IBoardType HEX5 = new HexagonalBoardType(5);

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(3, 0, 2)]
    [InlineData(0, 1, 3)]
    [InlineData(2, 1, 4)]
    [InlineData(1, 2, 5)]
    [InlineData(3, 2, 6)]
    [InlineData(0, 3, 7)]
    [InlineData(2, 3, 8)]
    public void CoordinateToIdOn4x4Board(int x, int y, int id) {
        // |_|1|_|2|
        // |3|_|4|_|
        // |_|5|_|6|
        // |7|_|8|_|
        SquareId.FromPosition(x, y, SQUARE4).Should().Be(id.AsSquare());
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(3, 0, 2)]
    [InlineData(5, 0, 3)]
    [InlineData(7, 0, 4)]
    [InlineData(0, 1, 5)]
    [InlineData(2, 1, 6)]
    [InlineData(6, 1, 8)]
    [InlineData(1, 2, 9)]
    [InlineData(2, 3, 14)]
    [InlineData(6, 7, 32)]
    public void CoordinateToIdOn8x8Board(int x, int y, int id) {
        SquareId.FromPosition(x, y, SQUARE8).Should().Be(id.AsSquare());
    }

    [Theory]
    [InlineData(2, 0, 1), InlineData(3, 0, 2), InlineData(4, 0, 3)]
    [InlineData(1, 1, 4), InlineData(2, 1, 5), InlineData(3, 1, 6), InlineData(4, 1, 7)]
    [InlineData(0, 2, 8), InlineData(1, 2, 9), InlineData(2, 2, 10), InlineData(3, 2, 11), InlineData(4, 2, 12)]
    [InlineData(0, 3, 13), InlineData(1, 3, 14), InlineData(2, 3, 15), InlineData(3, 3, 16)]
    [InlineData(0, 4, 17), InlineData(1, 4, 18), InlineData(2, 4, 19)]
    public void CoordinateToIdOnMiniHexdameBoard(int x, int y, int id) {
        SquareId.FromPosition(x, y, HEX3).Should().Be(id.AsSquare());
    }

    [Theory]
    [InlineData(4, 0, 1), InlineData(5, 0, 2), InlineData(6, 0, 3), InlineData(7, 0, 4), InlineData(8, 0, 5)]
    [InlineData(8, 1, 11), InlineData(8, 2, 18), InlineData(8, 3, 26), InlineData(8, 4, 35)]
    [InlineData(7, 5, 43), InlineData(6, 6, 50), InlineData(5, 7, 56), InlineData(4, 8, 61)]
    [InlineData(3, 8, 60), InlineData(2, 8, 59), InlineData(1, 8, 58), InlineData(0, 8, 57)]
    [InlineData(0, 7, 51), InlineData(0, 6, 44), InlineData(0, 5, 36), InlineData(0, 4, 27)]
    [InlineData(1, 3, 19), InlineData(2, 2, 12), InlineData(3, 1, 6)]
    public void CoordinateToIdOnHexdameBoard(int x, int y, int id) {
        SquareId.FromPosition(x, y, HEX5).Should().Be(id.AsSquare());
    }

    [Theory]
    [InlineData(0, 0), InlineData(2, 0)]
    [InlineData(1, 1), InlineData(3, 1)]
    [InlineData(0, 2), InlineData(2, 2)]
    [InlineData(1, 3), InlineData(3, 3)]
    public void NonPlayableCoordinatesOn4x4BoardShouldThrow(int x, int y) {
        // |_|.|_|.|
        // |.|x|.|_|
        // |_|.|_|.|
        // |.|_|.|_|
        Action fromPosition = () => SquareId.FromPosition(x, y, SQUARE4);
        fromPosition.Should().Throw<ManualValidationException>();
    }

    [Theory]
    [InlineData(0, 0), InlineData(1, 0), InlineData(0, 1)]
    [InlineData(4, 3), InlineData(3, 4), InlineData(4, 4)]
    public void NonPlayableCoordinatesOnMiniHexdameBoardShouldThrow(int x, int y) {
        //     |03|07|12|__|__|  Side view    E
        //    |02|06|11|16|__|             N  +  S
        //   |01|05|10|15|19|    <- You       W
        //  |__|04|09|14|18|
        // |__|__|08|13|17|
        Action fromPosition = () => SquareId.FromPosition(x, y, HEX3);
        fromPosition.Should().Throw<ManualValidationException>();
    }

    [Theory]
    [InlineData(0, 0), InlineData(1, 0), InlineData(2, 0)]
    [InlineData(0, 1), InlineData(1, 1)]
    [InlineData(0, 2)]
    [InlineData(8, 6)]
    [InlineData(7, 7), InlineData(8, 7)]
    [InlineData(6, 8), InlineData(7, 8), InlineData(8, 8)]
    public void NonPlayableCoordinatesOnHexdameBoardShouldThrow(int x, int y) {
        Action fromPosition = () => SquareId.FromPosition(x, y, HEX5);
        fromPosition.Should().Throw<ManualValidationException>();
    }

    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(2, 3, 0)]
    [InlineData(3, 0, 1)]
    [InlineData(4, 2, 1)]
    [InlineData(5, 1, 2)]
    [InlineData(6, 3, 2)]
    [InlineData(7, 0, 3)]
    [InlineData(8, 2, 3)]
    public void SquareIdToCoordinateOn4x4Board(int id, int x, int y) {
        // |_|1|_|2|
        // |3|_|4|_|
        // |_|5|_|6|
        // |7|_|8|_|
        id.AsSquare().ToPosition(SQUARE4).Should().Be((x, y));
    }

    [Theory]
    [InlineData(1, 1, 0)]
    [InlineData(2, 3, 0)]
    [InlineData(3, 5, 0)]
    [InlineData(4, 7, 0)]
    [InlineData(5, 0, 1)]
    [InlineData(6, 2, 1)]
    [InlineData(8, 6, 1)]
    [InlineData(9, 1, 2)]
    [InlineData(14, 2, 3)]
    [InlineData(32, 6, 7)]
    public void SquareIdToCoordinateOn8x8Board(int id, int x, int y) {
        id.AsSquare().ToPosition(SQUARE8).Should().Be((x, y));
    }

    [Theory]
    [InlineData(1, 2, 0)]
    [InlineData(2, 3, 0)]
    [InlineData(3, 4, 0)]
    [InlineData(4, 1, 1)]
    [InlineData(5, 2, 1)]
    [InlineData(6, 3, 1)]
    [InlineData(7, 4, 1)]
    [InlineData(8, 0, 2)]
    [InlineData(9, 1, 2)]
    [InlineData(10, 2, 2)]
    [InlineData(11, 3, 2)]
    [InlineData(12, 4, 2)]
    [InlineData(13, 0, 3)]
    [InlineData(14, 1, 3)]
    [InlineData(15, 2, 3)]
    [InlineData(16, 3, 3)]
    [InlineData(17, 0, 4)]
    [InlineData(18, 1, 4)]
    [InlineData(19, 2, 4)]
    public void SquareIdToCoordinateOnMiniHexdameBoard(int id, int x, int y) {
        //     |03|07|12|__|__|  Side view    E
        //    |02|06|11|16|__|             N  +  S
        //   |01|05|10|15|19|    <- You       W
        //  |__|04|09|14|18|
        // |__|__|08|13|17|
        id.AsSquare().ToPosition(HEX3).Should().Be((x, y));
    }

    [Theory]
    [InlineData(1, 4, 0), InlineData(2, 5, 0), InlineData(3, 6, 0), InlineData(4, 7, 0), InlineData(5, 8, 0)]
    [InlineData(11, 8, 1), InlineData(18, 8, 2), InlineData(26, 8, 3), InlineData(35, 8, 4)]
    [InlineData(43, 7, 5), InlineData(50, 6, 6), InlineData(56, 5, 7), InlineData(61, 4, 8)]
    [InlineData(60, 3, 8), InlineData(59, 2, 8), InlineData(58, 1, 8), InlineData(57, 0, 8)]
    [InlineData(51, 0, 7), InlineData(44, 0, 6), InlineData(36, 0, 5), InlineData(27, 0, 4)]
    [InlineData(19, 1, 3), InlineData(12, 2, 2), InlineData(6, 3, 1)]
    public void SquareIdToCoordinateOnHexdameBoard(int id, int x, int y) {
        //         |05|11|18|26|35|__|__|__|__|  Side view
        //        |04|10|17|25|34|43|__|__|__|                E
        //       |03|09|16|24|33|42|50|__|__|              N  +  S
        //      |02|08|15|23|32|41|49|56|__|                  W
        //     |01|07|14|22|31|40|48|55|61|      <- You
        //    |__|06|13|21|30|39|47|54|60|
        //   |__|__|12|20|29|38|46|53|59|
        //  |__|__|__|19|28|37|45|52|58|
        // |__|__|__|__|27|36|44|51|57|
        id.AsSquare().ToPosition(HEX5).Should().Be((x, y));
    }

    // TODO: These are BoardType tests, not SquareId tests
    [Theory]
    [InlineData(0, 0, false), InlineData(1, 0, true), InlineData(2, 0, false), InlineData(3, 0, true)]
    [InlineData(4, 0, false), InlineData(5, 0, true), InlineData(6, 0, false), InlineData(7, 0, true)]
    [InlineData(0, 1, true), InlineData(1, 1, false), InlineData(2, 1, true), InlineData(3, 1, false)]
    [InlineData(6, 1, true), InlineData(7, 1, false)]
    [InlineData(0, 2, false), InlineData(1, 2, true)]
    [InlineData(2, 3, true), InlineData(3, 3, false)]
    [InlineData(6, 7, true), InlineData(7, 7, false)]
    public void IsPlayable(int x, int y, bool expectedResult) {
        SQUARE8.IsPlayable(x, y).Should().Be(expectedResult);
    }

    // TODO: These are Square tests, not SquareId tests
    [Theory]
    [InlineData(6, 1, 2, 10, 9)]
    [InlineData(9, 5, 6, 14, 13)]
    [InlineData(4, null, null, null, 8)]
    [InlineData(29, null, 25, null, null)]
    public void BorderSquaresOn8x8Board(int id, int? nw, int? ne, int? se, int? sw) {
        var board = Board.InitialSetup(SQUARE8);
        var square = board[id.AsSquare()];

        Square? result;
        square.TryGetBorder(Direction.SQUARE_NW, out result).Should().Be(nw is not null);
        result?.Id.Value.Should().Be(nw);

        square.TryGetBorder(Direction.SQUARE_NE, out result).Should().Be(ne is not null);
        result?.Id.Value.Should().Be(ne);

        square.TryGetBorder(Direction.SQUARE_SE, out result).Should().Be(se is not null);
        result?.Id.Value.Should().Be(se);

        square.TryGetBorder(Direction.SQUARE_SW, out result).Should().Be(sw is not null);
        result?.Id.Value.Should().Be(sw);
    }

    [Theory]
    [InlineData(5, 1, 2, 6, 10, 9, 4)]
    [InlineData(1, null, null, 2, 5, 4, null)]
    [InlineData(3, null, null, null, 7, 6, 2)]
    [InlineData(16, 11, 12, null, null, 19, 15)]
    [InlineData(19, 15, 16, null, null, null, 18)]
    [InlineData(13, 8, 9, 14, 17, null, null)]
    public void BorderSquaresOnMiniHexdameBoard(int id, int? n, int? ne, int? se, int? s, int? sw, int? nw) {
        //   |03|07|12|      Side view    E
        //  |02|06|11|16|              N  +  S
        // |01|05|10|15|19|  <- You       W
        //  |04|09|14|18|
        //   |08|13|17|
        var board = Board.InitialSetup(HEX3);
        var square = board[id.AsSquare()];

        Square? result;
        square.TryGetBorder(Direction.HEX_N, out result).Should().Be(n is not null);
        result?.Id.Value.Should().Be(n);

        square.TryGetBorder(Direction.HEX_NE, out result).Should().Be(ne is not null);
        result?.Id.Value.Should().Be(ne);

        square.TryGetBorder(Direction.HEX_SE, out result).Should().Be(se is not null);
        result?.Id.Value.Should().Be(se);

        square.TryGetBorder(Direction.HEX_S, out result).Should().Be(s is not null);
        result?.Id.Value.Should().Be(s);

        square.TryGetBorder(Direction.HEX_SW, out result).Should().Be(sw is not null);
        result?.Id.Value.Should().Be(sw);

        square.TryGetBorder(Direction.HEX_NW, out result).Should().Be(nw is not null);
        result?.Id.Value.Should().Be(nw);
    }
}
