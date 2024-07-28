import { useEffect, useRef } from "react";
import { Outlet } from "react-router-dom";
import { useAuth } from "auth/use-auth";
import { hasAuthParams } from "auth/utils";
import BasicLayout from "./BasicLayout";
import Indicator from "components/indicator";

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

  return (
    <BasicLayout>
      <Outlet />
    </BasicLayout>
  );
};

SecurityLayout.displayName = "SecurityLayout";

export default SecurityLayout;
