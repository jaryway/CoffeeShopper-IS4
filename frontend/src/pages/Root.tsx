import { ConfigProvider, Empty, Layout } from "antd";
import zhCN from "antd/es/locale/zh_CN";
import { Outlet } from "react-router-dom";

const Root = () => {
  console.log("Root");

  return (
    <ConfigProvider
      locale={zhCN}
      prefixCls="cssc"
      iconPrefixCls="csscicon"
      componentSize="small"
      input={{ autoComplete: "off" }}
      renderEmpty={(_componentName) => {
        return <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />;
      }}
      space={{ size: "small" }}
    >
      <Outlet />
    </ConfigProvider>
  );
};


export default Root;
