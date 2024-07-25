var mgr = new oidc.UserManager({
  authority: "https://localhost:5443/",
  client_id: "js_oidc",
  response_type: "code",
  scope: "openid profile email offline_access CoffeeAPI.read CoffeeAPI.write DynamicWebApi.all",
  loadUserInfo: true,
  filterProtocolClaims: true,
  response_mode: "query",
});
// console.log("oidc.Log.DEBUG", oidc.Log.DEBUG);

oidc.Log.setLogger(window.console);
oidc.Log.setLevel(oidc.Log.DEBUG);
// debugger;
console.log("mgr.signinRedirectCallback", window.location);
mgr.signinRedirectCallback().then(function (user) {
  console.log(user);
  window.history.replaceState({}, window.document.title, window.location.origin + window.location.pathname);
  window.location = "index.html";
});
