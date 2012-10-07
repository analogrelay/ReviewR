declare interface Knockout {
    applyBindings(model: any, rootElement?: Element) : void;
    applyBindingsToDescendants(model: any, rootElement?: Element) : void;
    renderTemplate(template: string, model: any, options: Object, element: HTMLElement) : void;
}
declare var ko: Knockout;