import { Result } from "./result";

export interface TResult<T> extends Result{
    value: T;
}