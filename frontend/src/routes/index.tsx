// import { lazy } from "react";

import { Outlet, type RouteObject } from "react-router-dom";
import { DefaultErrorBoundary } from "../DefaultErrorBoundary";
import AuthProvider from "../auth/AuthProvider";

const lazyComponent = (loader: () => Promise<any>) => {
  return async () => {
    const {
      default: DefaultComponent,
      Component,
      ErrorBoundary,
    } = (await loader()) as {
      default: React.ComponentType | null;
      Component: React.ComponentType | null;
      ErrorBoundary: React.ComponentType | null;
    };

    return { Component: DefaultComponent || Component, ErrorBoundary: ErrorBoundary || DefaultErrorBoundary };
  };
};

const RootComponent = () => {
  return (
    <AuthProvider
      {...{
        authority: "https://localhost:5443/",
        client_id: "interactive",
        // 登录后的回跳地址
        redirect_uri: "http://localhost:4100/?redirect=/signin-callback",
        // 注销后的回跳地址
        post_logout_redirect_uri: "http://localhost:4100/signout-success",
        response_type: "code",
        scope: "openid profile CoffeeAPI.read offline_access",
        // 自动刷新 token
        automaticSilentRenew: true,
        loadUserInfo: true,
        monitorSession: true,
        onSigninCallback() {
          // console.log("onSigninCallback");
          // navigate("/");
        },
        onSignoutCallback() {
          // console.log("onSignoutCallback");
        },
      }}
    >
      <Outlet />
    </AuthProvider>
  );
};

const routes: RouteObject[] = [
  {
    element: <RootComponent />,
    children: [
      {
        lazy: lazyComponent(() => import("layouts/GuestLayout")),
        children: [
          {
            index: true,
            path: "/",
            lazy: lazyComponent(() => import("pages/Welcome")),
          },
        ],
      },
      {
        path: "/dashboard",
        lazy: lazyComponent(() => import("layouts/SecurityLayout")),
        children: [
          {
            // index: true,
            lazy: lazyComponent(() => import("layouts/BasicLayout")),
            children: [
              {
                index: true,
                lazy: lazyComponent(() => import("pages/Home")),
              },
            ],
          },
        ],
      },
      {
        path: "/signout-success",
        lazy: lazyComponent(() => import("auth/SignOutSuccess")),
      },
    ],
  },
];

export default routes;
