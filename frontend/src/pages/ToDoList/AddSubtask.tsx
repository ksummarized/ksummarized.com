import * as React from "react";

import { TodoItem } from "../../client";
import { useUpdateToDoTask } from "../../hooks/queries/ToDoTask";
import { PlusIcon } from "../../components/Icons/PlusIcon";

interface AddSubtaskProps {
  item: TodoItem;
}

export function AddSubtask({ item }: Readonly<AddSubtaskProps>) {
  const [newSubtaskName, setNewSubtaskName] = React.useState("");
  const { mutate: updateToDoTask } = useUpdateToDoTask();

  const handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter" && newSubtaskName.trim() !== "") {
      event.preventDefault();
      const newSubtask: TodoItem = {
        listId: item.listId,
        name: newSubtaskName.trim(),
        completed: false,
        notes: "",
        tags: [],
        subtasks: [],
      };
      updateToDoTask({
        ...item,
        subtasks: [...(item.subtasks || []), newSubtask],
      });
      setNewSubtaskName("");
    }
  };

  return (
    <div className="flex flex-row items-center gap-1 p-2">
      <label className="input flex-1" aria-label="Add new subtask">
        <PlusIcon />
        <input
          value={newSubtaskName}
          onChange={(e) => setNewSubtaskName(e.target.value)}
          onKeyDown={handleKeyDown}
          type="text"
          placeholder="Add new subtask"
          className="grow"
        />
      </label>
    </div>
  );
}
