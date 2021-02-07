using Draughts.Common;
using Draughts.Common.OoConcepts;
using Draughts.Common.Utilities;
using System.Collections.Generic;

namespace Draughts.Domain.GameAggregate.Models {
    public class Color : ValueObject<Color> {
        private bool _isWhite;

        private Color(bool isWhite) {
            _isWhite = isWhite;
        }

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return _isWhite;
        }

        public override string ToString() => _isWhite ? "white" : "black";

        public Color Other => new Color(!_isWhite);

        public static Color White => new Color(true);
        public static Color Black => new Color(false);
        public static Color Random => Rand.NextBool() ? White : Black;
        public static Color FromString(string color) => color switch {
            "white" => Color.White,
            "black" => Color.Black,
            _ => throw new ManualValidationException($"Unknown color: {color}")
        };
    }
}
