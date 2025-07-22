import * as React from "react";

import {
  useCreateToDoList,
  useDeleteToDoList,
  useGetAllToDoLists,
  useRenameToDoList,
} from "../../hooks/queries/ToDoList";
import { PlusIcon } from "../Icons/PlusIcon";
import { TrashIcon } from "../Icons/TrashIcon";
import { PencilSquareIcon } from "../Icons/PencilSquareIcon";
import { ChevronRightIcon } from "../Icons/ChevronRightIcon";

interface ToDoListContentProps {
  isOpen: boolean;
}

export function TodoListContent({ isOpen }: Readonly<ToDoListContentProps>) {
  const [newListName, setNewListName] = React.useState("");
  const [renameListId, setRenameListId] = React.useState<number | null>(null);
  const [renameListName, setRenameListName] = React.useState("");

  const { data: todoLists } = useGetAllToDoLists({ enabled: isOpen });
  const { mutate: createToDoList } = useCreateToDoList();
  const { mutate: deleteToDoList } = useDeleteToDoList();
  const { mutate: renameToDoList } = useRenameToDoList();

  if (todoLists == null) {
    return <span className="loading loading-spinner loading-md" />;
  }

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter" && newListName.trim() !== "") {
      event.preventDefault();
      createToDoList({ name: newListName });
      setNewListName("");
    }
  };

  return (
    <ul className="space-y-2 font-medium">
      {todoLists?.map((list) => (
        <li key={list.id}>
          <a
            href={`/todo-list/${list.id}`}
            className="flex items-center p-2 text-gray-900 rounded-lg hover:bg-gray-100 group"
          >
            <ChevronRightIcon />
            {renameListId === list.id ? (
              <label className="input" aria-label="Rename list">
                <input
                  type="text"
                  value={renameListName ?? list.name ?? ""}
                  onChange={(e) => {
                    setRenameListName(e.target.value);
                  }}
                  onKeyDown={(e) => {
                    if (e.key === "Enter" && renameListName.trim() !== "") {
                      e.preventDefault();
                      renameToDoList({
                        listId: list.id!,
                        newName: renameListName,
                      });
                      setRenameListId(null);
                    }
                  }}
                  onClick={(e) => e.preventDefault()}
                  className="grow"
                />
              </label>
            ) : (
              <span className="flex-1 ml-3">{list.name!}</span>
            )}
            <div className="flex flex-row items-center gap-1">
              <button
                className="btn btn-square btn-ghost cursor-pointer"
                type="button"
                onClick={(e) => {
                  e.preventDefault();
                  setRenameListId(list.id!);
                  setRenameListName(list.name ?? "");
                }}
              >
                <PencilSquareIcon />
              </button>
              <button
                className="btn btn-square btn-ghost cursor-pointer"
                type="button"
                onClick={(e) => {
                  e.preventDefault();
                  deleteToDoList(list.id!);
                }}
              >
                <TrashIcon />
              </button>
            </div>
          </a>
        </li>
      ))}
      <li>
        <div className="flex flex-row items-center gap-1">
          <label className="input" aria-label="Add new list">
            <PlusIcon />
            <input
              value={newListName}
              onChange={(e) => setNewListName(e.target.value)}
              onKeyDown={handleKeyDown}
              type="text"
              placeholder="Add new list"
              className="grow"
            />
          </label>
        </div>
      </li>
    </ul>
  );
}
