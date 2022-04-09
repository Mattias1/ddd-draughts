using Draughts.Common.OoConcepts;
using System;
using System.Collections.Generic;

namespace Draughts.Domain.GameContext.Models;

public sealed class Piece : ValueObject<Piece> {
    public byte RawValue { get; }

    public Piece(byte raw) {
        if (raw > 0 && raw < 0b100 || raw > 0b111) {
            throw new InvalidOperationException($"Invalid square value ({raw})");
        }
        RawValue = raw;
    }

    public bool IsEmpty => RawValue == 0;
    public bool IsNotEmpty => RawValue != 0;
    public Color? Color => IsEmpty ? null : (RawValue & 0b001) == 0 ? Color.Black : Color.White;
    public bool IsMan => (RawValue & 0b110) == 0b100;
    public bool IsKing => RawValue >= 0b110;

    public char ToChar() => char.Parse(RawValue.ToString());

    public Piece Promoted() => new Piece((byte)(RawValue | 0b010));

    protected override IEnumerable<object> GetEqualityComponents() {
        yield return RawValue;
    }

    public static Piece Empty => new Piece(0b000);
    public static Piece BlackMan => new Piece(0b100);
    public static Piece WhiteMan => new Piece(0b101);
    public static Piece BlackKing => new Piece(0b110);
    public static Piece WhiteKing => new Piece(0b111);
}
