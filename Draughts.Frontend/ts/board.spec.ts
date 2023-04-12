import { Board } from "./board";

describe("Board.fromString", () => {
    it("initialises the default board well", () => {
        expect(Board.fromString("44000055").toString()).toBe("44000055");
    });

    it("understands fancy boards", () => {
        expect(Board.fromString(
            " 4 4\n" +
            "0 0 \n" +
            " 0 0\n" +
            "5 5 "
        ).toLongString(",", "_")).toBe("_4_4,0_0_,_0_0,5_5_");
    });

    it("handles all pieces", () => {
        // 4: Black pawn        6: Black king
        // C: Dead black pawn   E: Dead black king
        // F: Dead white king   D: Dead white pawn
        // 7: White king        5: White pawn
        expect(Board.fromString(" 4 6,C E , F D,7 5 ").toLongString(",", " ")).toBe(" 4 6,C E , F D,7 5 ");
    });
});
