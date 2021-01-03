import { status } from "../enums/status";

export interface operationResult<T> {
    result: T;
    failure: boolean;
    message: string;
    status: status;
    success: boolean;
}