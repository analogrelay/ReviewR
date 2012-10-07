/// <reference path="..\ref\knockout.d.ts" />

module syrah {
    export module utils {
        export function update(source: Object, modified: Object) {
            for (var key in modified) {
                if (modified.hasOwnProperty(key) && source.hasOwnProperty(key)) {
                    var current = source[key];
                    if (ko && ko.isWriteableObservable(current)) {
                        current(modified[key]);
                    } else {
                        source[key] = modified[key];
                    }
                }
            }
        }
    }
}