import { useAuth } from "auth/use-auth";
import { User, UserManager } from "oidc-client-ts";
// import { settings } from "oidc/settings";
import { useEffect, useState } from "react";

export function useCurrentUser() {
  // TODO: implement

  // const [currentUser, setcurrentUser] = useState<any>();
  // const [loading, setLoading] = useState(false);

  const { user } = useAuth() || {};

  return user?.profile as User["profile"] | undefined;
}
