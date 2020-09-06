using System.Collections.Generic;
using System.Linq;

namespace Draughts.Controllers.Shared.ViewModels {
    public class MenuViewModel {
        public string Title { get; }
        public IReadOnlyList<(string name, string url)> Menu { get; }

        public MenuViewModel(string title, params (string name, string url)[] menu) : this(title, menu.ToList().AsReadOnly()) { }
        public MenuViewModel(string title, IReadOnlyList<(string name, string url)> menu) {
            Title = title;
            Menu = menu;
        }
    }
}
