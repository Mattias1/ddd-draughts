using Draughts.Domain.GameContext.Models;

namespace Draughts.Application.Shared.ViewModels;

public sealed class BoardViewModel {
    private readonly Piece?[,] _pieces;
    public int Size { get; }

    public BoardViewModel(Board board) {
        Size = board.Size;
        _pieces = new Piece[Size, Size];
        for (int y = 0; y < Size; y++) {
            for (int x = 0; x < Size; x++) {
                _pieces[x, y] = board[x, y]?.Piece;
            }
        }
    }

    public Piece? At(int x, int y, bool boardIsRotated) {
        if (boardIsRotated) {
            x = Size - x - 1;
            y = Size - y - 1;
        }
        return _pieces[x, y];
    }

    public SquareId SquareIdAt(int x, int y, bool boardIsRotated) {
        if (boardIsRotated) {
            x = Size - x - 1;
            y = Size - y - 1;
        }
        return SquareId.FromPosition(x, y, Size);
    }

    public SquareId FirstSquareIdOfRow(int row, bool boardIsRotated) {
        return Board.IsPlayable(0, row) ? SquareIdAt(0, row, boardIsRotated) : SquareIdAt(1, row, boardIsRotated);
    }

    public SquareId LastSquareIdOfCol(int col, bool boardIsRotated) {
        return Board.IsPlayable(col, Size - 1)
            ? SquareIdAt(col, Size - 1, boardIsRotated)
            : SquareIdAt(col, Size - 2, boardIsRotated);
    }
}
