import * as $ from 'jquery';
import * as dayjs from 'dayjs';
import * as signalR from '@microsoft/signalr';
import { Board } from './board';

let selectedSquare: number|null = null;
let isCaptureSequenceSquare: boolean = false;
let websocketConnection: signalR.HubConnection|null = null;
let timerIntervalId: number|null = null;

export function initPlaygame(): void {
    let gameId = $('#game-id').data('gameId');
    let captureSequenceFrom = $('#captureSequenceFrom').data('val');
    if (captureSequenceFrom) {
        selectSquare(captureSequenceFrom);
    }

    updateTimer($('#currentTime').data('val'), $('#turnExpiryTime').data('val'));

    $('#game-board div.square').on('click', function () {
        onSquareClick($(this));
    });

    websocketConnection = new signalR.HubConnectionBuilder()
        .withUrl("/hub")
        .configureLogging(signalR.LogLevel.Information)
        .build();
    websocketConnection.on("gameUpdateReady", (gameId: string) => {
        fetchAndUpdateGameData(gameId);
    });
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
            dataType: 'json',
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

function fetchAndUpdateGameData(gameId: string): void {
    $.ajax({
        type: 'GET',
        contentType: 'application/json',
        dataType: 'json',
        url: '/game/' + gameId + '/json',
        success: function (data: GameDto) {
            updateGameData(data);
        },
        error: function (jqXHR, errorText, textStatus) {
            console.error('Error fetching game data for game ' + gameId, jqXHR, textStatus, errorText);
        }
    });
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

        updateTimer(data.turn.currentTime, data.turn.expiresAt);
    }
}

function updateTimer(currentTimeStr: string, expiresAtStr: string): void {
    if (expiresAtStr === null || currentTimeStr === null) {
        $('#turn-time-left').text('88:88:88');
        return;
    }

    const now = dayjs();
    const currentTime = dayjs(currentTimeStr);
    const expiresAt = dayjs(expiresAtStr);
    if (expiresAt.isBefore(currentTime)) {
        $('#turn-time-left').text('00:00:00');
        return;
    }

    const offsetWithLocalTimeInMs = now.diff(currentTime);
    if (offsetWithLocalTimeInMs > 2000) {
        $('#turn-time-left-offset').text('±' + Math.ceil(Math.abs(offsetWithLocalTimeInMs) / 1000).toString() + 's');
    }
    else {
        $('#turn-time-left-offset').text('');
    }

    updateTimeLeftHtml(currentTime, expiresAt);

    if (timerIntervalId !== null) {
        window.clearInterval(timerIntervalId);
    }
    timerIntervalId = window.setInterval(() => {
        const now = dayjs().subtract(offsetWithLocalTimeInMs, 'milliseconds');
        updateTimeLeftHtml(now, expiresAt);
    }, 1000);
}

function updateTimeLeftHtml(currentTime: dayjs.Dayjs, expiresAt: dayjs.Dayjs): void {
    // Subtract a second, because it's better to guess too low than too high.
    let diffInSeconds = Math.floor(expiresAt.diff(currentTime, 'seconds')) - 1;
    if (diffInSeconds < 0) {
        $('#turn-time-left').text('00:00:00');
        return;
    }
    let diffInMinutes = Math.floor(diffInSeconds / 60);
    const diffInHours = Math.floor(diffInMinutes / 60);
    diffInSeconds -= diffInMinutes * 60;
    diffInMinutes -= diffInHours * 60;

    $('#turn-time-left').text(
        diffInHours.toString().padStart(2, '0') + ':'
        + diffInMinutes.toString().padStart(2, '0') + ':'
        + diffInSeconds.toString().padStart(2, '0')
    );
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
    currentTime: string;
    expiresAt: string;
}
