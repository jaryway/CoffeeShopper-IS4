var mgr = new oidc.UserManager({
//   authority: "https://localhost:5443/",
//   client_id: "js_oidc",
//   redirect_uri: window.location.origin + "/callback.html",
//   post_logout_redirect_uri: window.location.origin + "/index.html",
// 
  // if we choose to use popup window instead for logins
//   popup_redirect_uri: window.location.origin + "/popup.html",
//   popupWindowFeatures: "menubar=yes,location=yes,toolbar=yes,width=1200,height=800,left=100,top=100;resizable=yes",

  // these two will be done dynamically from the buttons clicked, but are
  // needed if you want to use the silent_renew
//   response_type: "code",
//   scope: "openid profile email CoffeeAPI.read CoffeeAPI.write DynamicWebApi.all",

  // this will toggle if profile endpoint is used
//   loadUserInfo: true,

  // silent renew will get a new access_token via an iframe
  // just prior to the old access_token expiring (60 seconds prior)
//   silent_redirect_uri: window.location.origin + "/silent.html",
});
console.log("signinSilentCallback");
mgr.signinSilentCallback();
