$(function () {
    $('#cgp-presets').on('change', function () {
        applyPresets($(this).val());
    });
});

function applyPresets(preset) {
    if (preset === 'international') {
        val('boardSize', 10);
        val('whiteHasFirstMove', true);
        val('flyingKings', true);
        val('menCaptureBackwards', true);
        val('captureConstraints', 'max');
    }
    else if (preset === 'english-draughts') {
        val('boardSize', 8);
        val('whiteHasFirstMove', false);
        val('flyingKings', false);
        val('menCaptureBackwards', false);
        val('captureConstraints', 'seq');
    }
    else if (preset === 'mini') {
        applyPresets('international');
        val('boardSize', 6);
    }
}

function val(name, value) {
    $('input[name="' + name + '"][value="' + value + '"]').prop('checked', 'checked');
}
