import { RequestInitPlus } from "./RequestInitPlus";

export interface ResponsePlus extends Response {
  request?: Request;
  requestConfig?: RequestInitPlus;
}
