import { UserManager, Log } from "oidc-client-ts";
import type { PropsWithChildren } from "react";
import React, { useEffect } from "react";
import { settings } from "./settings";

const OIDCContext = React.createContext({});

export type OIDCProviderProps = PropsWithChildren<{
  /**
   * 是否在加载后刷新用户信息，在 base 中可以设为 true，微应用中不需要，避免多次刷新
   *  */
  //   reloadUserInfoOnMounted?: boolean;
  /**
   * 如果没有登录，是否跳转去登录页面，在 base 中可以设为 true，微应用中不需要
   */
  //   redirect2LoginIfNotLogedIn?: boolean;
  //   indicator?: React.ReactElement<HTMLElement>;
  // refreshUserInfoURL?: string;
}>;

const mgr = new UserManager({
  ...settings,
  loadUserInfo: true,
  filterProtocolClaims: true,
  response_mode: "query",
});
Log.setLogger(console);
Log.setLevel(Log.INFO);

let flag = false;

const loginIfNotLogedIn = async () => {
  console.log("loginIfNotLogedIn");

  if (flag) return;

  flag = true;
  const user = await mgr.getUser();

  if (user) return;

  const result = mgr.signinRedirect();
  console.log("signinRedirect", result);
  flag = false;
};

const OIDCProvider = ({ children }: OIDCProviderProps) => {
  useEffect(() => {
    loginIfNotLogedIn();
  }, []);

  return <OIDCContext.Provider value={{}}>{children}</OIDCContext.Provider>;
};

export default OIDCProvider;
