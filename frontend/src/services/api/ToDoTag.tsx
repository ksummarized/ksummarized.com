import StatusCode from "../../helpers/StatusCode";
import { ApiService, CreateTagRequest } from "../../client";

export const getAllToDoTags = async () => {
  const { data, response } = await ApiService.getAllTags();

  if (response.status !== StatusCode.OK) {
    throw new Error("Failed to get all tags");
  }

  return data!;
};

export const createToDoTag = async (body: CreateTagRequest) => {
  const { data, response } = await ApiService.createTag({ body });

  if (response.status !== StatusCode.CREATED || !data) {
    throw new Error("Failed to create task");
  }

  return data;
};

export const getToDoTag = async (tagId: number) => {
  const { data, response } = await ApiService.getTag({ path: { Id: tagId } });

  if (response.status !== StatusCode.OK || !data) {
    throw new Error("Failed to fetch task");
  }

  return data;
};

export const deleteToDoTag = async (tagId: number) => {
  const { response } = await ApiService.deleteTag({ path: { Id: tagId } });

  if (response.status !== StatusCode.NO_CONTENT) {
    throw new Error("Failed to delete task");
  }
  return null;
};
