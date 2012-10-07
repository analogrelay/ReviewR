/// <reference path="..\ref\signals.d.ts" />

module syrah {
    export module bus {
        export class Sink {
            private sig = new signals.Signal();

            public publish(...params: any[]) {
                (<Function>this.sig.dispatch).apply(this.sig,
                    (<Function>Array.prototype.slice).apply(arguments));
            }

            public subscribe(handler: Function);
            public subscribe(handler: Function, context: Object);
            public subscribe(handler: Function, context: Object, priority: Number);
            public subscribe(handler: Function, context?: Object, priority?: Number) {
                return this.sig.add(handler, context, priority);
            }
        }
    }
}