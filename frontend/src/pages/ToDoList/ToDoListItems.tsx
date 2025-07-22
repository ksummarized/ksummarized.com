import * as React from "react";

import { useCreateToDoTask } from "../../hooks/queries/ToDoTask";
import { TodoItem } from "../../client";
import { PlusIcon } from "../../components/Icons/PlusIcon";
import { ToDoListItem } from "./ToDoListItem";

interface AddTaskInputProps {
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  handleKeyDown: (event: React.KeyboardEvent<HTMLInputElement>) => void;
}

function AddTaskInput({
  value,
  onChange,
  handleKeyDown,
}: Readonly<AddTaskInputProps>) {
  return (
    <div className="flex flex-row items-center gap-1">
      <label className="input" aria-label="Add new task">
        <PlusIcon />
        <input
          value={value}
          onChange={onChange}
          onKeyDown={handleKeyDown}
          type="text"
          placeholder="Add new task"
          className="grow"
        />
      </label>
    </div>
  );
}

interface ToDoListItemsProps {
  listItems: TodoItem[] | null | undefined;
  listId: number;
}

export function ToDoListItems({
  listItems,
  listId,
}: Readonly<ToDoListItemsProps>) {
  const [newTaskName, setNewTaskName] = React.useState("");

  const { mutate: createToDoTask } = useCreateToDoTask();

  if (listItems === null || listItems === undefined) {
    return <span className="loading loading-spinner loading-md" />;
  }

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter" && newTaskName.trim() !== "") {
      event.preventDefault();
      createToDoTask({
        listId: listId,
        name: newTaskName,
        notes: "",
        tags: [],
        subtasks: [],
      });
      setNewTaskName("");
    }
  };

  if (listItems.length === 0) {
    return (
      <div className="flex flex-col gap-2">
        <p>You do not have any tasks yet. Create one!</p>
        <AddTaskInput
          value={newTaskName}
          onChange={(e) => setNewTaskName(e.target.value)}
          handleKeyDown={handleKeyDown}
        />
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-2">
      <div className="flex flex-col gap-2">
        {listItems?.map((item) => {
          return <ToDoListItem key={item.id} item={item} />;
        })}
      </div>
      <AddTaskInput
        value={newTaskName}
        onChange={(e) => setNewTaskName(e.target.value)}
        handleKeyDown={handleKeyDown}
      />
    </div>
  );
}
