/// <reference path="..\ref\signals.d.ts" />
/// <reference path="syrah.dom.ts" />

module syrah {
    export module rendering {
        export interface ViewOptions {
            templateId?: string;
        }

        export class View {
            public injected = new signals.Signal();
            public removed = new signals.Signal();
            constructor (public templateId: string,
                         public modalConstructor: () => Object,
                         public options: ViewOptions) { }
        }

        export class Page extends View {
            public obscured = new signals.Signal();
            public revealed = new signals.Signal();
        }

        export class ViewHost {
            private currentView : View;
            constructor (private host: HTMLElement) {
            }

            public injectView(name : string, model : any) { }

            public obscure() {
                if (this.currentView instanceof Page) {
                    (<Page>this.currentView).obscured.dispatch();
                }
            }

            public reveal() {
                if (this.currentView instanceof Page) {
                    (<Page>this.currentView).revealed.dispatch();
                }
            }

            public clearView() {
                this.host.innerHTML = '';
                var old = this.currentView;
                this.currentView = null;
                if (old) {
                    old.removed.dispatch();
                }
            }

            public setView(view: View);
            public setView(view: View, model: any);
            public setView(view: View, model?: any) {
                this.clearView();
                
                // Create the view model if needed
                if (!model) {
                    // Not sure why, but this assertion is needed...
                    model = view.modalConstructor();
                }
                this.injectView(view.templateId, model);
                this.currentView = view;
                this.currentView.injected.dispatch(this.currentView, model);

                if (model.load && typeof (model.load) === "function") {
                    (<() => void>model.load)();
                }
            }

            public initDialog() {
                syrah.dom.initDialog(this.host, () => {
                    this.clearView();
                });
            }

            public showDialog();
            public showDialog(view: View, model: any);
            public showDialog(view? : View, model? : any) {
                if (view && model) {
                    this.setView(view, model);
                }
                syrah.dom.showDialog(this.host);
            }

            public closeDialog() {
                syrah.dom.hideDialog(this.host);
            }
        }
    }
}