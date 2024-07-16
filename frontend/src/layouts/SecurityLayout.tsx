import { PropsWithChildren } from "react";
import OIDCProvider from "oidc/OIDCProvider";
import { Outlet } from "react-router-dom";

function SecurityLayout() {
  console.log("SecurityLayout");

  return (
    <OIDCProvider>
      <Outlet />
    </OIDCProvider>
  );
}

SecurityLayout.displayName = "SecurityLayout";
// export const Component = SecurityLayout;

export default SecurityLayout;
