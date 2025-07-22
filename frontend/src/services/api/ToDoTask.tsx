import StatusCode from "../../helpers/StatusCode";
import { ApiService, CreateTaskRequest, TodoItem } from "../../client";

export const createToDoTask = async (body: CreateTaskRequest) => {
  const { data, response } = await ApiService.createTask({ body });

  if (response.status !== StatusCode.CREATED || !data) {
    throw new Error("Failed to create task");
  }

  return data;
};

export const getToDoTask = async (taskId: number) => {
  const { data, response } = await ApiService.getTask({ path: { Id: taskId } });

  if (response.status !== StatusCode.OK || !data) {
    throw new Error("Failed to fetch task");
  }

  return data;
};

export const updateToDoTask = async (body: TodoItem) => {
  const { response } = await ApiService.updateTask({ body });

  if (response.status !== StatusCode.OK) {
    throw new Error("Failed to update task");
  }
  return null;
};

export const deleteToDoTask = async (taskId: number) => {
  const { response } = await ApiService.deleteTask({ path: { Id: taskId } });

  if (response.status !== StatusCode.NO_CONTENT) {
    throw new Error("Failed to delete task");
  }
  return null;
};
