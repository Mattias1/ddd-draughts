import * as $ from 'jquery'
import * as signalR from "@microsoft/signalr";
import { Board } from './board';

let selectedSquare: number|null = null;
let isCaptureSequenceSquare: boolean = false;
let websocketConnection: signalR.HubConnection|null = null;

export function initPlaygame(): void {
    let gameId = $('#game-id').data('gameId');
    let captureSequenceFrom = $('#captureSequenceFrom').data('val');
    if (captureSequenceFrom) {
        selectSquare(captureSequenceFrom);
    }

    $('#game-board div.square').on('click', function () {
        onSquareClick($(this));
    });

    websocketConnection = new signalR.HubConnectionBuilder()
        .withUrl("/hub")
        .configureLogging(signalR.LogLevel.Information)
        .build();
    websocketConnection.on("gameUpdated", (data: GameDto) => {
        updateGameData(data);
    });
    websocketConnection.start()
        .then(_ => websocketConnection?.invoke("associateGame", gameId))
        .catch(err => console.error(err.toString()));
}

function onSquareClick(squareEl: JQuery<HTMLElement>): void {
    let square = squareEl.data('number');
    if (selectedSquare) {
        let gameId = squareEl.data('gameid');

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
                updateGameData(data);
            },
            error: function (jqXHR, errorText, textStatus) {
                console.error('Error moving from ' + selectedSquare + ' to ' + square, jqXHR, textStatus, errorText);
                if (!isCaptureSequenceSquare) {
                    selectSquare(null);
                }
            }
        });
    }
    else {
        selectSquare(square, squareEl);
    }
}

function updateGameData(data: GameDto): void {
    let board = Board.fromString(data.boardString);
    drawBoard(board);

    updateSidebar(data);

    selectSquare(data.captureSequenceFrom);
    isCaptureSequenceSquare = selectedSquare === null;

    if (data.gameEndedMessage != null && data.gameEndedMessage != "") {
        // Yeah, this is ugly but whatever.
        location.reload();
    }
}

function drawBoard(board: Board): void {
    for (let i = 1; i <= board.nrSquares; i++) {
        let piece = board.at(i);
        let el = $(`#square-${i}`);
        el.removeClass('black white man king');
        if (piece.isNotEmpty()) {
            el.addClass(piece.getColor() ?? '');
            el.addClass(piece.isMan() ? 'man' : 'king');
        }
    }
}

function updateSidebar(data: GameDto): void {
    $('.turn-player').hide();
    if (data.turn !== null) {
        $(`#turn-player-${data.turn?.playerId}`).css('display', 'inline');
    }
}

function selectSquare(square: number|null, squareEl: JQuery<HTMLElement>|null = null): void {
    $('.square.selected').removeClass('selected');
    selectedSquare = square;
    if (square !== null) {
        if (squareEl === null) {
            squareEl = $(`#square-${square}`);
        }
        squareEl.addClass('selected');
    }
}

interface GameDto {
    id: number;
    turn: TurnDto|null;
    gameEndedMessage: string;
    captureSequenceFrom: number|null;
    boardString: string;
}

interface TurnDto {
    playerId: number;
    expiresAt: string;
}
