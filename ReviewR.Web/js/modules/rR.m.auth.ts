/// <reference path="..\ref\knockout.d.ts" />
/// <reference path="..\ref\knockout.validation.d.ts" />
import sy = module('../fx/syrah');
import sybus = module('../fx/syrah.bus');
import rR = module('../rR.app');

interface MoreDataViewModelInit {
    authToken?: string;
    needName?: bool;
    needEmail?: bool;
}

export class MoreDataViewModel extends sy.DialogViewModelBase {
    public displayName: Knockout.Observable;
    public email: Knockout.Observable;
    public authToken: string;
    public needName: bool;
    public needEmail: bool;

    constructor ();
    constructor (init: MoreDataViewModelInit);
    constructor (init?: MoreDataViewModelInit) {
        super();

        this.displayName = ko.observable('');
        this.email = ko.observable('');

        this.authToken = init.authToken || '';
        this.needName = init.needName || false;
        this.needEmail = init.needEmail || false;

        if (this.needName) {
            this.displayName.required('Display Name is required');
        }
        if (this.needEmail) {
            this.email.required('Email is required');
        }
    }

    public submit() {
        this.validate();
        if (this.isValid()) {
            $.post(
                '~/api/v1/sessions',
                { authToken: this.authToken, email: this.email(), displayName: this.displayName() })
                .done(data => {
                    // Grab the rest of the user data and send it to the app level login action
                    sybus.get('auth.setToken').publish(data.user);

                    // Dismiss the dialog
                    self.close();
                })
                .fail(xhr => {
                    switch (xhr.status) {
                        case 400:
                            // Still not enough data?
                            this.customError('There was a validation error on the server');
                            break;
                        case 409:
                            // Conflict, someone already has this email address
                            this.customError('Someone already has that email address registered!');
                            break;
                    }
                });
        }
    }
}

export class LoginViewModel extends sy.DialogViewModelBase {
    public makeLoginHandler(id: string, url: string) {
        return function () {
            doauth(id, rR.app.resolveUrl(url));
        };
    }
}

function doauth(type: string, urlTemplate: string);
function doauth(type: string, urlTemplate: string, tokenExtractor: (any) => { token: string; });
function doauth(type: string, urlTemplate: string, tokenExtractor?: (any) => { token: string; }) {
    if (!tokenExtractor) {
        tokenExtractor = function (args) { return { token: args.access_token }; }
    }

    var redirectUrl = rR.app.resolveUrl('~/auth/' + type, true);
    var resp = window.showModalDialog(urlTemplate.replace(/__LAND__/, redirectUrl));
    if (resp.success) {
        $.ajax('~/api/v1/sessions/' + type, {
            type: 'post',
            data: tokenExtractor(resp.args)
        }).done(function (data) {
            sybus.get('dismiss').publish();
            sybus.get('auth.setToken').publish(data.user);
        }).fail(xhr => {
            if (xhr.status == 400) {
                var data = JSON.parse(xhr.responseText);
                alert('Sorry, your Auth provider didn\'t provide the following required info. At the moment, we require this information:\r\n\r\n' + data.missingFields.join('\r\n'));
            }
        });
    } else {
        alert('Error authenticating!');
    }
}

class AuthModule extends sy.Module {
    constructor () {
        super('auth');

        this.dialog('login', LoginViewModel);
        this.dialog('moreData', MoreDataViewModel);

        this.action('logout', function () {
            $.ajax('~/api/v1/sessions', { type: 'delete' })
                .done(function () {
                    sybus.get('auth.clearToken').publish();
                });
        });

        this.action('login', () => this.showDialog('login'));
        this.action('moredata', (authToken, missingFields) => {
            this.showDialog('moreData', new MoreDataViewModel({
                authToken: authToken,
                needName: missingFields.indexOf('displayName') > -1,
                needEmail: missingFields.indexOf('email') > -1
            }));
        });

        this.route('landing', 'auth/:type', type => {
            var args : any = {};
            if (location.hash) {
                var hash = location.hash;
                if (hash[0] === '#') {
                    hash = location.hash.substr(1);
                }
                var pairs = hash.split(/&/);
                for (var i = 0; i < pairs.length; i++) {
                    var pair = pairs[i].split(/=/);
                    args[pair[0]] = pair[1];
                }
            }

            var success = true;
            if (args.error) {
                success = false;
            }
            
            (<WindowModal>(<any>window)).returnValue = {
                type: type,
                args: args,
                success: success
            };
            window.close();
        });
    }
}
rR.module(AuthModule);