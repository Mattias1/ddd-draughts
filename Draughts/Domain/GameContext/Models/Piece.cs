using Draughts.Common.OoConcepts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.GameContext.Models;

public sealed class Piece : ValueObject<Piece> {
    public byte RawValue { get; }

    private Piece(byte raw) {
        if (raw > 0 && raw < 0b0100 || raw > 0b1111) {
            throw new InvalidOperationException($"Invalid square value ({raw})");
        }
        RawValue = raw;
    }

    public bool IsEmpty => RawValue == 0;
    public bool IsNotEmpty => RawValue != 0;
    public Color? Color => IsEmpty ? null : (RawValue & 0b0001) == 0 ? Color.Black : Color.White;
    public bool IsMan => (RawValue & 0b0110) == 0b0100;
    public bool IsKing => (RawValue & 0b0010) == 0b0010;
    public bool IsAlive => RawValue < 0b1000;
    public bool IsDead => RawValue >= 0b1000;

    public string CssClasses => $"{(IsDead ? "dead " : "")}{Color}{(IsMan ? " man" : "")}{(IsKing ? " king" : "")}";

    public string ToHexString() {
        string hex = Convert.ToHexString(new byte[] { RawValue });
        if (hex[0] != '0') {
            throw new InvalidOperationException("A piece should not have more than 4 bits of entropy.");
        }
        return hex.Substring(1);
    }

    public static Piece FromHexString(string hexChar) {
        return new Piece(Convert.FromHexString(hexChar.PadLeft(2, '0')).Single());
    }

    public Piece Promoted() => new Piece((byte)(RawValue | 0b0010));
    public Piece Killed() => new Piece((byte)(RawValue | 0b1000));

    protected override IEnumerable<object> GetEqualityComponents() {
        yield return RawValue;
    }

    public static Piece Empty => new Piece(0b0000);
    public static Piece BlackMan => new Piece(0b0100);
    public static Piece WhiteMan => new Piece(0b0101);
    public static Piece BlackKing => new Piece(0b0110);
    public static Piece WhiteKing => new Piece(0b0111);
}
