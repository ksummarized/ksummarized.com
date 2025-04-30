import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createToDoList,
  createToDoTask,
  getAllToDoLists,
  getToDoList,
  getToDoTask,
} from "../../services/api/ToDoList";
import { CreateListRequest, CreateTaskRequest, TodoItem } from "../../client";

export const useGetAllToDoLists = () => {
  return useQuery({
    queryKey: ["ToDo", "lists"],
    queryFn: getAllToDoLists,
  });
};

export const useCreateToDoList = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (body: CreateListRequest) => createToDoList(body),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["ToDo", "lists"],
      });
    },
  });
};

export const useGetToDoList = (listId: number) => {
  return useQuery({
    queryKey: ["ToDo", "list", listId],
    queryFn: () => getToDoList(listId),
  });
};

export const useCreateToDoTask = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (body: CreateTaskRequest) => createToDoTask(body),
    onSuccess: (data: TodoItem) => {
      queryClient.invalidateQueries({
        queryKey: ["ToDo", "list", data.listId],
      });
    },
  });
};

export const useGetToDoTask = (taskId: number) => {
  return useQuery({
    queryKey: ["ToDo", "task", taskId],
    queryFn: () => getToDoTask(taskId),
  });
};
