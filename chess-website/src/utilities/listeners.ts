namespace utilities {
    export class eventlisteners {
        public static addEventListeners(selector: string, event: string, handler: EventListener): void {
            let items = (document.querySelectorAll(selector) as any) as Array<HTMLElement>;
            if (items) {
                for (const item of items) {
                    item.addEventListener(event, handler);
                }
            }
        }
    
        public static removeEventListeners(selector: string, event: string, handler: EventListener): void {
            let items = (document.querySelectorAll(selector) as any) as Array<HTMLElement>;
            if (items) {
                for (const item of items) {
                    item.removeEventListener(event, handler);
                }
            }
        }
    }
}