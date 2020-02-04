class Index {
    private readonly FENInputId: string = "#FEN";
    private FENInput: string;

    constructor() {
        this.FENInput = (document.querySelector(this.FENInputId) as HTMLInputElement).value;
        alert(this.FENInput);
    }

}