using System;
using System.Text;
using Konscious.Security.Cryptography;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public readonly struct PasswordHash {
        private static byte[] Pepper = Encoding.UTF8.GetBytes("PlGFw266BWmA1i5vGg9XWrGsFaUqUI");

        private byte[] Hash { get; }

        private PasswordHash(byte[] hash) => Hash = hash;

        public bool CanLogin(string? plaintextPassword, AuthUserId userId, string username) {
            if (plaintextPassword is null) {
                return false;
            }

            bool result = true;
            var generatedHash = Generate(plaintextPassword, userId, username).Hash;
            for (int i = 0; i < generatedHash.Length; i++) {
                if (generatedHash[i] != Hash[i]) {
                    result = false;
                }
            }
            return result;
        }

        public static PasswordHash Generate(string plaintextPassword, AuthUserId userId, string username) {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(plaintextPassword);
            byte[] saltBytes = Encoding.UTF8.GetBytes(username);
            byte[] userIdBytes = BitConverter.GetBytes(userId.Id);

            var argon2 = new Argon2id(passwordBytes) {
                DegreeOfParallelism = 8,
                MemorySize = 512,
                Iterations = 8,
                Salt = saltBytes,
                AssociatedData = userIdBytes,
                KnownSecret = Pepper
            };

            return new PasswordHash(argon2.GetBytes(128));
        }
    }
}
