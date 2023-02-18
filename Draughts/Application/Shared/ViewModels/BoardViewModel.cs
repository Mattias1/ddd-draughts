using Draughts.Domain.GameContext.Models;

namespace Draughts.Application.Shared.ViewModels;

public sealed class BoardViewModel {
    private readonly Piece?[,] _pieces;
    public IBoardType BoardType { get;  }
    public int DisplaySize { get; }
    public int LongSize { get; }

    public BoardViewModel(Board board) {
        BoardType = board.Type;
        DisplaySize = board.DisplaySize;
        LongSize = board.LongSize;
        _pieces = new Piece[LongSize, LongSize];
        for (int y = 0; y < LongSize; y++) {
            for (int x = 0; x < LongSize; x++) {
                _pieces[x, y] = board[x, y]?.Piece;
            }
        }
    }

    public Piece? At(int x, int y, bool boardIsRotated) {
        if (boardIsRotated) {
            x = LongSize - x - 1;
            y = LongSize - y - 1;
        }
        return _pieces[x, y];
    }

    public SquareId SquareIdAt(int x, int y, bool boardIsRotated) {
        if (boardIsRotated) {
            x = LongSize - x - 1;
            y = LongSize - y - 1;
        }
        return SquareId.FromPosition(x, y, BoardType);
    }

    public (SquareId id, int cssX, int cssY) FirstSquareIdOfRow(int row, bool boardIsRotated) {
        var (x, cssX, cssY) = BoardType.AnnotationCoordinatesForRow(row);
        return (SquareIdAt(x, row, boardIsRotated), cssX, cssY);
    }

    public (SquareId id, int cssX, int cssY) LastSquareIdOfCol(int col, bool boardIsRotated) {
        var (y, cssX, cssY) = BoardType.AnnotationCoordinatesForCol(col);
        return (SquareIdAt(col, y, boardIsRotated), cssX, cssY);
    }

    public (int x, int y) CssCoordinatesFor(int x, int y) => BoardType.CssCoordinatesFor(x, y);
}
