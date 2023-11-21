type ResponseInterceptorHandler = {
  fulfilled: (response: Response) => void;
  rejected: (response: Response) => void;
};

export default class ResponseInterceptorManager {
  handlers: Array<ResponseInterceptorHandler>;

  constructor() {
    this.handlers = [];
  }

  use(
    fulfilled: (response: Response) => void,
    rejected: (response: Response) => void,
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
