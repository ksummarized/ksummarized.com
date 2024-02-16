import { useEffect } from "react";

import fetchPlus from "../helpers/fetchPlus";
import { ResponsePlus } from "../helpers/ResponsePlus";
import { useKeycloak } from "../helpers/RequireAuth";

const useFetchPlus = () => {
  const keycloak = useKeycloak();

  useEffect(() => {
    const requestInterceptorIndex = fetchPlus.requestInterceptors.use(
      (config) => {
        const headers = new Headers(config.headers);
        if (!headers.get("Authorization")) {
          headers.set("Authorization", `Bearer ${keycloak.token}`);
          config.headers = headers;
        }

        return config;
      },
      (error) => Promise.reject(error),
    );

    const responseInterceptorIndex = fetchPlus.responseInterceptors.use(
      (response: ResponsePlus) => response,
      async (error: ResponsePlus) => {
        console.log(
          `There was error during fetching request. Response: ${error}`,
        );
      },
    );

    return () => {
      fetchPlus.requestInterceptors.eject(requestInterceptorIndex);
      fetchPlus.responseInterceptors.eject(responseInterceptorIndex);
    };
  }, [keycloak]);

  return fetchPlus;
};

export default useFetchPlus;
