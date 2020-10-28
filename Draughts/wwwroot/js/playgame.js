$(document).ready(function () {
    var selectedSquare = null;
    $('#game-board div.square').click(function () {
        onSquareClick($(this));
    });

    function onSquareClick(squareEl) {
        let squareNumber = squareEl.data('number');
        if (selectedSquare) {
            let gameId = squareEl.data('gameid');

            console.log('Move from', selectedSquare, 'to', squareNumber);

            $.ajax({
                type: 'POST',
                contentType: 'application/json',
                // dataType: 'json', // It's not returning anything.
                url: '/game/' + gameId + '/move',
                data: JSON.stringify({
                    from: selectedSquare,
                    to: squareNumber
                }),
                success: function (data) {
                    location.reload(true);
                },
                error: function (jqXHR, errorText, textStatus) {
                    console.log('Error moving from ' + selectedSquare + ' to ' + squareNumber, jqXHR, textStatus, errorText);
                    location.reload(true);
                }
            });
        }
        else {
            console.log('Select square', squareNumber);
            selectedSquare = squareNumber;
            squareEl.addClass('selected');
        }
    }
});
