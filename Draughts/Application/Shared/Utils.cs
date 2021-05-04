using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using Flurl;
using Microsoft.AspNetCore.Html;
using NodaTime;

namespace Draughts.Application.Shared {
    public static class Utils {
        public static string BaseUrl { get; set; } = "http://localhost:52588";

        public static HtmlString Href(string url, params (string key, object? value)[] queryParams) {
            Url fullUrl = Url(url);
            foreach ((string key, object? value) in queryParams) {
                fullUrl.QueryParams.Add(E(key), E(value?.ToString() ?? ""));
            }
            return new HtmlString($"href=\"{E(fullUrl)}\"");
        }
        public static string UrlE(string url) => E(Url(url));
        public static Url Url(string url) => url.StartsWith('/') ? Flurl.Url.Combine(BaseUrl, url) : url;
        public static string E(string text) => HtmlEncoder.Default.Encode(text);

        public static HtmlString PostLink(string text, string url, params (string key, string value)[] parameters) {
            string s = $"<form class=\"post-link-form\" action=\"{UrlE(url)}\" method=\"post\">";
            foreach (var (key, value) in parameters) {
                s += $"<input type=\"hidden\" name=\"{E(key)}\" value=\"{E(value)}\">";
            }
            s += $"<input class=\"link\" type=\"submit\" value=\"{E(text)}\">";
            s += "</form>";
            return new HtmlString(s);
        }

        public static HtmlString GameLinkU(GameViewModel game) => GameLinkU(game.Id);
        public static HtmlString GameLinkU(GameId id) {
            return new HtmlString($"<a {Href("/game/" + id)}>Game {id}</a>");
        }

        public static HtmlString GameLinkL(GameViewModel game) => GameLinkL(game.Id);
        public static HtmlString GameLinkL(GameId id) {
            return new HtmlString($"<a {Href("/game/" + id)}>game {id}</a>");
        }

        public static HtmlString UserLink(PlayerViewModel player) => RawUserLink(player.UserId, player.Username);
        public static HtmlString UserLink(BasicUserViewModel user) => RawUserLink(user.Id, user.Username);
        public static HtmlString UserLink(UserId id, Username name) => RawUserLink(id, name);
        public static HtmlString UserLinkWithRank(PlayerViewModel player) => RawUserLink(player.UserId, $"{player.Rank.Name} {player.Username}");
        public static HtmlString UserLinkWithRank(UserViewModel user) => RawUserLink(user.Id, $"{user.Rank.Name} {user.Username}");
        private static HtmlString RawUserLink(long id, string name) {
            return new HtmlString($"<a class=\"user-a\" {Href("/user/" + id)}>{E(name)}</a>");
        }

        public static HtmlString SideMenu(MenuViewModel menuViewModel) {
            string s = $"<h4>{menuViewModel.Title}</h4><ul>";
            foreach (var (name, url) in menuViewModel.Menu) {
                s += $"<li><a {Href(url)}>{E(name)}</a></li>";
            }
            s += "</ul>";
            return new HtmlString(s);
        }

        public static bool Can(IReadOnlyList<Permission> permissions, Permission permission) => permissions.Contains(permission);

        public static string DateTime(ZonedDateTime? datetime) {
            return datetime?.ToString("dd MMM yyyy, HH:mm", CultureInfo.InvariantCulture) ?? "";
        }

        public static string YesNo(bool b) => b ? "Yes" : "No";

        public static HtmlString PaginationRangeOfTotal<T>(IPaginationViewModel<T> model) {
            return new HtmlString(PaginationRange(model).Value + " of " + model.Pagination.Count);
        }
        public static HtmlString PaginationRange<T>(IPaginationViewModel<T> model) {
            return new HtmlString($"{model.Pagination.BeginInclusive}-{model.Pagination.EndInclusive}");
        }

        public static HtmlString PaginationNav<T>(IPaginationViewModel<T> model, string url,
                int maxInitial = 2, int minAround = 3, int maxClosing = 2) {
            if (model.Pagination.PageCount <= 1) {
                return new HtmlString("");
            }

            long page = model.Pagination.Page;
            long pageCount = model.Pagination.PageCount;

            string s = "<nav class=\"pagination-nav\"><ul>";
            if (page == 1) {
                s += EmptyPaginationListItem("<");
            }
            else {
                s += PaginationListItem(url, page - 1, "<");
            }

            if (pageCount <= maxInitial + minAround * 2 + maxClosing + 3) {
                s += PaginationListItems(url, page, 1, pageCount);
            }
            else if (page <= maxInitial + minAround + 2) {
                s += PaginationListItems(url, page, 1, maxInitial + minAround * 2 + 2);
                s += EmptyPaginationListItem("...");
                s += PaginationListItems(url, page, pageCount - maxClosing + 1, pageCount);
            }
            else if (page > pageCount - maxClosing - minAround - 2) {
                s += PaginationListItems(url, page, 1, maxInitial);
                s += EmptyPaginationListItem("...");
                s += PaginationListItems(url, page, pageCount - maxClosing - minAround * 2 - 1, pageCount);
            }
            else {
                s += PaginationListItems(url, page, 1, maxInitial);
                s += EmptyPaginationListItem("...");
                s += PaginationListItems(url, page, page - minAround, page + minAround);
                s += EmptyPaginationListItem("...");
                s += PaginationListItems(url, page, pageCount - maxClosing + 1, pageCount);
            }

            if (page == pageCount) {
                s += EmptyPaginationListItem(">");
            }
            else {
                s += PaginationListItem(url, page + 1, ">");
            }
            s += "</ul></nav>";
            return new HtmlString(s);
        }

        private static string PaginationListItems(string url, long page, long startInclusive, long endInclusive) {
            string s = "";
            for (long i = startInclusive; i <= endInclusive; i++) {
                if (i == page) {
                    s += EmptyPaginationListItem(i.ToString(), "active");
                }
                else {
                    s += PaginationListItem(url, i, i.ToString());
                }
            }
            return s;
        }

        private static string PaginationListItem(string url, long page, string display) {
            return $"<li><a {Href(url, ("page", page))}>{E(display)}</a></li>";
        }

        private static string EmptyPaginationListItem(string display, string cssClass = "disabled") {
            return $"<li class=\"{cssClass}\"><span>{E(display)}</span></li>";
        }
    }
}
