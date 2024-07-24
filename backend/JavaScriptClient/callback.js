var mgr = new oidc.UserManager({
  authority: "https://localhost:5443/",
  client_id: "js_oidc",
//   loadUserInfo: true,
//   filterProtocolClaims: true,
//   response_mode: "query",
});
console.log("xxxxxxxxxxx", oidc, mgr);
mgr.signinRedirectCallback().then(function (user) {
  console.log(user);
  window.history.replaceState({}, window.document.title, window.location.origin + window.location.pathname);
  window.location = "index.html";
});
