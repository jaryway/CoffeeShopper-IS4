// import { Loading3QuartersOutlined, LoadingOutlined } from '@ant-design/icons';
// import { Spin } from 'antd';
import './style.scss';

const Indicator = () => {
  return (
    <div className='indicator'>
      <div className='indicator-inner'>
        <svg height='100%' viewBox='0 0 32 32' width='100%'>
          <circle
            cx='16'
            cy='16'
            fill='none'
            r='14'
            strokeWidth='4'
            style={{ stroke: 'rgb(29, 155, 240)', opacity: 0.2 }}
          />
          <circle
            cx='16'
            cy='16'
            fill='none'
            r='14'
            strokeWidth='4'
            style={{ stroke: 'rgb(29, 155, 240)', strokeDasharray: 80, strokeDashoffset: 60 }}
          />
        </svg>
      </div>
    </div>
  );
};
export default Indicator;
