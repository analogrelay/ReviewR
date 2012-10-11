var __extends = this.__extends || function (d, b) {
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
}
var sy = require('../fx/syrah')
var sybus = require('../fx/syrah.bus')
var rR = require('../rR.app')
var MoreDataViewModel = (function (_super) {
    __extends(MoreDataViewModel, _super);
    function MoreDataViewModel(init) {
        _super.call(this);
        this.displayName = ko.observable('');
        this.email = ko.observable('');
        this.authToken = init.authToken || '';
        this.needName = init.needName || false;
        this.needEmail = init.needEmail || false;
        if(this.needName) {
            this.displayName.required('Display Name is required');
        }
        if(this.needEmail) {
            this.email.required('Email is required');
        }
    }
    MoreDataViewModel.prototype.submit = function () {
        var _this = this;
        this.validate();
        if(this.isValid()) {
            $.post('~/api/v1/sessions', {
                authToken: this.authToken,
                email: this.email(),
                displayName: this.displayName()
            }).done(function (data) {
                sybus.get('auth.setToken').publish(data.user);
                self.close();
            }).fail(function (xhr) {
                switch(xhr.status) {
                    case 400: {
                        _this.customError('There was a validation error on the server');
                        break;

                    }
                    case 409: {
                        _this.customError('Someone already has that email address registered!');
                        break;

                    }
                }
            });
        }
    };
    return MoreDataViewModel;
})(sy.DialogViewModelBase);
exports.MoreDataViewModel = MoreDataViewModel;
var LoginViewModel = (function (_super) {
    __extends(LoginViewModel, _super);
    function LoginViewModel() {
        _super.apply(this, arguments);

    }
    LoginViewModel.prototype.makeLoginHandler = function (id, url) {
        return function () {
            doauth(id, rR.app.resolveUrl(url));
        }
    };
    return LoginViewModel;
})(sy.DialogViewModelBase);
exports.LoginViewModel = LoginViewModel;
function doauth(type, urlTemplate, tokenExtractor) {
    if(!tokenExtractor) {
        tokenExtractor = function (args) {
            return {
                token: args.access_token
            };
        };
    }
    var redirectUrl = rR.app.resolveUrl('~/auth/' + type, true);
    var resp = window.showModalDialog(urlTemplate.replace(/__LAND__/, redirectUrl));
    if(resp.success) {
        $.ajax('~/api/v1/sessions/' + type, {
            type: 'post',
            data: tokenExtractor(resp.args)
        }).done(function (data) {
            sybus.get('dismiss').publish();
            sybus.get('auth.setToken').publish(data.user);
        }).fail(function (xhr) {
            if(xhr.status == 400) {
                var data = JSON.parse(xhr.responseText);
                alert('Sorry, your Auth provider didn\'t provide the following required info. At the moment, we require this information:\r\n\r\n' + data.missingFields.join('\r\n'));
            }
        });
    } else {
        alert('Error authenticating!');
    }
}
var AuthModule = (function (_super) {
    __extends(AuthModule, _super);
    function AuthModule() {
        var _this = this;
        _super.call(this, 'auth');
        this.dialog('login', LoginViewModel);
        this.dialog('moreData', MoreDataViewModel);
        this.action('logout', function () {
            $.ajax('~/api/v1/sessions', {
                type: 'delete'
            }).done(function () {
                sybus.get('auth.clearToken').publish();
            });
        });
        this.action('login', function () {
            return _this.showDialog('login');
        });
        this.action('moredata', function (authToken, missingFields) {
            _this.showDialog('moreData', new MoreDataViewModel({
                authToken: authToken,
                needName: missingFields.indexOf('displayName') > -1,
                needEmail: missingFields.indexOf('email') > -1
            }));
        });
        this.route('landing', 'auth/:type', function (type) {
            var args = {
            };
            if(location.hash) {
                var hash = location.hash;
                if(hash[0] === '#') {
                    hash = location.hash.substr(1);
                }
                var pairs = hash.split(/&/);
                for(var i = 0; i < pairs.length; i++) {
                    var pair = pairs[i].split(/=/);
                    args[pair[0]] = pair[1];
                }
            }
            var success = true;
            if(args.error) {
                success = false;
            }
            ((window)).returnValue = {
                type: type,
                args: args,
                success: success
            };
            window.close();
        });
    }
    return AuthModule;
})(sy.Module);
rR.module(AuthModule);

//@ sourceMappingURL=rR.m.auth.js.map
