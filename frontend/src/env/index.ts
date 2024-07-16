function getConfig<T = string>(key: string, def: any = ""): T {
  return (window.CONFIG || {})[key] || def;
}

// function getBoolen(key: string) {
//   return(window.CONFIG || {})[key] !== 0;
// }

export const PUBLIC_URL = getConfig("PUBLIC_URL");
// // export const RUN_MODE = getConfig('RUN_MODE');
// /**
//  * 单租户时，配置租户信息
//  */
// export const COMPANIES = getConfig<Array<{ id: string; name: string }>>('COMPANIES', []);
// /**
//  * 站点名称
//  */
// export const SYS_NAME = getConfig('SYS_NAME');
// /**
//  * 登录页配置，当系统需要不同的登录页时配置
//  */
// export const LOGIN_PAGE = getConfig('LOGIN_PAGE');
// /**
//  * 登录页配置，当系统需要不同的登录页时配置
//  */
// export const APP_LOGIN_PAGE = getConfig('APP_LOGIN_PAGE');
// /**
//  * 站点静态资源 CDN 配置，默认空字符串
//  */
// export const IMAGE_CDN_BASE = getConfig('IMAGE_CDN_BASE');
// /**
//  * 接口请求地址配置，默认值 '/base'
//  */
// export const API_BASE_URL = getConfig('API_BASE_URL', '/base');
// /**
//  * 部署环境配置，可选值 prod=正式生产环境,test=测试生产环境，此时系统会显示“测试”,默认值 test
//  */
// export const ENV = getConfig('ENV', 'prod');
// /**
//  * 工作台默认布局,'1'=广西重工版,'2'=原布局，默认是 '2'
//  */
// export const DEFAULT_WORKSPACE_LAYOUT = getConfig('DEFAULT_WORKSPACE_LAYOUT', '2');
// /**
//  * 手机端应用下载地址
//  */
// export const APP_DOWNLOAD_URL = getConfig('APP_DOWNLOAD_URL');
// /**
//  * 是否显示首页风格切换按钮,0=不显示,1=显示
//  */
// export const SHOW_HOME_STYLE_BUTTON = getBoolen('SHOW_HOME_STYLE_BUTTON');
// /**
//  * 是否开启上报功能，默认开启
//  */
// export const ACCESS_REPORT = getBoolen('ACCESS_REPORT');
// /**
//  * 是否开启验证码功能，默认开启
//  */
// export const CAPTCHA = getBoolen('CAPTCHA');

// /**
//  * 隐藏企业选择
//  */
// export const SHOW_COMPANY_SELECT = getBoolen('SHOW_COMPANY_SELECT');
// /**
//  * 企业ID
//  */
// export const COMPANY_ID = getConfig('COMPANY_ID', '');
