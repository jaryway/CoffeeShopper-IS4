// import { PropsWithChildren } from "react";
// import AuthProvider from "auth/AuthProvider";
import { useAuth } from "auth/use-auth";
import { hasAuthParams } from "auth/utils";
import Indicator from "component/indicator";
import { useEffect, useRef } from "react";
import { Outlet, useNavigate } from "react-router-dom";

const SecurityLayout = () => {
  const { isLoading, isAuthenticated, signinRedirect, activeNavigator } = useAuth();

  const flag = useRef(false);

  // automatically sign-in
  useEffect(() => {
    if (!hasAuthParams() && !isAuthenticated && !activeNavigator && !isLoading && !flag.current) {
      signinRedirect();
      flag.current = true;
    }
  }, [activeNavigator, isAuthenticated, isLoading, signinRedirect]);

  if (activeNavigator === "signinRedirect") {
    return <Indicator title="登录中..." />;
  }

  if (activeNavigator === "signoutRedirect") {
    return <Indicator title="登出中..." />;
  }

  if (isLoading) {
    return <Indicator title="加载中..." />;
  }

  if (!isAuthenticated) {
    return <Indicator title="登录出错，请刷新页面重试登录" />;
  }

  return <Outlet />;
};

// function SecurityLayout() {
//   console.log("SecurityLayout");
//   const navigate = useNavigate();

//   return (
//     <AuthProvider
//       {...{
//         authority: "https://localhost:5443/",
//         client_id: "interactive",
//         // 登录后的回跳地址
//         redirect_uri: "http://localhost:4100/?redirect=signin-callback",
//         // 注销后的回跳地址
//         post_logout_redirect_uri: "http://localhost:4100/signout-success",
//         response_type: "code",
//         scope: "openid profile CoffeeAPI.read offline_access",
//         // 自动刷新 token
//         automaticSilentRenew: true,
//         loadUserInfo: true,
//         monitorSession: true,
//         onSigninCallback() {
//           // console.log("onSigninCallback");
//           navigate("/");
//         },
//         onSignoutCallback() {
//           // console.log("onSignoutCallback");
//         },
//       }}
//     >
//       <InnerSecurityLayout />
//     </AuthProvider>
//   );
// }

SecurityLayout.displayName = "SecurityLayout";

export default SecurityLayout;
