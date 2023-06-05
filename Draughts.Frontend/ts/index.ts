import * as $ from 'jquery'

import { initShowHideLinks } from "./site";
import { initCreateGame } from "./creategame";
import { initPlayGame } from "./playgame";

$(function () {
    // These functions are called on every page load
    initShowHideLinks();
});

export default {
    // These functions should be called manually in their respective pages
    initializeCreateGame: () => initCreateGame(),
    initializePlayGame: () => initPlayGame()
}
