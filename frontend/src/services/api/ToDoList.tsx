import StatusCode from "../../helpers/StatusCode";
import { ApiService, CreateListRequest, CreateTaskRequest } from "../../client";

export const getAllToDoLists = async () => {
  const { data, response } = await ApiService.getAllLists();

  if (response.status !== StatusCode.OK) {
    throw new Error("Failed to fetch lists");
  }

  return data;
};

export const createToDoList = async (body: CreateListRequest) => {
  const { data, response } = await ApiService.postApiTodoLists({ body });

  if (response.status !== StatusCode.CREATED || !data) {
    throw new Error("Failed to create list");
  }

  return data;
};

export const getToDoList = async (listId: number) => {
  const { data, response } = await ApiService.getList({ path: { Id: listId } });

  if (response.status !== StatusCode.OK || !data) {
    throw new Error("Failed to fetch list");
  }

  return data;
};

export const createToDoTask = async (body: CreateTaskRequest) => {
  const { data, response } = await ApiService.createTask({ body });

  if (response.status !== StatusCode.CREATED || !data) {
    throw new Error("Failed to create task");
  }

  return data;
};
