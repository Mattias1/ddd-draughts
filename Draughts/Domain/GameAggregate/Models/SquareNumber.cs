using Draughts.Common;
using Draughts.Common.OoConcepts;

namespace Draughts.Domain.GameAggregate.Models {
    public class SquareNumber : IntValueObject<SquareNumber> {
        public override int Value { get; }

        public SquareNumber(int? value) {
            if (value is null || value < 1) {
                throw new ManualValidationException("Invalid square number.");
            }
            Value = value.Value;
        }

        public (int x, int y) ToPosition(int size) {
            int y = (Value - 1) * 2 / size;
            int x = (Value * 2 - 1) % size - y % 2;
            return (x, y);
        }

        public static implicit operator int(SquareNumber squareNumber) => squareNumber.Value;
        public static implicit operator string(SquareNumber squareNumber) => squareNumber.ToString();

        public static SquareNumber FromPosition(int x, int y, int size) {
            if (!BoardPosition.IsPlayable(x, y)) {
                throw new ManualValidationException($"This position ({x}, {y}) is not playable.");
            }
            return new SquareNumber((x + 2 + y * size) / 2);
        }
    }
}
