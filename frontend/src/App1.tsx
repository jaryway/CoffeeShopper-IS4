import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { Spin } from "antd";
import "assets/styles/index.css";
import { UserManager, User } from "oidc-client-ts";
import type { ProcessResourceOwnerPasswordCredentialsArgs, UserManagerSettings } from "oidc-client-ts";
// import routes from "./routes";
// import { PUBLIC_URL } from "./env";

// const getLayoutMode = () => {
//   let pathname = window.location.pathname;
//   if (PUBLIC_URL) {
//     pathname = pathname.slice(PUBLIC_URL.length);
//   }

//   if (pathname.startsWith("/nosider")) {
//     return "nosider";
//   } else if (pathname.startsWith("/noheader")) {
//     return "noheader";
//   } else if (pathname.startsWith("/onlycontent")) {
//     return "onlycontent";
//   }
// };

const userManager = new UserManager({
  authority: "https://localhost:5443/",
  client_id: "interactive",
  // 登录后的回跳地址
  redirect_uri: "http://localhost:4100/",
  // 注销后的回跳地址
  post_logout_redirect_uri: "http://localhost:4100/signout-success",
  response_type: "code",
  scope: "openid profile CoffeeAPI.read offline_access",
  // 自动刷新 token
  // automaticSilentRenew: true,
  loadUserInfo: true,
  // monitorSession: true,
  filterProtocolClaims: true,
  monitorAnonymousSession: true,
});

const handleUserLoaded = (user: User) => {
  // dispatch({ type: "USER_LOADED", user });
  console.log("handleUserLoaded", user);
};

// event UserUnloaded (e.g. userManager.removeUser)
const handleUserUnloaded = () => {
  // dispatch({ type: "USER_UNLOADED" });
};

// event UserSignedOut (e.g. user was signed out in background (checkSessionIFrame option))
const handleUserSignedOut = async () => {
  console.log("handleUserSignedOut");

  // dispatch({ type: "USER_SIGNED_OUT" });
  await userManager.removeUser();
};

// event SilentRenewError (silent renew error)
const handleSilentRenewError = (error: Error) => {
  // dispatch({ type: "ERROR", error });
};

userManager.events.addUserLoaded(handleUserLoaded);
userManager.events.addUserUnloaded(handleUserUnloaded);
userManager.events.addUserSignedOut(handleUserSignedOut);
userManager.events.addSilentRenewError(handleSilentRenewError);

if (window.location?.search?.includes("code=")) {
  userManager.signinCallback().then(() => {
    window.location.href = "/";
  });
} else {
  userManager.getUser().then((user) => {
    if (!user) {
      userManager.signinRedirect();
    }
  });
}

function App() {
  // const layoutMode = getLayoutMode();
  // const router = createBrowserRouter(routes, { basename: PUBLIC_URL });
  // return <RouterProvider router={router} fallbackElement={<Spin />} />;
  return <div>sdfasdf</div>;
}

export default App;
