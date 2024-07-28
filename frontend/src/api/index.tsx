import { PropsWithChildren, useState, createContext, useContext, useRef } from "react";
import axios from "axios";
import type { AxiosInstance, AxiosRequestHeaders } from "axios";
// import { message, notification } from "antd";
import { API_BASE_URL } from "env";
import { useAuth } from "auth/use-auth";

type ApiContextValue = AxiosInstance;

export const ApiContext = createContext<ApiContextValue>({} as any);

const ApiProvider = (props: PropsWithChildren<any>) => {
  const { user, getAccessToken } = useAuth();
  const accessTokenGetter = useRef<() => Promise<string>>();

  const [api] = useState<AxiosInstance>(() => {
    const instance = axios.create({ baseURL: API_BASE_URL });
    instance.interceptors.request.use(
      async (config) => {
        console.log("accessTokenGetter", user, accessTokenGetter.current);
        if (!accessTokenGetter.current) {
          accessTokenGetter.current = getAccessToken;
        }

        const access_token = await accessTokenGetter.current();
        // debugger;
        // const access_token = user?.access_token || "";

        const headers = {
          ...config,
          headers: {
            ...config.headers,
            Authorization: access_token ? ["Bearer", access_token].join(" ") : undefined,
          } as AxiosRequestHeaders,
        };
        // console.log("first", { access_token, headers });
        return headers;
      },
      (error) => Promise.reject(error)
    );

    return instance;
  });

  return <ApiContext.Provider value={api}>{props.children}</ApiContext.Provider>;
};

export const useApi = () => useContext(ApiContext);

export default ApiProvider;

// const {api}= useAPI();

// s.get("/api/v1/user");

// // import { use } from 'ahooks';
// // import { useConstant } from './hooks/use-constant';
// // import { UserManagerContext } from './UserManagerProvider';
// // import { getSafeLoginURL, isExternal } from './utils';

// /**
//  *
//  */
// interface RequestConfig extends AxiosRequestConfig {
//   /**
//    * 请求出错时是否需要弹窗提示，false 不弹窗，如果想主动显示错误提示，请设置为false
//    * @default true
//    */
//   notifyOnError?: boolean;
//   /**
//    * 是否需要携带 token
//    * @default true
//    */
//   needToken?: boolean;
//   /**
//    * 是否处理返回的 code，为 true 时会 reject 掉 response.data.code 不为的 0 的请求
//    * @default true
//    */
//   handleResponseCode?: boolean;
//   /**
//    * 是否处理返回体中的 data, 即返回 response.data.data
//    * @default true
//    */
//   handleResponse2Data?: false;
//   /**
//    * 直接返回 response
//    */
//   responseDirect?: true;
// }

// // interface PaginatedResponse<T = any> {
// //   data: T[];
// //   total: number;
// //   current: number;
// //   pageSize: number;
// // }

// export type Request = <T = any>(url: string | RequestConfig, config?: RequestConfig) => Promise<T>;

// export interface IAPIContext {
//   /**
//    * 默认返回 AxiosResponse.data.data
//    * handleResponse2Data:false 返回 AxiosResponse.data
//    * responseDirect:true AxiosResponse
//    */
//   api: Request & {
//     get: Request;
//     post: Request;
//     put: Request;
//     delete: Request;
//     patch: Request;
//     /**
//      * @returns \{total: number, current: number, pageSize: number, data: []}
//      */
//     page: Request;
//   };

//   /**
//    * 默认返回 AxiosResponse.data.data
//    * 不携带 token 的请求，
//    * needToken:fasle
//    * @returns  AxiosResponse.data.data
//    **/
//   request: Request;
//   /**
//    * 默认返回 AxiosResponse.data.data
//    * handleResponseData:false 返回 AxiosResponse.data
//    * responseDirect:true AxiosResponse
//    */
//   // fetch?: Request;
//   // request4BusRequest?: Request;
// }

// export type DefaultConfig = {
//   /**
//    * 是否处理返回的 code，为 true 时会 reject 掉 response.data.code 不为的 0 的请求
//    * @default true
//    */
//   handleResponseCode?: boolean;
//   /**
//    * 是否处理返回体中的 data, 即返回 response.data.data
//    * @default true
//    */
//   handleResponse2Data?: boolean;
//   /**
//    * 直接返回 response
//    */
//   responseDirect?: boolean;
//   /**
//    * 请求成功处理函数，一般是处理 response.data.code，response.data.data
//    */
//   onResponseFulfilled?: (response: any) => any;
// };

// // eslint-disable-next-line @typescript-eslint/no-empty-interface
// export type APIProviderProps = PropsWithChildren<{
//   /**
//    * base 的跳转方法，fix:子应用token过期后跳转不正确的问题
//    */
//   globalNavigate?: NavigateFunction;
//   /**
//    * 默认配置
//    */
//   defaultConfig?: DefaultConfig;
// }>;

// export const APIContext = React.createContext<IAPIContext>({} as any);
// export const APIConsumer = APIContext.Consumer;

// export const APIProvider: FC<APIProviderProps> = (props) => {
//   const { children, defaultConfig, globalNavigate } = props;
//   const localNavigate = useNavigate();
//   const location = useLocation();
//   const { loginURL, apiBaseURL, userManager, getAccessToken } = useContext(UserManagerContext);
//   const safeLoginUrl = getSafeLoginURL(loginURL);
//   const defaultHandleResponseCode = defaultConfig?.handleResponseCode;
//   const defaultHandleResponse2Data = defaultConfig?.handleResponse2Data;
//   const defaultResponseDirect = defaultConfig?.responseDirect;
//   const navigate = globalNavigate || localNavigate;

//   const _logout = useConstant(() => {
//     return async () => {
//       await userManager.removeUser();

//       // 如果登录地址跟当前项目不一致，
//       if (isExternal(safeLoginUrl)) {
//         const redirect = encodeURIComponent(window.location.href);
//         window.location.href = `${safeLoginUrl}?redirect=${redirect}`;
//         return;
//       }

//       // 当前不是在登录页面
//       if (location.pathname !== safeLoginUrl) {
//         const redirect = encodeURIComponent(`${location.pathname}${location.search}`);
//         return navigate(`${safeLoginUrl}?redirect=${redirect}`, {});
//       }
//     };
//   });

//   const _onError = useCallback(async (error: any) => {
//     console.log("root.component.onError", error, error?.config, error?.toJSON?.());
//     // 发生网络错误时怎么处理
//     if (error?.config?.notifyOnError !== false) {
//       const errorMessage = error?.message || "发生网络错误";
//       console.warn("如果想自己控制错误弹窗，请配置 notifyOnError=false");
//       message.error(errorMessage);
//     }
//     throw error;
//   }, []);

//   const _onForbidden = useCallback(async (response: any) => {
//     console.log("root.component.onForbidden", response);
//     // code:403,权限不足(403)时怎么处理
//     // 提示信息并，reject 请求
//     if (response?.config?.notifyOnError !== false) {
//       notification.info({
//         duration: 2,
//         message: `${response.toJSON?.().message || "权限不足"}`,
//         description: `请联系管理员开放功能权限！`,
//       });
//     }
//     return Promise.reject(response);
//   }, []);

//   const _onUnauthorized = useCallback(
//     async (response: any) => {
//       // console.log('root.component.onUnauthorized', response);
//       // status=401 || code=401,权限不足(401)时怎么处理
//       // 跳转到登录页面
//       _logout();
//       return Promise.reject(response);
//     },
//     [_logout]
//   );

//   const _onRequestFulfilled = useConstant(() => {
//     return async (config: AxiosRequestConfig) => {
//       try {
//         const access_token = await getAccessToken();

//         return {
//           ...config,
//           headers: {
//             ...config.headers,
//             Authorization: `Bearer ${access_token}`,
//           },
//           params: { ...config.params, __r: new Date().getTime() },
//         };
//       } catch (ex) {
//         return config;
//       }
//     };
//   });

//   const _getOnFulfilledFunc = useConstant(() => {
//     return (handleResponseCode: boolean | undefined, defaultConf: DefaultConfig | undefined) => {
//       if (defaultConf?.onResponseFulfilled && typeof defaultConf?.onResponseFulfilled === "function")
//         return defaultConf.onResponseFulfilled;

//       if (handleResponseCode === false) return undefined;

//       return async (response: any) => {
//         const { data } = response as any;
//         if (typeof data !== "object") return response;
//         if (data.code === undefined || data.code === 0) return response; // 登录接口是没有 code 的
//         // console.log('instance.interceptors.response.onFulfilled1', 1);
//         // 以下是code不为 0 的情况，此时要返回 reject， 最终的 Promise 才会是 reject 状态。
//         if (data.code === 401) return _onUnauthorized(response);
//         if (data.code === 403) return _onForbidden(response);

//         // 其他状态交给业务去处理
//         const message = response.data?.message || response.data?.data || response.data;
//         const error: any = new Error(message);
//         error.config = response.config;
//         error.response = response;

//         return _onError(error);
//       };
//     };
//   });

//   const _onResponseRejected = useConstant(() => {
//     return async (ex: any) => {
//       // console.log('instance.interceptors.response.onRejected0', ex, ex.toJSON?.(), ex.response);
//       const { response } = ex || {};
//       const { status } = response || {};
//       if (status === 401) return _onUnauthorized(response);
//       if (status === 403) return _onForbidden(response);
//       return _onError(ex);
//     };
//   });

//   const fetch = async function <T = any>(url: string | RequestConfig, config?: RequestConfig): Promise<T> {
//     const options: RequestConfig = {
//       responseDirect: defaultResponseDirect,
//       handleResponseCode: defaultHandleResponseCode,
//       handleResponse2Data: defaultHandleResponse2Data,
//       ...config,
//       ...(typeof url === "string" ? { url } : (url as any)),
//     };
//     const instance = axios.create({ baseURL: apiBaseURL });
//     const { handleResponseCode, handleResponse2Data, responseDirect, needToken, ...rest } = options;

//     // 添加 token
//     if (needToken !== false) instance.interceptors.request.use(_onRequestFulfilled);
//     const onFulfilled = _getOnFulfilledFunc(handleResponseCode, defaultConfig);
//     instance.interceptors.response.use(onFulfilled, _onResponseRejected);
//     const resp = await instance.request(rest);

//     if (responseDirect === true) return resp as any;
//     if (handleResponse2Data !== false && typeof resp.data === "object") return resp.data.data;
//     return resp.data;
//   };

//   const api = useConstant(() => {
//     async function api<T = any>(url: string | RequestConfig, config?: RequestConfig) {
//       return fetch<T>(url, { ...config, needToken: true });
//     }

//     api.get = function <T = any>(url: string | RequestConfig, config?: RequestConfig) {
//       return api<T>(url, { ...config, method: "GET" });
//     };

//     api.post = function <T = any>(url: string | RequestConfig, config?: RequestConfig) {
//       return api<T>(url, { ...config, method: "POST" });
//     };

//     api.put = function <T = any>(url: string | RequestConfig, config?: RequestConfig) {
//       return api<T>(url, { ...config, method: "PUT" });
//     };

//     api.delete = function <T = any>(url: string | RequestConfig, config?: RequestConfig) {
//       return api<T>(url, { ...config, method: "DELETE" });
//     };

//     api.patch = function <T = any>(url: string | RequestConfig, config?: RequestConfig) {
//       return api<T>(url, { ...config, method: "PATCH" });
//     };

//     api.page = function <T = any>(url: string | RequestConfig, config?: RequestConfig) {
//       return api<T>(url, config).then((resp: any) => ({
//         ...resp,
//         current: resp.pageNum || 0,
//         data: resp.list || [],
//         success: true,
//       }));
//     };

//     return api;
//   });

//   const requestMethod = useConstant(() => {
//     // 在api 的基础上支持 Promise，直接返回数据
//     return function <T>(service: any) {
//       // console.log('request4UseRequest', typeof service === 'object' && service.then);
//       // 支持 Promise
//       if (typeof service === "object" && service.then) return Promise.resolve<T>(service);
//       return api<T>(service);
//     };
//   });

//   return (
//     <APIContext.Provider
//       value={{
//         api,
//         request: (url, config) => fetch(url, { ...config, needToken: false }),
//       }}
//     >
//       <UseAPIProvider value={{ requestMethod }}>{children}</UseAPIProvider>
//     </APIContext.Provider>
//   );
// };
