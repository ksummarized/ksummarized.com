import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getAllToDoTags, createToDoTag } from "../../services/api/ToDoTag";
import { CreateTagRequest } from "../../client";

export const useGetAllToDoTags = () => {
  return useQuery({
    queryKey: ["ToDo", "tags", "all"],
    queryFn: getAllToDoTags,
  });
};

export const useCreateToDoTag = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (body: CreateTagRequest) => createToDoTag(body),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["ToDo", "tags"],
      });
    },
  });
};
