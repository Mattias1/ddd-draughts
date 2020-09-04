using Draughts.Common.OoConcepts;
using System.Collections.Generic;

namespace Draughts.Domain.GameAggregate.Models {
    public class Color : ValueObject<Color> {
        private bool _isBlack;

        private Color(bool isBlack) {
            _isBlack = isBlack;
        }

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return _isBlack;
        }

        public override string ToString() => _isBlack ? "black" : "white";

        public static Color Black => new Color(true);
        public static Color White => new Color(false);
    }
}
