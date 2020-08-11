using Draughts.Common.OoConcepts;
using System;
using System.Collections.Generic;

namespace Draughts.Domain.UserAggregate.Models {
    public class Rank : ValueObject<Rank>, IComparable<Rank> {
        public int Order { get; }
        public string Name { get; }
        public Rating RequiredRating { get; }
        public bool IsOfficerRank => Order >= Ranks.SecondLieutenant.Order;
        public int Icon => Order;

        private Rank(int order, string name, int requiredRating) {
            Order = order;
            Name = name;
            RequiredRating = new Rating(requiredRating);
        }

        public int CompareTo(Rank? other) => Order.CompareTo(other?.Order);

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Order;
            yield return Name.ToLower();
        }

        public static class Ranks {
            public static Rank Private => new Rank(1, "Private", 0);
            public static Rank LanceCorporal => new Rank(2, "Lance corporal", 500);
            public static Rank Corporal => new Rank(3, "Corporal", 1100);
            public static Rank Sergeant => new Rank(4, "Sergeant", 1250);
            public static Rank SergeantMajor => new Rank(5, "Sergeant major", 1400);
            public static Rank WarrantOfficer => new Rank(6, "Warrant officer", 1600);
            public static Rank ChiefWarrantOfficer => new Rank(7, "Chief warrant officer", 1800);
            public static Rank SecondLieutenant => new Rank(8, "2nd lieutenant", 2000);
            public static Rank Lieutenant => new Rank(9, "Lieutenant", 2200);
            public static Rank Captain => new Rank(10, "Captain", 2450);
            public static Rank Major => new Rank(11, "Major", 2700);
            public static Rank LieutenantColonel => new Rank(12, "Lieutenant colonel", 3000);
            public static Rank Colonel => new Rank(13, "Colonel", 3300);
            public static Rank BrigadierGeneral => new Rank(14, "Brigadier general", 3600);
            public static Rank MajorGeneral => new Rank(15, "Major general", 3900);
            public static Rank General => new Rank(16, "General", 4200);
            public static Rank FieldMarshal => new Rank(17, "Field marshal", 4500);

            public static IReadOnlyList<Rank> All => new List<Rank> {
                Private, LanceCorporal, Corporal, Sergeant, SergeantMajor, WarrantOfficer, ChiefWarrantOfficer,
                SecondLieutenant, Lieutenant, Captain, Major, LieutenantColonel, Colonel,
                BrigadierGeneral, MajorGeneral, General, FieldMarshal
            }.AsReadOnly();
        }
    }
}
