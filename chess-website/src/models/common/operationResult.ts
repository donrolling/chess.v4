namespace models {
    export interface operationResult {
        failure: boolean;
        message: string;
        status: status;
        success: boolean;
    }
}