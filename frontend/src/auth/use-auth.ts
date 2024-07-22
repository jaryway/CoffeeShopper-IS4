import { useContext } from "react";
import { AuthContext, AuthContextValue } from "./AuthContext";

export const useAuth = (): AuthContextValue => {
  const context = useContext(AuthContext);

  if (!context) {
    console.warn("AuthProvider context is undefined, please verify you are calling useAuth() as child of a <AuthProvider> component.");
  }

  return context as AuthContextValue;
};
