using Microsoft.AspNetCore.Html;

namespace Draughts.Application.Shared;

public sealed class ReferenceTooltips {
    private int _counter = 0;

    public HtmlString Print(HtmlString text) => Print(text.ToString());

    public HtmlString Print(string text) {
        _counter++;

        string result = $"<span class=\"ref\">";
        result += "<a href=\"#\" class=\"refnum show-hide-link\" "
            + $"data-id=\"refbody-{_counter}\" data-class=\"refbody\">[{_counter}]</a>";
        result += $"<span class=\"refbody hide-on-click-outside\" id=\"refbody-{_counter}\">{text}</span>";
        result += "</span>";
        return new HtmlString(result);
    }
}
