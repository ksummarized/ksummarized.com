import { useEffect } from "react";

import fetchPlus from "../helpers/fetchPlus";
import useRefreshToken from "./useRefreshToken";
import { UserType } from "../helpers/CustomTypes";
import StatusCode from "../helpers/StatusCode";
import { ResponsePlus } from "../helpers/ResponsePlus";

const useFetchPlus = () => {
  const refresh = useRefreshToken();
  const localStorageUser = localStorage.getItem("user");
  let user: UserType | null = null;
  if (localStorageUser) {
    user = JSON.parse(localStorageUser);
  }

  useEffect(() => {
    const requestInterceptorIndex = fetchPlus.requestInterceptors.use(
      (config) => {
        const headers = new Headers(config.headers);
        if (!headers.get("Authorization")) {
          headers.set("Authorization", `Bearer ${user?.token}`);
          config.headers = headers;
        }

        return config;
      },
      (error) => Promise.reject(error),
    );

    const responseInterceptorIndex = fetchPlus.responseInterceptors.use(
      (response: ResponsePlus) => response,
      async (error: ResponsePlus) => {
        const request = error?.request;
        const requestConfig = error?.requestConfig;
        if (
          request &&
          requestConfig &&
          error?.status === StatusCode.FORBIDDEN &&
          !requestConfig?.sent
        ) {
          requestConfig.sent = true;
          const newToken = await refresh();
          if (newToken) {
            const headers = new Headers(requestConfig.headers);
            headers.set("Authorization", `Bearer ${newToken}`);
            requestConfig.headers = headers;

            return fetchPlus(request.url, requestConfig);
          }
        }
        return Promise.reject(error);
      },
    );

    return () => {
      fetchPlus.requestInterceptors.eject(requestInterceptorIndex);
      fetchPlus.responseInterceptors.eject(responseInterceptorIndex);
    };
  }, [user, refresh]);

  return fetchPlus;
};

export default useFetchPlus;
