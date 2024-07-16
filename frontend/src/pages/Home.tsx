import { Table } from "antd";
import React from "react";

const Home = () => {
  console.log("Home");

  const dataSource = new Array(100).fill(null).map((_, index) => ({
    key: index,
    name: `Edward King ${index}`,
    age: 32,
    address: `London, Park Lane no. ${index}`,
  }));

  const columns = [
    {
      title: "姓名",
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
    <div style={{ flex: "auto", overflow: "auto", width: 0 }}>
      <Table dataSource={dataSource} columns={columns} style={{ width: "3000px" }} />
    </div>
  );
};

export default Home;
