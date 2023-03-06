type RequestInterceptorHandler = {
  fulfilled: (init: RequestInit) => void;
  rejected: (init: RequestInit) => void;
};

export default class RequestInterceptorManager {
  handlers: Array<RequestInterceptorHandler>;

  constructor() {
    this.handlers = [];
  }

  use(
    fulfilled: (init: RequestInit) => void,
    rejected: (init: RequestInit) => void
  ) {
    return this.handlers.push({ fulfilled, rejected });
  }

  get() {
    return this.handlers;
  }

  eject(index: number) {
    const indexOfHandler = index - 1;
    if (index > -1) {
      this.handlers.splice(indexOfHandler, 1);
    }
  }
}
