declare module signals {
    export declare class SignalBinding {
        public active: bool;
        public context: Object;
        public params: any[];

        constructor (signal: Signal, listener: Function , isOnce: bool);
        constructor (signal: Signal, listener: Function , isOnce: bool, listenerContext: Object);
        constructor (signal: Signal, listener: Function , isOnce: bool, listenerContext: Object, priority: Number);
        constructor (signal: Signal, listener: Function , isOnce: bool, listenerContext?: Object, priority?: Number);

        public detach(): Function;

        public execute(...params: any[]): any;
        public getListener(): Function;
        public isBound(): bool;
        public isOnce(): bool;
        public toString(): string;
    }

    export declare class Signal {
        public active: bool;
        public memorize: bool;
        public VERSION: string;

        public dispatch(...args: any[]): void;

        public add(listener: Function): SignalBinding;
        public add(listener: Function, listenerContext: Object): SignalBinding;
        public add(listener: Function, listenerContext: Object, priority: Number): SignalBinding;
        public add(listener: Function, listenerContext?: Object, priority?: Number): SignalBinding;

        public addOnce(listener: Function): SignalBinding;
        public addOnce(listener: Function, listenerContext: Object): SignalBinding;
        public addOnce(listener: Function, listenerContext: Object, priority: Number): SignalBinding;
        public addOnce(listener: Function, listenerContext?: Object, priority?: Number): SignalBinding;

        public dispose(): void;
        public forget(): void;
        public getNumListeners(): Number;
        public halt(): void;

        public has(listener: Function): bool;
        public has(listener: Function, context: Object): bool;
        public has(listener: Function, context?: Object): bool;

        public remove(listener: Function): Function;
        public remove(listener: Function, context: Object): Function;
        public remove(listener: Function, context?: Object): Function;

        public removeAll(): void;
        public toString(): string;
    }
}