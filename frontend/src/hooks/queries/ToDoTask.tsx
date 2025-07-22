import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createToDoTask,
  deleteToDoTask,
  getToDoTask,
  updateToDoTask,
} from "../../services/api/ToDoTask";
import { CreateTaskRequest, TodoItem } from "../../client";

export const useCreateToDoTask = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (body: CreateTaskRequest) => createToDoTask(body),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["ToDo", "lists"],
      });
    },
  });
};

export const useGetToDoTask = (taskId: number) => {
  return useQuery({
    queryKey: ["ToDo", "tasks", taskId],
    queryFn: () => getToDoTask(taskId),
  });
};

export const useUpdateToDoTask = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (body: TodoItem) => updateToDoTask(body),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["ToDo", "tasks"],
      });
      queryClient.invalidateQueries({
        queryKey: ["ToDo", "lists"],
      });
    },
  });
};

export const useDeleteToDoTask = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (taskId: number) => deleteToDoTask(taskId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["ToDo", "lists"],
      });
    },
  });
};
