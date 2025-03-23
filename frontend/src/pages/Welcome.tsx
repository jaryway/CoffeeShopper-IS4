import { MessageOutlined, SafetyOutlined } from "@ant-design/icons";
import { Button, Result } from "antd";
import React from "react";

const Welcome = () => {
  console.log("Welcome");

  return (
    <div className="content" style={{ flex: "auto", padding: "0 16px" }}>
      <Result
        // icon={<div style={{ margin: "0 auto", width: 100, height: 100 }}></div>}
        icon={<SafetyOutlined />}
        status="success"
        title="欢迎你，我的朋友"
        // subTitle="Sorry, you are not authorized to access this page."
        extra={
          <Button
            type="primary"
            onClick={() => {
              // console.log();/
            }}
          >
            去登录
          </Button>
        }
      />
    </div>
  );
};

export default Welcome;
