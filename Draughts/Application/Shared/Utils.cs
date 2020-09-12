using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Microsoft.AspNetCore.Html;
using NodaTime;

namespace Draughts.Application.Shared {
    public static class Utils {
        public static string E(string text) => HtmlEncoder.Default.Encode(text);
        public static string UrlE(string url)
            => string.Join('/', url.Split('/').Select(text => UrlEncoder.Default.Encode(text)));

        public static HtmlString PostLink(string text, string url, params (string key, string value)[] parameters) {
            string s = $"<form class=\"post-link-form\" action=\"{UrlE(url)}\" method=\"post\">";
            foreach (var (key, value) in parameters) {
                s += $"<input type=\"hidden\" name=\"{E(key)}\" value=\"{E(value)}\">";
            }
            s += $"<input class=\"link\" type=\"submit\" value=\"{E(text)}\">";
            s += "</form>";
            return new HtmlString(s);
        }

        public static HtmlString UserLink(UserViewModel user) => RawUserLink(user.Id, user.Username);
        public static HtmlString UserLink(UserId id, Username name) => RawUserLink(id, name);
        public static HtmlString UserLinkWithRank(UserViewModel user) => RawUserLink(user.Id, $"{user.Rank.Name} {user.Username}");
        private static HtmlString RawUserLink(long id, string name) {
            return new HtmlString($"<a class=\"user-a\" href=\"/user/{id}\">{E(name)}</a>");
        }

        public static HtmlString SideMenu(MenuViewModel menuViewModel) {
            string s = $"<h4>{menuViewModel.Title}</h4><ul>";
            foreach (var (name, url) in menuViewModel.Menu) {
                s += $"<li><a href=\"{UrlE(url)}\">{E(name)}</a></li>";
            }
            s += "</ul>";
            return new HtmlString(s);
        }

        public static bool Can(IReadOnlyList<Permission> permissions, Permission permission) => permissions.Contains(permission);

        public static string DateTime(ZonedDateTime datetime) {
            return datetime.ToString("dd MMM yyyy, HH:mm", CultureInfo.InvariantCulture);
        }

        public static string YesNo(bool b) => b ? "Yes" : "No";
    }
}
