using Draughts.Domain.GameContext.Models;

namespace Draughts.Application.Shared.ViewModels;

public class BoardViewModel {
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
}
