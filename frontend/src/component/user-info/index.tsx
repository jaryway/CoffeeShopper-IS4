/* eslint-disable jsx-a11y/alt-text */
import { useCallback, useState } from "react";
import { useNavigate } from "react-router-dom";
// import classNames from "classnames";
import { KeyOutlined, PoweroffOutlined, InfoCircleOutlined, UserOutlined } from "@ant-design/icons";
import { Avatar, Menu, MenuProps, Spin } from "antd";

import HeaderDropdown from "../header-dropdown";
import { UserManager } from "oidc-client-ts";
import { settings } from "oidc/settings";
import { useCurrentUser } from "hooks/use-current-user";
// import { useLogout, useCurrentUser, useAPI } from "@fregata/shared";
// import ChangePassword from "pages/accounts/ChangePassword";
// import Profile from "pages/accounts/Profile";

// const styles: any = {};

export default function UserInfo({ ...rest }: any) {
  // const userManager = useUserManager();
  //   const { api } = useAPI();
  //   const logoutUser = useLogout();
  const { currentUser, logout } = useCurrentUser();

  console.log("loading", currentUser);

  //   const currentUser = { userName: "小明", headImg: "https://gw.alipayobjects.com/zos/antfincdn/XAosXuNZyF/BiazfanxmamNRoxxVxka.png" };
  const { headImg, name } = currentUser || {};
  const [passwordVisible, setPasswordVisible] = useState(false);
  const [profileVisible, setProfileVisible] = useState(false);
  const navigate = useNavigate();
  //   const logout = useCallback(async () => {
  //     // try {
  //     //   await api.post("/auth/api/index/logout");
  //     // } catch (ex) {
  //     //   console.error("调用服务器端注销接口出错");
  //     // }
  //     // // 不管是否成功，都删除本地登录信息
  //     // logoutUser();
  //     // localStorage.removeItem("hiddenHeader");
  //     // localStorage.removeItem("hiddenSider");
  //   }, []);

  const _onPasswordVisibleChange = useCallback(() => {
    setPasswordVisible((prev) => !prev);
  }, []);

  const _onProfileVisibleChange = useCallback(() => {
    setProfileVisible((prev) => !prev);
  }, []);

  const _onMenuClick = useCallback(
    ({ key }: any) => {
      switch (key) {
        case "logout":
          return logout();
        case "changepwd":
          _onPasswordVisibleChange();
          break;
        case "profile":
          _onProfileVisibleChange();
          break;
        case "follows":
          navigate("/follows");
          break;

        default:
          break;
      }
    },
    [_onPasswordVisibleChange, _onProfileVisibleChange, logout, navigate]
  );

  var mgr = new UserManager(settings);
  mgr.getUser().then((user) => console.log(user));

  const items: MenuProps["items"] = [
    {
      icon: <InfoCircleOutlined />,
      key: "profile",
      label: "个人信息",
    },
    {
      icon: <KeyOutlined />,
      key: "changepwd",
      label: "修改密码",
    },
    {
      key: "d0",
      type: "divider",
    },
    {
      icon: <PoweroffOutlined />,
      key: "logout",
      label: "注销登录",
    },
  ];

  console.log("currentUser", currentUser, name);

  return (
    <>
      {currentUser && name ? (
        <HeaderDropdown menu={{ style: { minWidth: 100 }, onClick: _onMenuClick, items }}>
          <div className={rest.className}>
            {headImg ? (
              <Avatar className="avatar" src={headImg} alt="avatar" />
            ) : (
              //   <Avatar className="avatar" icon={<img src={`/resimages/gfyz/guangxi_${sex === 0 ? "femalehead" : "malehead"}.png`} />} />
              <Avatar className="avatar" icon={<UserOutlined />} />
            )}
            <span className="name">{name}</span>
          </div>
        </HeaderDropdown>
      ) : (
        <Spin className="user-spin" style={{ marginLeft: 8, marginRight: 8 }} />
      )}

      {/* <Profile visible={profileVisible} id={id} onCancel={_onProfileVisibleChange} onOk={_onProfileVisibleChange} /> */}
      {/* <ChangePassword visible={passwordVisible} id={id} onCancel={_onPasswordVisibleChange} onOk={_onPasswordVisibleChange} /> */}
    </>
  );
}
