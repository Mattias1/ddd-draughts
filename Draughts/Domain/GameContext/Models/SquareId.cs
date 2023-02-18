using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameContext.Models;

public sealed class SquareId : IntValueObject<SquareId> {
    public override int Value { get; }

    public SquareId(int? value) {
        if (value is null || value < 1 || value > byte.MaxValue) {
            throw new ManualValidationException("Invalid square id.");
        }
        Value = value.Value;
    }

    public (int x, int y) ToPosition(IBoardType boardType) => boardType.CoordinateFor(this);

    public static SquareId FromPosition(int x, int y, IBoardType boardType) => boardType.SquareIdFor(x, y);
}
