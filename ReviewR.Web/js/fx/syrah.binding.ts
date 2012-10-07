/// <reference path="../ref/knockout.d.ts" />
import syren = module('syrah.rendering');
export class KnockoutViewHost extends syren.ViewHost {
    constructor (private host: HTMLElement) {
        super(host);
    }

    public injectView(name : string, model : any) {
        ko.renderTemplate('v:' + name, model, {}, this.host);
    }
}