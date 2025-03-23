import { UserManager, User } from "oidc-client-ts";
import type { ProcessResourceOwnerPasswordCredentialsArgs, UserManagerSettings } from "oidc-client-ts";
import React, { useCallback, useEffect, useMemo, useReducer, useRef, useState } from "react";
import { AuthContext, AuthContextValue } from "./AuthContext";
import { initialAuthState } from "./AuthState";
import { reducer } from "./reducer";
import { hasAuthParams, signinError, signoutError } from "./utils";
import { useNavigate } from "react-router-dom";

const userManagerContextKeys = [
  "clearStaleState",
  "querySessionStatus",
  "revokeTokens",
  "startSilentRenew",
  "stopSilentRenew", //
] as const;

const navigatorKeys = [
  "signinPopup",
  "signinSilent",
  "signinRedirect",
  // "signinCallback",
  "signinResourceOwnerCredentials",
  "signoutPopup",
  "signoutRedirect",
  "signoutSilent",
] as const;

const unsupported = (fnName: string) => () => {
  throw new Error(
    `UserManager#${fnName} was called from an unsupported context. If this is a server-rendered page, defer this call with useEffect() or pass a custom UserManager implementation.`
  );
};
/**
 * @public
 */
export interface AuthProviderBaseProps {
  /**
   * The child nodes your Provider has wrapped
   */
  children?: React.ReactNode;
  /**
   * On sign in callback hook. Can be a async function.
   * Here you can remove the code and state parameters from the url when you are redirected from the authorize page.
   *
   * ```jsx
   * const onSigninCallback = (_user: User | void): void => {
   *     window.history.replaceState(
   *         {},
   *         document.title,
   *         window.location.pathname
   *     )
   * }
   * ```
   */
  onSigninCallback?: (user: User | void) => Promise<void> | void;

  /**
   * By default, if the page url has code/state params, this provider will call automatically the `userManager.signinCallback`.
   * In some cases the code might be for something else (another OAuth SDK perhaps). In these
   * instances you can instruct the client to ignore them.
   *
   * ```jsx
   * <AuthProvider
   *   skipSigninCallback={window.location.pathname === "/stripe-oauth-callback"}
   * >
   * ```
   */
  skipSigninCallback?: boolean;

  /**
   * Match the redirect uri used for logout (e.g. `post_logout_redirect_uri`)
   * This provider will then call automatically the `userManager.signoutCallback`.
   *
   * HINT:
   * Do not call `userManager.signoutRedirect()` within a `React.useEffect`, otherwise the
   * logout might be unsuccessful.
   *
   * ```jsx
   * <AuthProvider
   *   matchSignoutCallback={(args) => {
   *     window &&
   *     (window.location.href === args.post_logout_redirect_uri);
   *   }}
   * ```
   */
  matchSignoutCallback?: (args: UserManagerSettings) => boolean;

  /**
   * On sign out callback hook. Can be a async function.
   * Here you can change the url after the user is signed out.
   * When using this, specifying `matchSignoutCallback` is required.
   *
   * ```jsx
   * const onSignoutCallback = (): void => {
   *     // go to home after logout
   *     window.location.pathname = ""
   * }
   * ```
   */
  onSignoutCallback?: () => Promise<void> | void;

  /**
   * On remove user hook. Can be a async function.
   * Here you can change the url after the user is removed.
   *
   * ```jsx
   * const onRemoveUser = (): void => {
   *     // go to home after logout
   *     window.location.pathname = ""
   * }
   * ```
   */
  onRemoveUser?: () => Promise<void> | void;
}

/**
 * This interface (default) is used to pass `UserManagerSettings` together with `AuthProvider` properties to the provider.
 *
 * @public
 */
export interface AuthProviderNoUserManagerProps extends AuthProviderBaseProps, UserManagerSettings {
  /**
   * Prevent this property.
   */
  userManager?: never;
}

/**
 * This interface is used to pass directly a `UserManager` instance together with `AuthProvider` properties to the provider.
 *
 * @public
 */
export interface AuthProviderUserManagerProps extends AuthProviderBaseProps {
  /**
   * Allow passing a custom UserManager instance.
   */
  userManager?: UserManager;
}

/**
 * @public
 */
export type AuthProviderProps = AuthProviderNoUserManagerProps | AuthProviderUserManagerProps;

const AuthProvider = (props: AuthProviderProps) => {
  const {
    children,

    onSigninCallback,
    skipSigninCallback,

    matchSignoutCallback,
    onSignoutCallback,

    onRemoveUser,

    userManager: userManagerProp = null,
    ...userManagerSettings
  } = props;

  const navigate = useNavigate();
  const [userManager] = useState<UserManager>(() => {
    return userManagerProp || new UserManager(userManagerSettings as UserManagerSettings);
  });
  const [state, dispatch] = useReducer(reducer, initialAuthState);
  const didInitialize = useRef(false);

  useEffect(() => {
    if (!userManager || didInitialize.current) {
      return;
    }
    didInitialize.current = true;

    // sign-in
    const autoSignin = async () => {
      let user: User | void | null = null;
      try {
        // check if returning back from authority server
        if (hasAuthParams() && !skipSigninCallback) {
          console.log("signinCallback-11");
          user = await userManager.signinCallback();
          // console.log("signinCallback-22", user);

          onSigninCallback && (await onSigninCallback(user));
        }
        user = !user ? await userManager.getUser() : user;
        dispatch({ type: "INITIALISED", user });
      } catch (error) {
        // console.log("error", error);
        dispatch({ type: "ERROR", error: signinError(error) });
      }
    };

    const autoSignout = async () => {
      try {
        if (matchSignoutCallback && matchSignoutCallback(userManager.settings)) {
          // console.log("signoutCallback-22");

          await userManager.signoutCallback();
          onSignoutCallback && (await onSignoutCallback());
        }
      } catch (error) {
        dispatch({ type: "ERROR", error: signoutError(error) });
      }
    };
    autoSignin();
    autoSignout();
  }, [matchSignoutCallback, onSigninCallback, onSignoutCallback, skipSigninCallback, userManager]);

  useEffect(() => {
    if (!userManager) return undefined;

    const handleUserLoaded = (user: User) => {
      dispatch({ type: "USER_LOADED", user });
      // console.log("handleUserLoaded", user);
    };

    // event UserUnloaded (e.g. userManager.removeUser)
    const handleUserUnloaded = () => {
      dispatch({ type: "USER_UNLOADED" });
    };

    // event UserSignedOut (e.g. user was signed out in background (checkSessionIFrame option))
    const handleUserSignedOut = async () => {
      console.log("handleUserSignedOut");

      dispatch({ type: "USER_SIGNED_OUT" });
      await userManager.removeUser();
      navigate("/");
    };

    // event SilentRenewError (silent renew error)
    const handleSilentRenewError = (error: Error) => {
      dispatch({ type: "ERROR", error });
    };

    userManager.events.addUserLoaded(handleUserLoaded);
    userManager.events.addUserUnloaded(handleUserUnloaded);
    userManager.events.addUserSignedOut(handleUserSignedOut);
    userManager.events.addSilentRenewError(handleSilentRenewError);

    return () => {
      userManager.events.removeUserLoaded(handleUserLoaded);
      userManager.events.removeUserUnloaded(handleUserUnloaded);
      userManager.events.removeUserSignedOut(handleUserSignedOut);
      userManager.events.removeSilentRenewError(handleSilentRenewError);
    };
  }, [userManager, navigate]);

  const removeUser = useCallback(async () => {
    if (!userManager) unsupported("removeUser");
    await userManager.removeUser();
    onRemoveUser && (await onRemoveUser());
  }, [userManager, onRemoveUser]);

  const authContextValue = useMemo(() => {
    return {
      settings: userManager.settings,
      events: userManager.events,

      getAccessToken: async () => {
        const user = await userManager.getUser();
        return user ? user.access_token || "" : "";
      },

      //
      ...(Object.fromEntries(
        userManagerContextKeys.map((key) => {
          return [key, userManager[key]?.bind(userManager) ?? unsupported(key)];
        })
      ) as Pick<UserManager, (typeof userManagerContextKeys)[number]>),
      //
      ...(Object.fromEntries(
        navigatorKeys.map((key) => {
          const func = userManager[key]
            ? async (args: ProcessResourceOwnerPasswordCredentialsArgs & string & never[]) => {
                dispatch({ type: "NAVIGATOR_INIT", method: key });
                try {
                  return await userManager[key](args);
                } catch (error) {
                  dispatch({ type: "ERROR", error: error as Error });
                  return null;
                } finally {
                  dispatch({ type: "NAVIGATOR_CLOSE" });
                }
              }
            : unsupported(key);

          return [key, func as any];
        })
      ) as Pick<UserManager, (typeof navigatorKeys)[number]>),
    } as AuthContextValue;
  }, [userManager]);

  return <AuthContext.Provider value={{ ...authContextValue, ...state, removeUser }}>{children}</AuthContext.Provider>;
};

AuthProvider.diplayName = "AuthProvider";
export default AuthProvider;
