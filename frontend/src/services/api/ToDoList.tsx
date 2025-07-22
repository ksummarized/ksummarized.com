import StatusCode from "../../helpers/StatusCode";
import { ApiService, CreateListRequest } from "../../client";

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

export const getToDoList = async (listId: number, includeSubtasks: boolean) => {
  const { data, response } = await ApiService.getList({
    path: { Id: listId },
    query: { IncludeSubtasks: includeSubtasks },
  });

  if (response.status !== StatusCode.OK || !data) {
    throw new Error("Failed to fetch list");
  }

  return data;
};

export const deleteToDoList = async (listId: number) => {
  const { response } = await ApiService.deleteList({ path: { Id: listId } });

  if (response.status !== StatusCode.NO_CONTENT) {
    throw new Error("Failed to delete list");
  }
  return null;
};

export const renameToDoList = async (listId: number, newName: string) => {
  const { response } = await ApiService.renameList({
    path: { Id: listId },
    body: { name: newName },
  });

  if (response.status !== StatusCode.OK) {
    throw new Error("Failed to rename list");
  }

  return null;
};
