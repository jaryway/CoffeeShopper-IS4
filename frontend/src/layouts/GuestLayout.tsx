import { Layout, ConfigProvider, theme, Button } from "antd";
import { Outlet } from "react-router-dom";
import "./GuestLayout.css";
import { useAuth } from "auth/use-auth";

const { Header, Footer } = Layout;

const GuestLayout = () => {
  // const navigate = useNavigate();
  const auth = useAuth();

  return (
    <ConfigProvider
      theme={{
        algorithm: theme.compactAlgorithm,
        token: {
          borderRadius: 1,
        },
        components: {
          Layout: {
            headerHeight: 48,
            headerPadding: "0 16px",
            headerColor: "white",
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
          <div className="global-header-left">DynamicSpace</div>
          <div className="global-header-right" style={{ display: "flex", alignItems: "center", lineHeight: 1 }}>
            {/* <UserInfo className="global-header-action" /> */}

            <div className="global-header-action">
              <a
                href="/login"
                onClick={(e) => {
                  e.preventDefault();
                  auth.signinRedirect();
                }}
              >
                <span>登录</span>
              </a>
            </div>
          </div>
        </Header>
        <Layout>
          <Outlet />
          <Footer className="footer" style={{ textAlign: "center" }}>
            版权所有 © 2023
          </Footer>
        </Layout>
      </Layout>
    </ConfigProvider>
  );
};

export default GuestLayout;
