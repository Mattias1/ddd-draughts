import * as $ from 'jquery'

export function initCreateGame(): void {
    $('#cgp-presets').on('change', function () {
        let preset = $(this).val()?.toString() ?? '';
        applyPresets(preset);
    });
}

function applyPresets(preset: string): void {
    if (preset === 'international') {
        val('boardSize', 10);
        val('whiteHasFirstMove', true);
        val('flyingKings', true);
        val('menCaptureBackwards', true);
        val('captureConstraints', 'max');
    } else if (preset === 'english-draughts') {
        val('boardSize', 8);
        val('whiteHasFirstMove', false);
        val('flyingKings', false);
        val('menCaptureBackwards', false);
        val('captureConstraints', 'seq');
    } else if (preset === 'mini') {
        applyPresets('international');
        val('boardSize', 6);
    } else if (preset == 'hexdame') {
        applyPresets('international');
        val('boardSize', 5);
    } else if (preset == 'minihex') {
        applyPresets('international');
        val('boardSize', 3);
    }
}

function val(name: string, value: string|number|boolean): void {
    $('input[name="' + name + '"][value="' + value + '"]').prop('checked', 'checked');
}
