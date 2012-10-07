declare module Knockout {
    declare interface Observable {
        (): any;
        (value: any): void;
    }

    declare interface ObservableArray extends Observable {
        (): any[];
        (value: any[]): void;
    }

    declare interface KnockoutObject {
        applyBindings(model: any, rootElement?: Element): void;
        applyBindingsToDescendants(model: any, rootElement?: Element): void;
        renderTemplate(template: string, model: any, options: Object, element: HTMLElement): void;
        
        observable(): Observable;
        observable(initialValue: any): Observable;
        observableArray(): ObservableArray;
        observableArray(initialValue: any[]): ObservableArray;
        computed(calculation: () => any): Observable;

        isWriteableObservable(obj: any): bool;
        isObservable(obj: any): bool;
    }
}

declare var ko: Knockout.KnockoutObject;