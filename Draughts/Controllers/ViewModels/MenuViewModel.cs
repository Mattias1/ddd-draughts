using System.Collections.Generic;
using System.Linq;

namespace Draughts.Controllers.ViewModels {
    public class MenuViewModel {
        public IReadOnlyList<(string name, string url)> Menu { get; }

        public MenuViewModel(params (string name, string url)[] menu) : this(menu.ToList().AsReadOnly()) { }
        public MenuViewModel(IReadOnlyList<(string name, string url)> menu) => Menu = menu;
    }
}
