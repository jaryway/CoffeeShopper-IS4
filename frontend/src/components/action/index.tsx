import { message, Spin, Typography } from "antd";
import { useApi } from "api";
import { useState } from "react";
const { Link } = Typography;

export const ActionLink = ({ text, url, onSuccess }: { text: string; id?: string; url: string; onSuccess: () => void }) => {
  const api = useApi();
  const [loading, setLoading] = useState(false);

  const remove = async () => {
    try {
      setLoading(true);
      await api.delete(url);
      onSuccess && onSuccess();
      message.success("操作成功");
    } catch (error) {
      message.error("操作失败");
    } finally {
      setLoading(false);
    }
  };

  if (loading) return <Spin />;

  return (
    <Link key="delete" type="danger" onClick={() => remove()}>
      {text}
    </Link>
  );
};
