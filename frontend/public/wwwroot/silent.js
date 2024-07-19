var mgr = new Oidc.UserManager();
Oidc.Log.logger = window.console;
Oidc.Log.level = Oidc.Log.DEBUG;
mgr.signinSilentCallback();
