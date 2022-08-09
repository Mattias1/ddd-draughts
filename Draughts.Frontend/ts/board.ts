export class Board {
    private _squares: Piece[];
    public size: number;

    get nrSquares(): number {
        return this._squares.length;
    }

    constructor(size: number, squares: Piece[]) {
        this.size = size;
        this._squares = squares;
    }

    public at(i: number): Piece {
        return this._squares[i - 1];
    }
    public atPosition(x: number, y: number): Piece|null {
        if (!this.isPlayable(x, y)) {
            return null;
        }
        return this.at(Math.floor((x + 2 + y * this.size) / 2));
    }

    public isPlayable(x: number, y: number): boolean {
        return (x + y) % 2 == 1;
    }

    public toString(): string {
        return this.toLongString("", "");
    }
    public toLongString(separator = "\n", empty = " "): string {
        let result = '';
        for (let y = 0; y < this.size; y++) {
            for (let x = 0; x < this.size; x++) {
                result += this.atPosition(x, y)?.toChar() ?? empty;
            }
            if (y != this.size - 1) {
                result += separator;
            }
        }
        return result;
    }

    public static fromString(boardString: string): Board {
        let pieces = Array.from(boardString)
            .map(c => parseInt('0x0' + c, 16))
            .filter(i => !isNaN(i))
            .map(i => new Piece(i));
        let size = Math.sqrt(pieces.length * 2);
        return new Board(size, pieces);
    }
}

export class Piece {
    private value: number;

    constructor(value: number) {
        this.value = value;
    }

    public isEmpty(): boolean {
        return this.value === 0;
    }
    public isNotEmpty(): boolean {
        return this.value !== 0;
    }
    public isMan(): boolean {
        return (this.value & 0b110) === 0b100;
    }
    public isKing(): boolean {
        return (this.value & 0b0010) === 0b0010;
    }
    public isAlive(): boolean {
        return this.value < 0b1000;
    }
    public isDead(): boolean {
        return this.value >= 0b1000;
    }

    public getColor(): string|null {
        return this.isEmpty() ? null : (this.value & 0b001) === 0 ? 'black' : 'white';
    }

    public toChar(): string {
        return this.value.toString();
    }
    public toString(): string {
        if (this.isEmpty()) {
            return 'empty';
        }
        return this.getColor() + (this.isMan() ? ' man' : ' king');
    }
}
