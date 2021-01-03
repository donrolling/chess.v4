export class arrays {
    public static removeItemFromArray(xs: any, x: any): void {
        let index = xs.indexOf(x);
        if (index > -1) {
            xs.splice(index, 1);
        }
    }
}