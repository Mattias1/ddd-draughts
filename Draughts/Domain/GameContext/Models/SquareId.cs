using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameContext.Models {
    public class SquareId : IntValueObject<SquareId> {
        public override int Value { get; }

        public SquareId(int? value) {
            if (value is null || value < 1 || value > byte.MaxValue) {
                throw new ManualValidationException("Invalid square id.");
            }
            Value = value.Value;
        }

        public (int x, int y) ToPosition(int size) {
            int y = (Value - 1) * 2 / size;
            int x = (Value * 2 - 1) % size - y % 2;
            return (x, y);
        }

        public static SquareId FromPosition(int x, int y, int size) {
            if (!Board.IsPlayable(x, y)) {
                throw new ManualValidationException($"This position ({x}, {y}) is not playable.");
            }
            return new SquareId((x + 2 + y * size) / 2);
        }
    }
}
