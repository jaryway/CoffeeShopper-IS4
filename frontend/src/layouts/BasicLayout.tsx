import { Layout, ConfigProvider, Menu } from "antd";
import type { MenuProps } from "antd";
import {
  AppstoreOutlined,
  BarChartOutlined,
  CloudOutlined,
  ShopOutlined,
  TeamOutlined,
  UploadOutlined,
  UserOutlined,
  VideoCameraOutlined,
} from "@ant-design/icons";
import React from "react";
import { Outlet } from "react-router-dom";

const { Header, Content, Footer, Sider } = Layout;

const BasicLayout = () => {
  const items: MenuProps["items"] = [
    UserOutlined,
    VideoCameraOutlined,
    UploadOutlined,
    BarChartOutlined,
    CloudOutlined,
    AppstoreOutlined,
    TeamOutlined,
    ShopOutlined,
  ].map((icon, index) => ({
    key: String(index + 1),
    icon: React.createElement(icon),
    label: `nav ${index + 1}`,
  }));
  return (
    <ConfigProvider
      theme={{
        components: {
          Layout: {
            headerBg: "#1677ff",
          },
        },
      }}
    >
      <Layout style={{ display: "flex", flexDirection: "column", height: "100%", width: "100%", overflow: "hidden" }}>
        <Header
          style={{
            flex: "none",
          }}
        >
          header
        </Header>
        <Layout style={{ flex: 1, display: "flex" }}>
          <Sider style={{ width: "240px" }}>
            <div className="demo-logo-vertical" />
            <Menu theme="dark" mode="inline" defaultSelectedKeys={["4"]} items={items} />
          </Sider>
          <Layout style={{ overflow: "auto" }}>
            <div className="content-wrapper" style={{ flex: "auto", height: 0 }}>
              <Content style={{ display: "flex", overflow: "auto", border: "1px solid red" }}>
                <Outlet />
              </Content>
              <Footer style={{ flex: "none" }}>footer</Footer>
            </div>
          </Layout>
        </Layout>
      </Layout>
    </ConfigProvider>
  );
};

export default BasicLayout;
