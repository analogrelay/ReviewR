/// <reference path="../ref/knockout.d.ts" />
/// <reference path="syrah.rendering.ts" />
module syrah {
    export module binding {
        export class KnockoutViewHost extends syrah.rendering.ViewHost {
            constructor (private host: HTMLElement) {
                super(host);
            }

            public injectView(name : string, model : any) {
                ko.renderTemplate('v:' + name, model, {}, this.host);
            }
        }
    }
}