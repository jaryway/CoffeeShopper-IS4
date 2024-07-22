// import { lazy } from "react";

import type { RouteObject } from "react-router-dom";
import { DefaultErrorBoundary } from "../DefaultErrorBoundary";

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

const routes: RouteObject[] = [
  {
    // path: "/",
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
  // {
  //   path: "/signin-oidc",
  //   lazy: lazyComponent(() => import("auth/SignInCallback")),
  // },
  {
    path: "/signout-success",
    lazy: lazyComponent(() => import("auth/SignOutSuccess")),
  },
  // {
  //   path: "/test",
  //   lazy: lazyComponent(() => import("pages/Test")),
  // },
];

export default routes;
