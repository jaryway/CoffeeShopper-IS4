import { Table, message } from "antd";
import React, { useEffect, useRef, useState } from "react";
import { useRequest } from "ahooks";

const Management = () => {
//   const flag = useRef(false).current;

  async function getDynamicObjects(keywords = "") {
    return new Promise<any[]>((resolve) => {
      setTimeout(() => {
        resolve([]);
      }, 1000);
    });
  }

  const { data: dataSource, loading } = useRequest(getDynamicObjects, {
    onSuccess: (result, params) => {
      //   setState("");
      message.success(`The username was changed to "${params[0]}"!`);
    },
    onError: (error) => {
      message.error(error.message);
    },
  });

  const columns = [
    {
      title: "姓名000",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "年龄",
      dataIndex: "age",
      key: "age",
    },
    {
      title: "住址",
      dataIndex: "address",
      key: "address",
    },
    {
      title: "住址1",
      dataIndex: "address",
      key: "address1",
    },
    {
      title: "住址1",
      dataIndex: "address",
      key: "address1",
    },
    {
      title: "住址2",
      dataIndex: "address",
      key: "address2",
    },
    {
      title: "住址3",
      dataIndex: "address",
      key: "address3",
    },
    {
      title: "住址4",
      dataIndex: "address",
      key: "address4",
    },
    {
      title: "住址5",
      dataIndex: "address",
      key: "address5",
    },
    {
      title: "住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6",
      dataIndex: "address",
      key: "address6",
      width: 200,
    },
    {
      title: "住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6",
      dataIndex: "address",
      key: "address7",
      width: 200,
    },
    {
      title: "住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6",
      dataIndex: "address",
      key: "address8",
      width: 200,
    },
    {
      title: "住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6",
      dataIndex: "address",
      key: "address9",
      width: 200,
    },
    {
      title: "住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6",
      dataIndex: "address",
      key: "address10",
      width: 200,
    },
    {
      title: "住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6住址6",
      dataIndex: "address",
      key: "address11",
      width: 200,
    },
  ];

  return (
    <div className="content" style={{ flex: "auto", padding: "0 16px" }}>
      <div style={{ padding: "16px 0" }}>人员管理</div>
      <Table dataSource={dataSource} columns={columns} loading={loading} scroll={{ x: 3000 }} />
    </div>
  );
};

export default Management;
