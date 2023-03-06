import ResponseInterceptorManager from "./ResponseInterceptorManager";
import RequestInterceptorManager from "./RequestInterceptorManager";
import { ResponsePlus } from "./ResponsePlus";
import { RequestInitPlus } from "./RequestInitPlus";

function fetchPlus(input: URL | RequestInfo, init: RequestInitPlus = {}) {
  const requestInterceptors = fetchPlus.requestInterceptors.get();
  const responseInterceptors = fetchPlus.responseInterceptors.get();
  const dispatchRequest = async (
    initModified: RequestInitPlus
  ): Promise<ResponsePlus> => {
    const request = new Request(input, initModified);
    const originalResponse: ResponsePlus = await fetch(request);
    originalResponse.request = request;
    originalResponse.requestConfig = initModified;
    return Promise.resolve(originalResponse);
  };
  const chain = [
    ...requestInterceptors,
    { fulfilled: dispatchRequest, rejected: null },
    ...responseInterceptors,
  ];
  let promise: any = Promise.resolve(init);
  while (chain.length) {
    const chainItem = chain.shift();
    promise = promise.then(chainItem?.fulfilled, chainItem?.rejected);
  }
  return promise;
}

fetchPlus.requestInterceptors = new RequestInterceptorManager();
fetchPlus.responseInterceptors = new ResponseInterceptorManager();

fetchPlus.requestInterceptors.use(
  (config: RequestInitPlus) => {
    config.method = "POST";
    config.credentials = "include";
    const requestHeaders = new Headers(config.headers);
    requestHeaders.set("Content-Type", "application/json");
    config.headers = requestHeaders;
    return config;
  },
  () =>
    Promise.reject(
      Error("An error occurred while setting the required request parameters.")
    )
);

export default fetchPlus;
