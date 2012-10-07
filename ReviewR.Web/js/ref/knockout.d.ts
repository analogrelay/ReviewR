declare module Knockout {
    declare interface Observable {
        (): any;
        (value: any): void;
    }

    declare interface KnockoutObject {
        applyBindings(model: any, rootElement?: Element): void;
        applyBindingsToDescendants(model: any, rootElement?: Element): void;
        renderTemplate(template: string, model: any, options: Object, element: HTMLElement): void;
        
        observable(initialValue: any): Observable;
        computed(calculation: () => any): Observable;
    }
}

declare var ko: Knockout.KnockoutObject;