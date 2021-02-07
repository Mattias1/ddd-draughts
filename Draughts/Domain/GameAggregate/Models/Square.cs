using Draughts.Common;
using Draughts.Common.OoConcepts;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Draughts.Domain.GameAggregate.Models {
    public class Square : IntValueObject<Square> {
        public override int Value { get; }

        public Square(int? value) {
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

        public bool TryGetBorder(Direction direction, int size, [NotNullWhen(returnValue: true)] out Square? result) {
            var (x, y) = ToPosition(size);
            if (x <= 0 && direction.DX < 0
                    || x >= size - 1 && direction.DX > 0
                    || y <= 0 && direction.DY < 0
                    || y >= size - 1 && direction.DY > 0) {
                result = null;
                return false;
            }
            result = FromPosition(x + direction.DX, y + direction.DY, size);
            return true;
        }

        public static implicit operator int(Square square) => square.Value;
        public static implicit operator string(Square square) => square.ToString();

        public static Square FromPosition(int x, int y, int size) {
            if (!BoardPosition.IsPlayable(x, y)) {
                throw new ManualValidationException($"This position ({x}, {y}) is not playable.");
            }
            return new Square((x + 2 + y * size) / 2);
        }
    }
}
