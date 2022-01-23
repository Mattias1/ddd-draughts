using Draughts.Common.OoConcepts;

namespace Draughts.Domain.UserContext.Models;

public class Rating : IntValueObject<Rating> {
    public static Rating StartRating = new Rating(1000);

    public override int Value { get; }

    public Rating(int value) => Value = value;
}
