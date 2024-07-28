/* eslint-disable no-unused-vars */
import { DropDownProps } from "antd/es/dropdown";
import { Dropdown } from "antd";
import React from "react";
// import classNames from 'classnames';
// import './style';

// declare type OverlayFunc = () => React.ReactNode;

export interface HeaderDropdownProps extends DropDownProps {
  overlayClassName?: string;
  // overlay: React.ReactElement | OverlayFunc;
  // placement?: 'bottomLeft' | 'bottomRight' | 'topLeft' | 'topCenter' | 'topRight' | 'bottomCenter';
}

const HeaderDropdown: React.FC<HeaderDropdownProps> = ({ overlayClassName: cls, ...restProps }) => (
  <Dropdown overlayClassName={["header-dropdown", cls].join(" ")} {...restProps} />
);

export default HeaderDropdown;
