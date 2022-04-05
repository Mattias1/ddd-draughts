using Draughts.Application.Shared.ViewModels;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using Flurl;
using Microsoft.AspNetCore.Html;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;

namespace Draughts.Application.Shared;

public static class Utils {
    /// <summary>
    /// The base url, 'http://localhost:52588' or the url in the settings depending on the environment
    /// </summary>
    public static string BaseUrl { get; set; } = "http://localhost:52588";

    /// <summary>
    /// Print 'hidden' or not
    /// </summary>
    public static HtmlString HideIf(bool isHidden) => new HtmlString(isHidden ? "hidden" : "");

    /// <summary>
    /// Print a href attribute, for example 'href="http://localhost:52588/the-url"'
    /// </summary>
    public static HtmlString Href(string url, params (string key, object? value)[] queryParams) {
        string safeUrl = Url(url, queryParams);
        return new HtmlString($"href=\"{safeUrl}\"");
    }

    /// <summary>
    /// Print the source attribute, for example 'src="http://localhost:52588/the-url"'
    /// </summary>
    public static HtmlString Src(string url, params (string key, object? value)[] queryParams) {
        string safeUrl = Url(url, queryParams);
        return new HtmlString($"src=\"{safeUrl}\"");
    }

    /// <summary>
    /// A url, for example 'http://localhost:52588/the-url'
    /// </summary>
    public static Url Url(string url, params (string key, object? value)[] queryParams) {
        Url fullUrl = url.StartsWith('/') ? Flurl.Url.Combine(BaseUrl, url) : new Url(url);
        foreach ((string key, object? value) in queryParams) {
            fullUrl.QueryParams.Add(key, value?.ToString() ?? "");
        }
        return fullUrl;
    }

    /// <summary>
    /// Encode a piece of text to protect against XSS attacks
    /// </summary>
    public static string E(string text) => HtmlEncoder.Default.Encode(text);

    /// <summary>
    /// Print a form formatted as link that sends a Post request
    /// </summary>
    public static HtmlString PostLink(string text, string url, params (string key, string value)[] parameters) {
        string s = $"<form class=\"post-link-form\" action=\"{Url(url)}\" method=\"post\">";
        foreach (var (key, value) in parameters) {
            s += $"<input type=\"hidden\" name=\"{E(key)}\" value=\"{E(value)}\">";
        }
        s += $"<input class=\"link\" type=\"submit\" value=\"{E(text)}\">";
        s += "</form>";
        return new HtmlString(s);
    }

    /// <summary>
    /// An uppercase game link, for example '<a href=...>Game 42</a>'
    /// </summary>
    public static HtmlString GameLinkU(GameViewModel game) => GameLinkU(game.Id);
    /// <summary>
    /// An uppercase game link, for example '<a href=...>Game 42</a>'
    /// </summary>
    public static HtmlString GameLinkU(GameId id) {
        return new HtmlString($"<a {Href("/game/" + id)}>Game {id}</a>");
    }

    /// <summary>
    /// A lowercase game link, for example '<a href=...>game 42</a>'
    /// </summary>
    public static HtmlString GameLinkL(GameViewModel game) => GameLinkL(game.Id);
    /// <summary>
    /// A lowercase game link, for example '<a href=...>game 42'</a>
    /// </summary>
    public static HtmlString GameLinkL(GameId id) {
        return new HtmlString($"<a {Href("/game/" + id)}>game {id}</a>");
    }

    /// <summary>
    /// A user link, for example '<a href=...>MyUsername</a>'
    /// </summary>
    public static HtmlString UserLink(PlayerViewModel player) => RawUserLink(player.UserId.Value, player.Username.Value);
    /// <summary>
    /// A user link, for example '<a href=...>MyUsername</a>'
    /// </summary>
    public static HtmlString UserLink(BasicUserViewModel user) => RawUserLink(user.Id.Value, user.Username.Value);
    /// <summary>
    /// A user link, for example '<a href=...>MyUsername</a>'
    /// </summary>
    public static HtmlString UserLink(UserId id, Username name) => RawUserLink(id.Value, name.Value);
    /// <summary>
    /// A user link with rank name, for example '<a href=...>Captain MyUsername</a>'
    /// </summary>
    public static HtmlString UserLinkWithRank(PlayerViewModel player) => RawUserLink(player.UserId.Value, $"{player.Rank.Name} {player.Username}");
    /// <summary>
    /// A user link with rank name, for example '<a href=...>Captain MyUsername</a>'
    /// </summary>
    public static HtmlString UserLinkWithRank(UserViewModel user) => RawUserLink(user.Id.Value, $"{user.Rank.Name} {user.Username}");
    private static HtmlString RawUserLink(long id, string name) {
        return new HtmlString($"<a class=\"user-a\" {Href("/user/" + id)}>{E(name)}</a>");
    }

    /// <summary>
    /// Print a side menu
    /// </summary>
    public static HtmlString SideMenu(MenuViewModel menuViewModel) {
        string s = $"<h4>{menuViewModel.Title}</h4><ul>";
        foreach (var (name, url) in menuViewModel.Menu) {
            s += $"<li><a {Href(url)}>{E(name)}</a></li>";
        }
        s += "</ul>";
        return new HtmlString(s);
    }

    /// <summary>
    /// Returns whether or not the user has the permission
    /// </summary>
    public static bool Can(IReadOnlyList<Permission> permissions, Permission permission) => permissions.Contains(permission);

    /// <summary>
    /// DateTime to display, for example '29 Feb 2021, 07:42'
    /// </summary>
    public static string DateTime(ZonedDateTime? datetime) {
        return datetime?.ToString("dd MMM yyyy, HH:mm", CultureInfo.InvariantCulture) ?? "";
    }
    /// <summary>
    /// DateTime in ISO format, for example '2021-02-29T07:13:37Z'
    /// </summary>
    public static HtmlString DateTimeIso(ZonedDateTime? datetime) => new HtmlString(datetime.ToIsoString());

    /// <summary>
    /// Print yes or no
    /// </summary>
    public static string YesNo(bool b) => b ? "Yes" : "No";

    /// <summary>
    /// Print the range description including the total amount for the pagination, for example '1-10 of 70'
    /// </summary>
    public static HtmlString PaginationRangeOfTotal<T>(IPaginationViewModel<T> model) {
        return new HtmlString(PaginationRange(model).Value + " of " + model.Pagination.Count);
    }
    /// <summary>
    /// Print the range description (without the total amount) for the pagination, for example '1-10'
    /// </summary>
    public static HtmlString PaginationRange<T>(IPaginationViewModel<T> model) {
        return new HtmlString($"{model.Pagination.BeginInclusive}-{model.Pagination.EndInclusive}");
    }

    /// <summary>
    /// Print the url list for the pagination, for example '< 1 2 ... 7 >'
    /// </summary>
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
