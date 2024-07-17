import { UserManager } from "oidc-client-ts";
import { settings } from "oidc/settings";
import { useEffect, useState } from "react";

const mgr = new UserManager(settings);
let flag = false;

export function useCurrentUser() {
  // TODO: implement

  const [currentUser, setcurrentUser] = useState<any>();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (flag) return;

    flag = true;
    setLoading(true);

    mgr
      .getUser() //
      .then((user) => {
        setcurrentUser(user?.profile);
      })
      .finally(() => {
        flag = false;
        setLoading(false);
      });
  }, []);

  return { currentUser, loading, logout: () => mgr.signoutRedirect() };
}
