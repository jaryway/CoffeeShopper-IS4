var config = {
  authority: "https://localhost:5443/",
  client_id: "js_oidc",
  redirect_uri: window.location.origin + "/wwwroot/callback.html",
  post_logout_redirect_uri: window.location.origin + "/wwwroot/index.html",

  // if we choose to use popup window instead for logins
  popup_redirect_uri: window.location.origin + "/wwwroot/popup.html",
  popupWindowFeatures: "menubar=yes,location=yes,toolbar=yes,width=1200,height=800,left=100,top=100;resizable=yes",

  // these two will be done dynamically from the buttons clicked, but are
  // needed if you want to use the silent_renew
  response_type: "code",
  scope: "openid profile email DynamicWebApi.all offline_access",

  // this will toggle if profile endpoint is used
  loadUserInfo: true,

  // silent renew will get a new access_token via an iframe
  // just prior to the old access_token expiring (60 seconds prior)
  silent_redirect_uri: window.location.origin + "/wwwroot/silent.html",
  automaticSilentRenew: true,

  monitorAnonymousSession: true,

  // will revoke (reference) access tokens at logout time
  revokeAccessTokenOnSignout: true,

  // this will allow all the OIDC protocol claims to be visible in the window. normally a client app
  // wouldn't care about them or want them taking up space
  filterProtocolClaims: true,
  monitorSession: true,
};

oidc.Log.logger = window.console;
oidc.Log.level = oidc.Log.DEBUG;

var mgr = new oidc.UserManager(config);

mgr.signoutRedirectCallback().then(function () {
  console.log("Signout redirect callback complete");
  mgr.removeUser();
  sessionStorage.clear();
  window.location = "index.html";
});
