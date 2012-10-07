/// <reference path="..\ref\signals.d.ts" />

var busses : Object = {};

export function create(name: string) {
    busses[name] = new Sink();
}

export function get(name: string) {
    if (!busses.hasOwnProperty(name)) {
        throw new Error('No such message bus: ' + name);
    }
    return <Sink>busses[name];
}

class Sink {
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

    public register(name: string) {
        
    }
}