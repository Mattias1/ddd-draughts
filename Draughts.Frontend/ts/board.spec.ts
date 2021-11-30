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
});
