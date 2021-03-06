var selectedSquare = null;

$(function () {
    let captureSequenceFrom = $('#captureSequenceFrom').data('val');
    if (captureSequenceFrom) {
        selectSquare(captureSequenceFrom, $('#square-' + captureSequenceFrom));
    }

    $('#game-board div.square').on('click', function () {
        onSquareClick($(this));
    });
});

function onSquareClick(squareEl) {
    let square = squareEl.data('number');
    if (selectedSquare) {
        let gameId = squareEl.data('gameid');

        console.log('Move from', selectedSquare, 'to', square);

        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            // dataType: 'json', // It's not returning anything.
            url: '/game/' + gameId + '/move',
            data: JSON.stringify({
                from: selectedSquare,
                to: square
            }),
            success: function (data) {
                location.reload();
            },
            error: function (jqXHR, errorText, textStatus) {
                console.log('Error moving from ' + selectedSquare + ' to ' + square, jqXHR, textStatus, errorText);
                location.reload();
            }
        });
    }
    else {
        selectSquare(square, squareEl);
    }
}

function selectSquare(square, squareEl) {
    console.log('Select square', square);
    selectedSquare = square;
    squareEl.addClass('selected');
}
