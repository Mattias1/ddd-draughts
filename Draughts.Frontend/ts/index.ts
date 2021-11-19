import * as $ from 'jquery'

import { initializeShowHideLinks } from "./site";
import { initCreateGame } from "./creategame";
import { initPlaygame } from "./playgame";

$(function () {
    initializeShowHideLinks();
    initCreateGame();
    initPlaygame();
});
