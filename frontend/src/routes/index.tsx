// import { lazy } from "react";

import { Outlet, useNavigate, type RouteObject } from "react-router-dom";
import { DefaultErrorBoundary } from "../DefaultErrorBoundary";
import AuthProvider from "../auth/AuthProvider";
import ApiProvider from "api";

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
  const navigate = useNavigate();
  return (
    <AuthProvider
      {...{
        authority: "http://localhost:5000/",
        client_id: "interactive",
        // 登录后的回跳地址
        redirect_uri: "http://localhost:4100/",
        // 注销后的回跳地址
        post_logout_redirect_uri: "http://localhost:4100/signout-success",
        response_type: "code",
        scope: "openid profile DynamicWebApi.all CoffeeAPI.read offline_access",
        // 自动刷新 token
        automaticSilentRenew: true,
        loadUserInfo: true,
        monitorSession: true,
        onSigninCallback() {
          // console.log("onSigninCallback");
          navigate("/dashboard");
        },
        onSignoutCallback() {
          // console.log("onSignoutCallback");
        },
      }}
    >
      <ApiProvider>
        <Outlet />
      </ApiProvider>
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
            // element: <div>Welcome</div>,
          },
        ],
      },
      {
        lazy: lazyComponent(() => import("layouts/SecurityLayout")),
        children: [
          {
            path: "/dashboard",
            lazy: lazyComponent(() => import("pages/Home")),
          },

          {
            path: "/dynamic-object",
            children: [
              {
                index: true,
                path: "management",
                lazy: lazyComponent(() => import("pages/dynamic-object/Management")),
              },
              {
                path: "edit",
                lazy: lazyComponent(() => import("pages/dynamic-object/Edit")),
              },
            ],

            // element: <div>Home</div>,
          },
        ],
      },
      {},
      {
        path: "/signout-success",
        lazy: lazyComponent(() => import("auth/SignOutSuccess")),
      },
    ],
  },
];

export default routes;
