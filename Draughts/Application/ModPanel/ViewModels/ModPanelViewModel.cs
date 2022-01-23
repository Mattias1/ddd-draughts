using Draughts.Application.Shared.ViewModels;

namespace Draughts.Application.ModPanel.ViewModels;

public class ModPanelViewModel {
    public MenuViewModel Menu { get; }

    public ModPanelViewModel(MenuViewModel menuViewModel) {
        Menu = menuViewModel;
    }
}
