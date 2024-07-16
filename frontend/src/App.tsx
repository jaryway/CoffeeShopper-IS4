import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { Spin } from "antd";
import "assets/styles/index.css";
import routes from "./routes";
import { PUBLIC_URL } from "./env";

// const getLayoutMode = () => {
//   let pathname = window.location.pathname;
//   if (PUBLIC_URL) {
//     pathname = pathname.slice(PUBLIC_URL.length);
//   }

//   if (pathname.startsWith("/nosider")) {
//     return "nosider";
//   } else if (pathname.startsWith("/noheader")) {
//     return "noheader";
//   } else if (pathname.startsWith("/onlycontent")) {
//     return "onlycontent";
//   }
// };

function App() {
  // const layoutMode = getLayoutMode();
  const router = createBrowserRouter(routes, { basename: PUBLIC_URL });
  return <RouterProvider router={router} fallbackElement={<Spin />} />;
}

export default App;
