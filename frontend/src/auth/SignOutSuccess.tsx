import { Button, Result } from "antd";
import React, { useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "./use-auth";
// import { settings } from "oidc/settings";
// import { UserManager } from "oidc-client-ts";

// const mgr = new UserManager(settings);

const SignOut = () => {
  // const {s}= useAuth()
  // let flag = useRef(false).current;
  const navigate = useNavigate();

  // useEffect(() => {
  //   if (flag) return;

  //   flag = true;

  //   mgr.events.addUserSignedOut(() => {
  //     console.log("addUserSignedOut:用户已登出");
  //   });
  //   mgr.events.addUserUnloaded(() => {
  //     console.log("addUserUnloaded:用户已登出");
  //   });

  //   mgr
  //     .signoutRedirectCallback() //
  //     .then(() => {
  //       // mgr.removeUser().then(() => {
  //       //   console.log("已经移除用户登录信息");
  //       // });
  //       console.log("你已经成功登出");
  //     })
  //     .finally(() => {
  //       flag = false;
  //     });
  // }, []);

  return (
    <Result
      status="success"
      title="你已成功登出系统"
      extra={
        <Button
          type="primary"
          key="console"
          onClick={() => {
            navigate("/");
          }}
        >
          去登录
        </Button>
      }
    />
  );
};

export default SignOut;
