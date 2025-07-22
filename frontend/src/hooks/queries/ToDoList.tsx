import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createToDoList,
  deleteToDoList,
  getAllToDoLists,
  getToDoList,
  renameToDoList,
} from "../../services/api/ToDoList";
import { CreateListRequest } from "../../client";

interface GetAllToDoListsProps {
  enabled?: boolean;
}

export const useGetAllToDoLists = ({
  enabled = true,
}: GetAllToDoListsProps) => {
  return useQuery({
    queryKey: ["ToDo", "lists", "all"],
    queryFn: getAllToDoLists,
    enabled,
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

export const useGetToDoList = (listId: number, includeSubtasks: boolean) => {
  return useQuery({
    queryKey: ["ToDo", "lists", { listId, includeSubtasks }],
    queryFn: () => getToDoList(listId, includeSubtasks),
    enabled: !!listId,
  });
};

export const useDeleteToDoList = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (listId: number) => deleteToDoList(listId),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["ToDo", "lists"],
      });
    },
  });
};

export const useRenameToDoList = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ listId, newName }: { listId: number; newName: string }) =>
      renameToDoList(listId, newName),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["ToDo", "lists"],
      });
    },
  });
};
