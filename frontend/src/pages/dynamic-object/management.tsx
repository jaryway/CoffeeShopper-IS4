import { Button, FormInstance, Modal, Popconfirm, message } from "antd";
import { useRef, useState } from "react";
import { useApi } from "api";
import { type ActionType, ProColumns, ProTable, type ProTableProps } from "@ant-design/pro-table";
import { CheckCircleOutlined, PlusOutlined, CheckCircleTwoTone } from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { useRequest } from "ahooks";

const Management = () => {
  //   const flag = useRef(false).current;

  const navigate = useNavigate();
  const api = useApi();
  const actionRef = useRef<ActionType>();
  const formRef = useRef<FormInstance>();
  const [open, setOpen] = useState(false);
  const [record, setRecord] = useState<any>();

  const request: ProTableProps<any, any>["request"] = () => {
    return api.get<any[]>("http://localhost:5003/api/Runtime/Query").then((resp) => {
      console.log("resp", resp.data);

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

  const { loading, runAsync: publish } = useRequest(publishRequest, { manual: true });

  const submit = async (data: any) => {
    return api(`http://localhost:5003/api/Runtime/${data.id ? "Update/" + data.id : "Create"}`, {
      method: data.id ? "PUT" : "POST",
      data,
    })
      .then((resp) => {
        if (resp.data) {
          actionRef.current?.reload();
          setOpen(false);
          setRecord({});
        }
      })
      .catch(() => {
        message.error("操作成功");
      });

    // return resp;

    // return api.get<any[]>("http://localhost:5003/api/Runtime/Query").then((resp) => resp.data);
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
      search: false,
    },

    {
      title: "是否发布",
      dataIndex: "published",
      key: "published",
      hideInForm: true,
      render: (text) => (text ? <CheckCircleTwoTone twoToneColor="#52c41a" /> : <CheckCircleOutlined color="error" />),
    },
    {
      title: "属性发生变化",
      dataIndex: "entityPropertiesHasChanged",
      key: "entityPropertiesHasChanged",
      hideInForm: true,
      search: false,
      valueType: "text",
      render: (text) => (text ? "是" : "否"),
    },
    {
      title: "属性",
      dataIndex: "entityProperties",
      valueType: "textarea",
      key: "entityProperties",
      search: false,
      hideInTable: true,
    },
    {
      title: "描述",
      dataIndex: "description",
      key: "description",
      search: false,
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
            // action?.startEditable?.(record.id);
            // navigate("/dynamic-object/edit/0");
            const { entityProperties_, ...rest } = record;

            setRecord({ ...rest, entityProperties: entityProperties_ });
            setOpen(true);
            // setTimeout(() => {
            //   console.log("rest", formRef.current);
            //   // formRef.current?.resetFields({ ...rest, entityProperties: entityProperties_ });
            //   // formRef.current?.setFieldsValue({ ...rest, entityProperties: entityProperties_ });
            // }, 100);
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
          <Popconfirm title="您确定要发布吗" onConfirm={() => publish()}>
            <Button key="generate" loading={loading}>
              发布
            </Button>
          </Popconfirm>,
        ]}
        pagination={false}
        request={request}
        columns={columns}
        // scroll={{ x: 3000 }}
      />
      <Modal
        open={open}
        title="新建动态对象"
        footer={null}
        onClose={() => {
          console.log("onClose");
          setOpen(false);
        }}
        onCancel={() => {
          console.log("onCancel");
          setOpen(false);
        }}
      >
        <ProTable
          key={record?.id ? "edit" + record.id : "create"}
          type="form"
          columns={columns}
          formRef={formRef}
          form={{ initialValues: record }}
          onSubmit={(values) => {
            submit({ ...record, ...values });
          }}
        />
      </Modal>
    </div>
  );
};

export default Management;
