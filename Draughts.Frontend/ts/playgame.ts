import * as $ from 'jquery'
import { Board } from './board';

let selectedSquare: number|null = null;

export function initPlaygame(): void {
    let captureSequenceFrom = $('#captureSequenceFrom').data('val');
    if (captureSequenceFrom) {
        selectSquare(captureSequenceFrom, $('#square-' + captureSequenceFrom));
    }

    $('#game-board div.square').on('click', function () {
        onSquareClick($(this));
    });
}

function onSquareClick(squareEl: JQuery<HTMLElement>): void {
    let square = squareEl.data('number');
    if (selectedSquare) {
        let gameId = squareEl.data('gameid');

        console.log('Move from', selectedSquare, 'to', square);

        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            dataType: 'json', // It's not returning anything.
            url: '/game/' + gameId + '/move',
            data: JSON.stringify({
                from: selectedSquare,
                to: square
            }),
            success: function (data: GameDto) {
                console.log('success start'); // DEBUG
                let board = Board.fromString(data.boardString);
                drawBoard(board);
                selectedSquare = data.captureSequenceFrom;
                // location.reload();
                console.log('success end'); // DEBUG
            },
            error: function (jqXHR, errorText, textStatus) {
                console.log('Error moving from ' + selectedSquare + ' to ' + square, jqXHR, textStatus, errorText);
                // location.reload();
            }
        });
    }
    else {
        selectSquare(square, squareEl);
    }
}

function selectSquare(square: number|null, squareEl: JQuery<HTMLElement>): void {
    console.log('Select square', square);
    selectedSquare = square;
    squareEl.addClass('selected');
}

function drawBoard(board: Board): void {
    console.log('size:', board.size, board.toString(), board.toLongString()); // DEBUG
    for (let i = 1; i <= board.nrSquares; i++) {
        let piece = board.at(i);
        console.log(`#square-${i}`, piece.toString()); // DEBUG
        let el = $(`#square-${i}`);
        el.removeClass('black white man king');
        if (piece.isNotEmpty()) {
            el.addClass(piece.getColor() ?? '');
            el.addClass(piece.isMan() ? 'man' : 'king');
        }
    }
}

interface GameDto {
    id: bigint;
    turn: TurnDto|null;
    gameEndedMessage: string;
    captureSequenceFrom: number|null;
    boardString: string;
}

interface TurnDto {
    playerId: bigint;
    expiresAt: string;
}
