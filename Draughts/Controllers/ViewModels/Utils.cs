using System.Linq;
using System.Text.Encodings.Web;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Microsoft.AspNetCore.Html;

namespace Draughts.Controllers.ViewModels {
    public static class Utils {
        public static string E(string text) => HtmlEncoder.Default.Encode(text).ToString();
        public static string UrlE(string url)
            => string.Join('/', url.Split('/').Select(text => UrlEncoder.Default.Encode(text).ToString()));

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
            string s = "<ul>";
            foreach (var (name, url) in menuViewModel.Menu) {
                s += $"<li><a href=\"{UrlE(url)}\">{E(name)}</a></li>";
            }
            s += "</ul>";
            return new HtmlString(s);
        }
    }
}
