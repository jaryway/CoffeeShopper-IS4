import { Layout } from "antd";
const { Header, Content, Footer, Sider } = Layout;

const BasicLayout = () => {
  return (
    <Layout>
      <Header>header</Header>
      <Layout>
        <Sider>left sidebar</Sider>
        <Content>main content</Content>
        <Sider>right sidebar</Sider>
      </Layout>
      <Footer>footer</Footer>
    </Layout>
  );
};

export default BasicLayout;
