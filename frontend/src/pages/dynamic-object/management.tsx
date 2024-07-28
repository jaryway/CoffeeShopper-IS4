import { Button, message } from "antd";
import React, { useEffect, useRef, useState } from "react";
import { useRequest } from "ahooks";
import { useApi } from "api";
import { type ActionType, ProColumns, ProTable, type ProTableProps } from "@ant-design/pro-table";
import { PlusOutlined } from "@ant-design/icons";

const Management = () => {
  //   const flag = useRef(false).current;

  const api = useApi();
  const actionRef = useRef<ActionType>();

  const request: ProTableProps<any, any>["request"] = () => {
    return api.get<any[]>("http://localhost:5003/api/Runtime/Query").then((resp) => resp.data);
  };

  // const { data: dataSource, loading } = useRequest(getDynamicObjects, {
  //   onSuccess: (result, params) => {
  //     //   setState("");
  //     // message.success(`The username was changed to "${params[0]}"!`);
  //   },
  //   onError: (error) => {
  //     message.error(error.message);
  //   },
  // });

  const columns: ProColumns<any>[] = [
    {
      title: "名称",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "表名",
      dataIndex: "tableName",
      key: "tableName",
    },
    {
      title: "是否发布",
      dataIndex: "published",
      key: "published",
    },
    {
      title: "描述",
      dataIndex: "description",
      key: "description",
    },

    {
      title: "操作",
      valueType: "option",
      key: "option",
      render: (text, record, _, action) => [
        // eslint-disable-next-line jsx-a11y/anchor-is-valid
        <a
          key="editable"
          onClick={() => {
            action?.startEditable?.(record.id);
          }}
        >
          编辑
        </a>,
        <a href={record.url} target="_blank" rel="noopener noreferrer" key="view">
          查看
        </a>,
        // <TableDropdown
        //   key="actionGroup"
        //   onSelect={() => action?.reload()}
        //   menus={[
        //     { key: 'copy', name: '复制' },
        //     { key: 'delete', name: '删除' },
        //   ]}
        // />,
      ],
    },
  ];

  return (
    <div className="content" style={{ flex: "auto", padding: "0 16px" }}>
      <div
        style={{
          padding: "16px",
          margin: "0 -16px",
          fontWeight: "600",
          fontSize: "16px",
          // backgroundColor: "#fff"
        }}
      >
        动态对象管理
      </div>
      <ProTable
        // headerTitle={"动态对象管理"}
        actionRef={actionRef}
        toolBarRender={() => [
          <Button
            key="button"
            icon={<PlusOutlined />}
            onClick={() => {
              // actionRef.current?.reload();
            }}
            type="primary"
          >
            新建
          </Button>,
        ]}
        pagination={false}
        request={request}
        columns={columns}
        scroll={{ x: 3000 }}
      />
    </div>
  );
};

export default Management;
