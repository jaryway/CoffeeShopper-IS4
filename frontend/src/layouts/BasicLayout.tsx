import { Layout, ConfigProvider, Menu, theme, Button, Avatar } from "antd";
import type { MenuProps } from "antd";
import {
  AppstoreOutlined,
  ContainerOutlined,
  MailOutlined,
  PieChartOutlined,
  DesktopOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
} from "@ant-design/icons";
import { Outlet, useNavigate } from "react-router-dom";
import { useState } from "react";
import "./BasicLayout.css";
import UserInfo from "component/user-info";

const { Header, Footer, Sider } = Layout;

const BasicLayout = () => {
  const items: MenuProps["items"] = [
    { key: "1", icon: <PieChartOutlined />, label: "动态对象管理" },
    { key: "2", icon: <DesktopOutlined />, label: "Option 2" },
    { key: "3", icon: <ContainerOutlined />, label: "Option 3" },
    {
      key: "sub1",
      label: "Navigation One",
      icon: <MailOutlined />,
      children: [
        { key: "5", label: "Option 5" },
        { key: "6", label: "Option 6" },
        { key: "7", label: "Option 7" },
        { key: "8", label: "Option 8" },
      ],
    },
    {
      key: "sub2",
      label: "Navigation Two",
      icon: <AppstoreOutlined />,
      children: [
        { key: "9", label: "Option 9" },
        { key: "10", label: "Option 10" },
        {
          key: "sub3",
          label: "Submenu",
          children: [
            { key: "11", label: "Option 11" },
            { key: "12", label: "Option 12" },
          ],
        },
      ],
    },
  ];

  const [collapsed, handleSetCollapsed] = useState(false);
  const navigate = useNavigate();

  return (
    <ConfigProvider
      theme={{
        algorithm: theme.compactAlgorithm,
        token: {
          borderRadius: 1,
        },
        components: {
          // Slider: {
          //   siderBg: "white",
          // },
          Layout: {
            headerHeight: 48,
            headerPadding: "0 16px",
            headerBg: "#1677ff",
            siderBg: "white",
          },
          Menu: {
            itemPaddingInline: 4,
            itemHeight: 32,
          },
        },
      }}
    >
      <Layout style={{ display: "flex", flexDirection: "column", height: "100%", width: "100%", overflow: "hidden" }}>
        <Header
          style={{
            flex: "none",
            justifyContent: "space-between",
            display: "flex",
          }}
        >
          <div>DynamicSpace</div>

          <div>
            <UserInfo className="global-header-action" />
          </div>
        </Header>
        <Layout>
          <Sider width={200} trigger={null} collapsible collapsed={collapsed} collapsedWidth={48}>
            <div className="global-sider">
              <div className="global-sider-hd">
                <Button
                  size="small"
                  icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
                  onClick={() => {
                    handleSetCollapsed((prev) => (prev = !prev));
                  }}
                />
              </div>
              <div className="global-menu">
                <div className="global-menu-bd">
                  <div className="global-menu-inner" style={{ width: collapsed ? "48px" : "200px" }}>
                    <Menu
                      inlineIndent={8}
                      theme="light"
                      mode="inline"
                      defaultSelectedKeys={["4"]}
                      items={items}
                      style={{ borderRight: 0 }}
                      onClick={() => {
                        navigate("/dynamic-object-management");
                      }}
                    />
                  </div>
                </div>
              </div>
              {/* <div className="global-sider-ft" style={{ flex: "none" }}>footer</div> */}
            </div>
          </Sider>
          <Layout style={{ overflow: "auto" }}>
            <Outlet />
            <Footer className="footer" style={{ textAlign: "center" }}>
              版权所有 © 2023
            </Footer>
          </Layout>
        </Layout>
      </Layout>
    </ConfigProvider>
  );
};

export default BasicLayout;
