import * as React from "react";

import { PlusIcon } from "../../components/Icons/PlusIcon";
import { useCreateToDoTask } from "../../hooks/queries/ToDoTask";

interface AddTaskProps {
  listId: number;
}

export function AddTask({ listId }: Readonly<AddTaskProps>) {
  const [newTaskName, setNewTaskName] = React.useState("");

  const { mutate: createToDoTask } = useCreateToDoTask();

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

  return (
    <div className="max-w-2xl mx-auto">
      <div className="flex flex-row items-center gap-1 p-2">
        <label className="input flex-1" aria-label="Add new task">
          <PlusIcon />
          <input
            value={newTaskName}
            onChange={(e) => setNewTaskName(e.target.value)}
            onKeyDown={handleKeyDown}
            type="text"
            placeholder="Add new task"
            className="grow"
          />
        </label>
      </div>
    </div>
  );
}
