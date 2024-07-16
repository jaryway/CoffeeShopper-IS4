import { UserManager } from "oidc-client-ts";
import React, { useEffect } from "react";
// http://localhost:4100/signin-oidc
// ?code=D89074FADF1E75AFAEF3B223EA957234CA339854CC8D1446F761FCACD8A53861
// &scope=openid%20profile%20CoffeeAPI.read%20offline_access
// &state=5efc22d508fe4f0aa368ec792d4f08f4
// &session_state=RwmsiFFydGldeAXqTyI6HLVhAcT0LMSTf6j4gWir-q0.824C25526220E2E11CB48C0EA47783C0
import { settings } from "./settings";
import { useNavigate } from "react-router-dom";
const mgr = new UserManager({
  ...settings,
  loadUserInfo: true,
  filterProtocolClaims: true,
  response_mode: "query",
});

let flag = false;
const signInCallback = async (cb: () => void) => {
  // console.log("signInCallback");

  if (flag) return;

  flag = true;
  if (window.location.search.length < 1) return;

  mgr
    .signinRedirectCallback()
    .then(cb)
    .catch((ex) => {
      console.log(ex);
    })
    .finally(() => {
      flag = false;
    });
};

const SignIn = () => {
  const navigate = useNavigate();

  useEffect(() => {
    signInCallback(() => navigate("/"));
  }, []);

  return <div>SignIn</div>;
};

export default SignIn;
