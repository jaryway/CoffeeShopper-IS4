﻿/// <reference path="libs/oidc-client-ts.js" />

var config = {
  authority: "http://localhost:5000/",
  client_id: "js_oidc",
  redirect_uri: window.location.origin + "/callback.html",
  post_logout_redirect_uri: window.location.origin + "/index.html",

  // if we choose to use popup window instead for logins
  // popup_redirect_uri: window.location.origin + "/popup.html",
  // popupWindowFeatures: "menubar=yes,location=yes,toolbar=yes,width=1200,height=800,left=100,top=100;resizable=yes",

  // these two will be done dynamically from the buttons clicked, but are
  // needed if you want to use the silent_renew
  response_type: "code",
  scope: "openid profile email offline_access CoffeeAPI.read CoffeeAPI.write DynamicWebApi.all",

  // this will toggle if profile endpoint is used
//   loadUserInfo: true,

  // silent renew will get a new access_token via an iframe
  // just prior to the old access_token expiring (60 seconds prior)
//   silent_redirect_uri: window.location.origin + "/silent.html",
//   includeIdTokenInSilentRenew: true,
//   automaticSilentRenew: true,

  // monitorAnonymousSession: true,

  // will revoke (reference) access tokens at logout time
  // revokeAccessTokenOnSignout: true,
  // 监控 seesion 变化，用于实现单点登出
  // 注意：由于浏览器安全的限制，客户端和idp需要使用相同的协议，比如：都是https；否则会失败
  monitorSession: true,

  // this will allow all the OIDC protocol claims to be visible in the window. normally a client app
  // wouldn't care about them or want them taking up space
  filterProtocolClaims: false,
//   response_mode: "query",
};

oidc.Log.setLogger(window.console);
oidc.Log.setLevel(oidc.Log.DEBUG);

// oidc.Log.logger = window.console;
// oidc.Log.level = oidc.Log.DEBUG;

// console.log("oidc", oidc);

var mgr = new oidc.UserManager(config);

mgr.events.addUserLoaded(function (user) {
  log("User loaded");
  showTokens();
});
mgr.events.addUserUnloaded(function () {
  log("User logged out locally");
  showTokens();
});
mgr.events.addAccessTokenExpiring(function () {
  log("Access token expiring...");
});
mgr.events.addSilentRenewError(function (err) {
  log("Silent renew error: " + err.message);
});
mgr.events.addUserSignedIn(function (e) {
  log("user logged in to the token server");
});
mgr.events.addUserSignedOut(function () {
  log("User signed out of OP");
  //   mgr.removeUser();
});

function login() {
  mgr.signinRedirect();
}

function popup() {
  mgr.signinPopup().then(function () {
    log("Logged In");
  });
}

function logout() {
  mgr.signoutRedirect();
}

function revoke() {
  mgr
    .revokeAccessToken()
    .then(function () {
      log("Access Token Revoked.");
    })
    .catch(function (err) {
      log(err);
    });
}

function renewToken() {
  mgr
    .signinSilent()
    .then(function () {
      log("silent renew success");
      showTokens();
    })
    .catch(function (err) {
      log("silent renew error", err);
    });
}
function callApi() {
  mgr.getUser().then(function (user) {
    // console.log("useruseruseruser", user);
    var xhr = new XMLHttpRequest();
    xhr.onload = function (e) {
      if (xhr.status >= 400) {
        display("#ajax-result", {
          status: xhr.status,
          statusText: xhr.statusText,
          wwwAuthenticate: xhr.getResponseHeader("WWW-Authenticate"),
        });
      } else {
        display("#ajax-result", xhr.response);
      }
    };
    xhr.open("GET", "https://localhost:5446/api/Runtime/Query", true);
    xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
    xhr.send();
  });
}

if (window.location.hash) {
  handleCallback();
}

document.querySelector(".login").addEventListener("click", login, false);
document.querySelector(".popup").addEventListener("click", popup, false);
document.querySelector(".renew").addEventListener("click", renewToken, false);
document.querySelector(".call").addEventListener("click", callApi, false);
document.querySelector(".revoke").addEventListener("click", revoke, false);
document.querySelector(".logout").addEventListener("click", logout, false);

function log(data) {
  document.getElementById("response").innerText = "";

  Array.prototype.forEach.call(arguments, function (msg) {
    if (msg instanceof Error) {
      msg = "Error: " + msg.message;
    } else if (typeof msg !== "string") {
      msg = JSON.stringify(msg, null, 2);
    }
    document.getElementById("response").innerText += msg + "\r\n";
  });
}

function display(selector, data) {
  if (data && typeof data === "string") {
    try {
      data = JSON.parse(data);
    } catch (e) {}
  }
  if (data && typeof data !== "string") {
    data = JSON.stringify(data, null, 2);
  }
  document.querySelector(selector).textContent = data;
}

function showTokens() {
  mgr.getUser().then(function (user) {
    if (user) {
      display("#id-token", user);
    } else {
      log("Not logged in");
    }
  });
}
showTokens();

function handleCallback() {
  mgr.signinRedirectCallback().then(
    function (user) {
      var hash = window.location.hash.substr(1);
      var result = hash.split("&").reduce(function (result, item) {
        var parts = item.split("=");
        result[parts[0]] = parts[1];
        return result;
      }, {});

      log(result);
      showTokens();

      window.history.replaceState({}, window.document.title, window.location.origin + window.location.pathname);
    },
    function (error) {
      log(error);
    }
  );
}
