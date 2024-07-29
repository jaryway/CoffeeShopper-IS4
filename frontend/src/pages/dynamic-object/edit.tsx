import React, { useState } from "react";
import { Button, Checkbox, Form, Input, Select, Modal, InputNumber, message } from "antd";
import { MinusCircleOutlined, PlusOutlined } from "@ant-design/icons";
import { useApi } from "api";
const { Option } = Select;

const Edit = (props: { record: any; open: boolean; onCancel?: () => void; onOk?: () => void }) => {
  const { record, open, onCancel, onOk } = props;
  const [form] = Form.useForm();
  const api = useApi();
  const [loading, setLoading] = useState(false);

  const submit = async (data: any) => {
    try {
      setLoading(true);
      await api(`http://localhost:5003/api/Runtime/${data.id ? "Update/" + data.id : "Create"}`, {
        method: data.id ? "PUT" : "POST",
        data,
      });

      onOk && onOk();
      form.resetFields();
      message.success("操作成功");
    } catch (error) {
      message.error("操作失败");
    } finally {
      setLoading(false);
    }
  };

  const onFinish = (values: any) => {
    console.log("values", values);
    submit({ ...record, ...values });
  };

  return (
    <Modal
      open={open}
      title={`新建动态对象`}
      // footer={null}
      onClose={() => {
        console.log("onClose");
        // setOpen(false);
      }}
      onCancel={() => {
        console.log("onCancel");
        onCancel && onCancel();
      }}
      width={720}
      onOk={() => {
        form.submit();
      }}
      confirmLoading={loading}
    >
      {open && (
        <Form form={form} layout="vertical" onFinish={onFinish}>
          <Form.Item name={"name"} label="名称" required>
            <Input />
          </Form.Item>
          <Form.Item name={"tableName"} label="表名" required>
            <Input />
          </Form.Item>
          <Form.Item label="字段" required>
            <Form.List
              name="json"
              rules={[
                {
                  validator: async (_, fields) => {
                    console.log("json", fields);

                    if (!fields || fields.length < 1) {
                      return Promise.reject(new Error("请至少填写一个字段"));
                    }
                  },
                },
              ]}
            >
              {(fields, { add, remove }, { errors }) => (
                <>
                  {fields.length > 0 && (
                    <table style={{ width: "100%", borderCollapse: "collapse", marginBottom: 8 }}>
                      <thead>
                        <tr>
                          <th style={{ border: "solid 1px #ccc", padding: 4, height: 24, textAlign: "left" }}>名称</th>
                          <th style={{ border: "solid 1px #ccc", padding: 4, textAlign: "left", width: 140 }}>数据类型</th>
                          <th style={{ border: "solid 1px #ccc", padding: 4, textAlign: "left" }}>长度</th>
                          <th style={{ border: "solid 1px #ccc", padding: 4, textAlign: "left" }}>默认值</th>
                          <th style={{ border: "solid 1px #ccc", padding: 4, textAlign: "left", width: 24 }}>非空</th>
                          <th style={{ border: "solid 1px #ccc", padding: 4, textAlign: "left", width: 24 }}>唯一</th>
                          <th style={{ border: "solid 1px #ccc", padding: 4, textAlign: "left", width: 24 }}></th>
                        </tr>
                      </thead>
                      <tbody>
                        {fields.map(({ key, name, ...rest }, index) => {
                          console.log("field", rest, key, name, index);

                          return (
                            <tr key={key}>
                              <td style={{ border: "solid 1px #ccc", padding: 4 }}>
                                <Form.Item
                                  {...rest}
                                  name={[name, "name"]}
                                  validateTrigger={["onChange", "onBlur"]}
                                  rules={[
                                    {
                                      required: true,
                                      // whitespace: true,
                                      message: "请输入名称",
                                    },
                                  ]}
                                  noStyle
                                >
                                  <Input placeholder="请输入" style={{ width: "100%" }} />
                                </Form.Item>
                              </td>
                              <td style={{ border: "solid 1px #ccc", padding: 4 }}>
                                <Form.Item
                                  {...rest}
                                  name={[name, "dataType"]}
                                  validateTrigger={["onChange", "onBlur"]}
                                  rules={[
                                    {
                                      required: true,
                                      // whitespace: true,
                                      message: "请选择数据类型",
                                    },
                                  ]}
                                  noStyle
                                >
                                  <Select placeholder="请输入" style={{ width: "100%" }}>
                                    <Option value={1}>TEXT</Option>
                                    <Option value={2}>INTEGER</Option>
                                    <Option value={3}>NUMERIC</Option>
                                  </Select>
                                </Form.Item>
                              </td>
                              <td style={{ border: "solid 1px #ccc", padding: 4 }}>
                                <Form.Item
                                  {...rest}
                                  name={[name, "length"]}
                                  validateTrigger={["onChange", "onBlur"]}
                                  rules={[
                                    {
                                      // required: true,
                                      // whitespace: true,
                                      // message: "Please input passenger's name or delete this field.",
                                    },
                                  ]}
                                  noStyle
                                >
                                  <InputNumber placeholder="请输入" style={{ width: "100%" }} />
                                </Form.Item>
                              </td>
                              <td style={{ border: "solid 1px #ccc", padding: 4 }}>
                                <Form.Item
                                  {...rest}
                                  name={[name, "defaultValue"]}
                                  validateTrigger={["onChange", "onBlur"]}
                                  rules={[
                                    {
                                      // required: true,
                                      // whitespace: true,
                                      // message: "Please input passenger's name or delete this field.",
                                    },
                                  ]}
                                  noStyle
                                >
                                  <Input placeholder="请输入" style={{ width: "100%" }} />
                                </Form.Item>
                              </td>
                              <td style={{ border: "solid 1px #ccc", padding: 4, textAlign: "center" }}>
                                <Form.Item
                                  {...rest}
                                  name={[name, "notNull"]}
                                  validateTrigger={["onChange", "onBlur"]}
                                  noStyle
                                  valuePropName="checked"
                                >
                                  <Checkbox />
                                </Form.Item>
                              </td>
                              <td style={{ border: "solid 1px #ccc", padding: 4, textAlign: "center" }}>
                                <Form.Item
                                  {...rest}
                                  name={[name, "unique"]}
                                  validateTrigger={["onChange", "onBlur"]}
                                  noStyle
                                  valuePropName="checked"
                                >
                                  <Checkbox />
                                </Form.Item>
                              </td>
                              <td style={{ border: "solid 1px #ccc", padding: 4, textAlign: "center" }}>
                                <MinusCircleOutlined className="dynamic-delete-button" onClick={() => remove(name)} />
                              </td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  )}
                  <Form.Item>
                    <Button onClick={() => add()} icon={<PlusOutlined />}>
                      添加字段
                    </Button>
                    <Form.ErrorList errors={errors} />
                  </Form.Item>
                </>
              )}
            </Form.List>
          </Form.Item>
        </Form>
      )}
    </Modal>
  );
};

export default Edit;
