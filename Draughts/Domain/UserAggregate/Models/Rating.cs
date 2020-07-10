using Draughts.Common;
using System;
using System.Collections.Generic;

namespace Draughts.Domain.UserAggregate.Models {
    public class Rating : ValueObject<Rating>, IComparable<Rating> {
        public static Rating StartRating = new Rating(1000);

        public int Value { get; }

        public Rating(int value) => Value = value;

        public static implicit operator int(Rating rating) => rating.Value;

        public override string ToString() => Value.ToString();

        public int CompareTo(Rating other) => Value.CompareTo(other.Value);

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Value;
        }
    }
}
