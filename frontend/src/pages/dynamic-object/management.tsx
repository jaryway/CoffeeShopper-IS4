/* eslint-disable jsx-a11y/anchor-is-valid */
import { Alert, Button, Popconfirm, message } from "antd";
import { useRef, useState } from "react";
import { useApi } from "api";
import { type ActionType, ProColumns, ProTable, type ProTableProps } from "@ant-design/pro-table";
import { CheckCircleOutlined, PlusOutlined, CheckCircleTwoTone } from "@ant-design/icons";
import { useRequest } from "ahooks";
import { ActionLink } from "components/action";
import Edit from "./Edit";

const Management = () => {
  //   const flag = useRef(false).current;

  // const navigate = useNavigate();
  const api = useApi();
  const actionRef = useRef<ActionType>();
  const [open, setOpen] = useState(false);
  const [record, setRecord] = useState<any>();
  const references = useRef<string[]>([]);

  const request: ProTableProps<any, any>["request"] = () => {
    return api.get<any[]>("http://localhost:5003/api/Runtime/Query").then((resp) => {
      console.log("resp", resp.data);

      references.current = resp.data.map((m) => m.name);

      return {
        data: resp.data,
        success: true,
      };
    });
  };
  const publishRequest = () => {
    return api.post<any[]>("http://localhost:5003/api/Runtime/Generate").then((resp) => {
      console.log("publish", resp.data);
      return {
        data: resp.data,
        // success: true,
      };
    });
  };

  const { loading, runAsync: publish } = useRequest(publishRequest, {
    manual: true, //
    onSuccess: () => {
      actionRef.current?.reload();
    },
    onError() {
      message.error("生成失败");
    },
  });

  const columns: ProColumns<any>[] = [
    {
      title: "名称",
      dataIndex: "name",
      key: "name",
      formItemProps: {
        required: true,
      },
    },
    {
      title: "表名",
      dataIndex: "tableName",
      key: "tableName",
      formItemProps: {
        required: true,
      },
      search: false,
    },

    {
      title: "是否生成",
      dataIndex: "published",
      key: "published",
      hideInForm: true,
      search: false,
      render: (text) => (text ? <CheckCircleTwoTone twoToneColor="#52c41a" /> : <CheckCircleOutlined />),
    },
    {
      title: "属性有更新",
      dataIndex: "entityPropertiesHasChanged",
      key: "entityPropertiesHasChanged",
      hideInForm: true,
      search: false,
      valueType: "text",
      render: (text) => (text ? <CheckCircleTwoTone twoToneColor="#52c41a" /> : <CheckCircleOutlined />),
    },
    {
      title: "属性",
      dataIndex: "entityProperties",
      valueType: "textarea",
      key: "entityProperties",
      search: false,
      hideInTable: true,
      formItemProps: {
        required: true,
      },
    },
    {
      title: "属性1",
      dataIndex: "json",
      valueType: "formList",
      key: "json",
      search: false,
      hideInTable: true,
      formItemProps: {
        required: true,
      },
      renderFormItem: () => {
        return <div>sdf</div>;
      },
    },
    { title: "描述", dataIndex: "description", key: "description", search: false },
    {
      title: "操作",
      valueType: "option",
      key: "option",
      render: (text, record, _, action) => [
        <a
          key="editable"
          onClick={() => {
            const { entityProperties_, json, ...rest } = record;
            setRecord({ ...rest, entityProperties: entityProperties_, json: JSON.parse(json || "[]") });
            setOpen(true);
          }}
        >
          编辑
        </a>,

        <ActionLink
          key="remove"
          url={`http://localhost:5003/api/Runtime/Delete/${record.id}`}
          onSuccess={() => actionRef.current?.reload()}
          text="删除"
        />,
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
        rowKey={"id"}
        toolBarRender={() => [
          <Button
            key="button"
            icon={<PlusOutlined />}
            onClick={() => {
              setRecord({});
              setOpen(true);
            }}
            type="primary"
          >
            新建
          </Button>,
          <Popconfirm
            title="您确定要生成吗"
            onConfirm={() => {
              publish();
            }}
          >
            <Button key="generate" loading={loading}>
              生成
            </Button>
          </Popconfirm>,
        ]}
        pagination={false}
        request={request}
        columns={columns}
        // scroll={{ x: 3000 }}
      />
      <Alert style={{ marginTop: 16 }} message={<p>新建对象或更改属性后需要重新生成，才会生效。</p>} type="info" />
      {/* <Modal
        open={open}
        title={`新建动态对象`}
        footer={null}
        onClose={() => {
          console.log("onClose");
          setOpen(false);
        }}
        onCancel={() => {
          console.log("onCancel");
          setOpen(false);
        }}
        width={720}
      >
        

      </Modal> */}

      <Edit
        open={open}
        record={record}
        references={(references.current || []).filter((m) => m !== record?.name)}
        onCancel={() => {
          setOpen(false);
        }}
        onOk={() => {
          setOpen(false);
        }}
      />
    </div>
  );
};

export default Management;
