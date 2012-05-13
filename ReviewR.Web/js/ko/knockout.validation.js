/// <reference path="../../Scripts/knockout.debug.js" />
(function (window, ko, undefined) {
    ko.validation = {};
    ko.validation.validators = {};

    function validator(params, handler) {
        return {
            create: function (target, message, args) {
                /// <param name="target" type="ko.observable" />
                var self = {};
                self.isValid = ko.computed(function () {
                    return handler.apply(target, args)
                });
                self.errorMessage = ko.observable(message);
                return self;
            },
            parameters: params
        };
    };
    ko.validation.validator = validator;

    ko.validation.attach = function (ob, msg, val, args) {
        /// <param name="ob" type="ko.observable" />
        /// <param name="val" type="ko.validation.validator" />
        /// <param name="arguments" type="Array" />
        ensureValidation(ob);
        ob.validators.push(val.create(ob, msg, args));
        return ob;
    }

    ko.validation.validators._list = [];
    ko.validation.validators.add = function (name, params, impl) {
        /// <summary>Adds a validator</summary>
        /// <param name="name" type="String">The name of the validator</param>
        /// <param name="params" type="Array">An array of parameter names accepted by the implementation</param>
        /// <param name="impl" type="Function">
        /// A function which takes the arguments listed in params and returns true or false depending
        /// on the values of this (which is the target observable) and other the parameters
        /// </param>
        ko.validation.validators._list[name] = validator(params, impl);
    }

    ko.validation.validators.add('required', [], function () {
        return this() !== null && this() !== ''
    });

    ko.validation.validators.add('equalTo', ['other'], function (other) {
        return this() === other();
    });

    ko.validation.validators.add('mustBe', ['value'], function (value) {
        return this() === value;
    });

    ko.validation.validators.add('regex', ['pattern'], function (pattern) {
        /// <param name="pattern" type="Regex" />
        return this().match(pattern);
    });

    function ensureValidation(ob) {
        /// <param name="ob" type="ko.observable" />
        if (!ob.validators) {
            ob.validators = ko.observableArray([]);
            ob.validating = ko.observable(false);
            ob.isValid = ko.computed(function () {
                var v = true;
                if (ob.validating()) {
                    ko.utils.arrayForEach(ob.validators(), function (val) {
                        v &= val.isValid();
                    });
                }
                return v;
            });
            ob.errorMessage = ko.computed(function () {
                var errorMessage;
                if (ob.validating()) {
                    ko.utils.arrayForEach(ob.validators(), function (val) {
                        var thisMsg = val.errorMessage();
                        if (!val.isValid() && !errorMessage) {
                            errorMessage = thisMsg;
                        }
                    });
                }
                return errorMessage || '';
            });
        }
    }

    ko.validation.addValidation = function (self) {
        self = self || {};
        self.customError = ko.observable('');
        self.isValid = ko.computed(function () {
            var v = true;
            for (var key in self) {
                if (self.hasOwnProperty(key) && ko.isObservable(self[key]) && self[key].validating && self[key].validating()) {
                    v &= self[key].isValid();
                }
            }
            return v;
        });
        self.errorMessage = ko.computed(function () {
            return self.isValid() ? self.customError() : 'Whoops, there were some errors :(';
        });
        self.hasMessage = ko.computed(function () {
            return !self.isValid() || (self.customError() && (self.customError().length > 0));
        });
        self.validate = function () {
            for (var key in self) {
                if (self.hasOwnProperty(key) && ko.isObservable(self[key]) && self[key].validating) {
                    self[key].validating(true);
                }
            }
        };
    }
    
    ko.validation.refreshValidators = function () {
        /// <summary>Reattach all validators to ko.observable</summary>
        var validators = ko.validation.validators._list;
        for (var key in validators) {
            if (validators.hasOwnProperty(key)) {
                var validator = validators[key];
                ko.observable.fn[key] =
                    new Function(
                        ['message'].concat(validator.parameters),
                        "return ko.validation.attach(this, message, ko.validation.validators._list." + key + ", arguments.length > 1 ? Array.prototype.slice.call(arguments, 1) : []);");
            }
        }
    }
    ko.validation.refreshValidators();
})(window, window.ko);