using Draughts.Application.Shared.ViewModels;

namespace Draughts.Application.ModPanel.ViewModels {
    public class ModpanelViewModel {
        public MenuViewModel Menu { get; }

        public ModpanelViewModel(MenuViewModel menuViewModel) {
            Menu = menuViewModel;
        }
    }
}
