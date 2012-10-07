/// <reference path="jquery.d.ts" />

module Bootstrap {
    declare interface ModalArgs {
        backdrop?: bool;
        keyboard?: bool;
        show?: bool;
        remote?: string;
    }
}

declare interface JQuery {
    modal(args: Bootstrap.ModalArgs): JQuery;
    modal(action: string);
}