import { createContext } from "react";
import type {
  UserManagerSettings,
  UserManagerEvents,
  User,
  SessionStatus,
  SigninPopupArgs,
  SigninSilentArgs,
  SigninRedirectArgs,
  SignoutRedirectArgs,
  SignoutPopupArgs,
  QuerySessionStatusArgs,
  RevokeTokensTypes,
  SignoutSilentArgs,
  SigninResourceOwnerCredentialsArgs,
} from "oidc-client-ts";
import { AuthState } from "./AuthState";

export interface AuthContextValue extends AuthState {
  /**
   * UserManager functions. See [UserManager](https://github.com/authts/oidc-client-ts) for more details.
   */
  readonly settings: UserManagerSettings;
  readonly events: UserManagerEvents;
  clearStaleState(): Promise<void>;
  getUser(): Promise<User | null>;
  getAccessToken(): Promise<string>;
  removeUser(): Promise<void>;
  signinPopup(args?: SigninPopupArgs): Promise<User>;
  signinSilent(args?: SigninSilentArgs): Promise<User | null>;
  signinRedirect(args?: SigninRedirectArgs): Promise<void>;
  // signinCallback(url?: string): Promise<User | void>;
  signinResourceOwnerCredentials(args: SigninResourceOwnerCredentialsArgs): Promise<User>;
  signoutRedirect(args?: SignoutRedirectArgs): Promise<void>;
  signoutPopup(args?: SignoutPopupArgs): Promise<void>;
  signoutSilent(args?: SignoutSilentArgs): Promise<void>;
  querySessionStatus(args?: QuerySessionStatusArgs): Promise<SessionStatus | null>;
  revokeTokens(types?: RevokeTokensTypes): Promise<void>;
  startSilentRenew(): void;
  stopSilentRenew(): void;
}

/**
 * @public
 */
export const AuthContext = createContext<AuthContextValue | undefined>(undefined);
AuthContext.displayName = "AuthContext";
