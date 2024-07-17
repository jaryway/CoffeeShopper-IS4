import { Button, Result } from "antd";
import React from "react";
import { useNavigate } from "react-router-dom";

const SignOut = () => {
  const navigate = useNavigate();
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
