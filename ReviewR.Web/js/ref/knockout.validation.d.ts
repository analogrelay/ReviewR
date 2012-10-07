/// <reference path="knockout.d.ts" />
declare module Knockout {
    export declare interface ValidatableViewModel {
        customError: Knockout.Observable;
        isValid: Knockout.Observable;
        errorMessage: Knockout.Observable;
        hasMessage: Knockout.Observable;
        validate(): void;
    }

    export declare interface Observable {
        required(message: string): Observable;
        equalTo(message: string, other: string): Observable;
        mustBe(message: string, value: any): Observable;
        regex(message: string, pattern: RegExp): Observable;
    }

    export declare interface KnockoutObject {
        validation: ValidationObject;
    }

    export declare interface ValidationObject {
        addValidation(vm: any): ValidatableViewModel;
    }
}

declare var ko: Knockout.KnockoutObject;