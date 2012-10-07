var syrah;
(function (syrah) {
    (function (utils) {
        function update(source, modified) {
            for(var key in modified) {
                if(modified.hasOwnProperty(key) && source.hasOwnProperty(key)) {
                    var current = source[key];
                    if(ko && ko.isWriteableObservable(current)) {
                        current(modified[key]);
                    } else {
                        source[key] = modified[key];
                    }
                }
            }
        }
        utils.update = update;
    })(syrah.utils || (syrah.utils = {}));
    var utils = syrah.utils;

})(syrah || (syrah = {}));

