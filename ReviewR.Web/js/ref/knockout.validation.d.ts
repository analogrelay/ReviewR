/// <reference path="knockout.d.ts" />
declare module Knockout {
    interface Observable {
        required(message: string): Observable;
        equalTo(message: string, other: string): Observable;
        mustBe(message: string, value: any): Observable;
        regex(message: string, pattern: RegExp): Observable;
    }
}

declare var ko: Knockout.KnockoutObject;