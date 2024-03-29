using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Draughts.Common;

public sealed class JsonWebToken {
    private static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };
    private static readonly byte[] SECRET = Convert.FromBase64String("SvR9OUqzB23f3j3v0KfRlgJjGEcNLyCYDCf4Hougaz0=");
    public const int EXPIRATION_SECONDS = 24 * 60 * 60;

    private string? _cachedJwtString;
    private readonly JwtData _data;

    public UserId UserId => new UserId(_data.Usr);
    public Username Username => new Username(_data.Una);
    public IReadOnlyList<RoleId> RoleIds => _data.Rol.Select(r => new RoleId(r)).ToList().AsReadOnly();

    private JsonWebToken(JwtData data) => _data = data;

    public string ToJwtString() {
        if (_cachedJwtString is not null) {
            return _cachedJwtString;
        }

        string header = ToBase64Json(new JwtHeader());
        string data = ToBase64Json(_data);
        string signature = ComputeSignature(header, data);

        _cachedJwtString = $"{header}.{data}.{signature}";
        return _cachedJwtString;
    }

    public static bool TryParseFromJwtString(string jwtString, IClock clock, [NotNullWhen(returnValue: true)] out JsonWebToken? jwt) {
        var parts = jwtString.Split('.');
        if (parts.Length != 3) {
            jwt = null;
            return false;
        }
        var header = FromBase64Json<JwtHeader>(parts[0]);
        var data = FromBase64Json<JwtData>(parts[1]);

        if (header?.Alg != "HS256" || header.Typ != "JWT") {
            jwt = null;
            return false;
        }

        long unixNow = clock.GetCurrentInstant().ToUnixTimeSeconds();
        string expectedSignature = ComputeSignature(parts[0], parts[1]);
        // Note: This is not secure - it's home made crypto, what did you expect?
        if (data?.Aud != "Draughts" || data.Exp < unixNow || data.Usr == default || parts[2] != expectedSignature) {
            jwt = null;
            return false;
        }

        jwt = new JsonWebToken(data);
        return true;
    }

    private static string ComputeSignature(string header, string data) {
        using var hmacSha = new HMACSHA256(SECRET);
        byte[] signatureBytes = hmacSha.ComputeHash(Encoding.UTF8.GetBytes($"{header}.{data}"));
        return ToBase64(signatureBytes);
    }

    private static string ToBase64Json<T>(T part)
        => ToBase64(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(part, JSON_SETTINGS)));
    private static string ToBase64(byte[] bytes) {
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').Replace("=", "");
    }

    private static T? FromBase64Json<T>(string raw) {
        byte[] bytes = FromBase64(raw);
        string value = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<T>(value);
    }

    private static byte[] FromBase64(string raw) {
        string padding = new string('=', (4 - raw.Length % 4) % 4);
        string base64 = raw.Replace('-', '+').Replace('_', '/') + padding;
        return Convert.FromBase64String(base64);
    }

    public static JsonWebToken Generate(AuthUser authUser, IClock clock) {
        Instant expires = clock.GetCurrentInstant().Plus(Duration.FromSeconds(EXPIRATION_SECONDS));
        return new JsonWebToken(new JwtData(authUser, expires));
    }

    public sealed class JwtHeader {
        public string Alg { get; set; } = "HS256";
        public string Typ { get; set; } = "JWT";
    }

    private sealed class JwtData {
        public string Aud { get; set; }
        public long Exp { get; set; }
        public long Usr { get; set; }
        public string Una { get; set; }
        public long[] Rol { get; set; }

        public JwtData() {
            Aud = "Unknown";
            Una = "";
            Rol = new long[0];
        }

        public JwtData(AuthUser authUser, Instant expires) {
            Aud = "Draughts";
            Exp = expires.ToUnixTimeSeconds();
            Usr = authUser.Id.Value;
            Una = authUser.Username.Value;
            Rol = authUser.RoleIds.Select(r => r.Value).ToArray();
        }
    }
}
