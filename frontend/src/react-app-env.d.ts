/// <reference types="react-scripts" />
interface ICONFIG {
  PUBLIC_URL: string;
  API_BASE_URL: string;
  [k: string]: any;
}

interface Window {
  CONFIG: ICONFIG;
}

// declare let CONFIG: ICONFIG;
