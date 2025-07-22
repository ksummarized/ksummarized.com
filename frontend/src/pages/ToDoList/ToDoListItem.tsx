import * as React from "react";

import {
  useDeleteToDoTask,
  useUpdateToDoTask,
} from "../../hooks/queries/ToDoTask";
import { TodoItem } from "../../client";
import { PlusIcon } from "../../components/Icons/PlusIcon";
import { ClockIcon } from "../../components/Icons/ClockIcon";
import { TrashIcon } from "../../components/Icons/TrashIcon";
import { DocumentIcon } from "../../components/Icons/DocumentIcon";
import { ItemDetailsModal } from "./ItemDetailsModal";
import { ToDoSubtaskItem } from "./ToDoSubtaskItem";

interface ToDoListItemProps {
  item: TodoItem;
}

export function ToDoListItem({ item }: Readonly<ToDoListItemProps>) {
  const [newSubtaskName, setNewSubtaskName] = React.useState("");

  const { mutate: updateToDoTask } = useUpdateToDoTask();
  const { mutate: deleteToDoTask } = useDeleteToDoTask();

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

  const completedSubtasks = item.subtasks?.filter(
    (subtask) => subtask.completed,
  ).length;

  return (
    <div className="collapse border overflow-visible">
      <input type="checkbox" />
      <div className="collapse-title bg-ks-secondary-soil flex flex-col p-4 gap-2 rounded-box">
        <div className="flex flex-row gap-2 justify-between items-center">
          <div className="flex flex-row gap-2">
            <input
              type="checkbox"
              checked={item.completed}
              onClick={(e) => e.stopPropagation()}
              onChange={() => {
                updateToDoTask({ ...item, completed: !item.completed });
              }}
              className="checkbox size-6 rounded-full border-ks-primary bg-ks-secondary-paper z-10 hover:bg-ks-secondary-sand"
            />
            <p className="text-ks-secondary-paper">{item.name}</p>
          </div>
          <div className="flex flex-row divide-x-2 divide-ks-secondary-paper items-center">
            {item.notes && (
              <div className="flex flex-row text-ks-secondary-paper px-1">
                <DocumentIcon />
                <span className="text-ks-secondary-paper">Note</span>
              </div>
            )}
            {item.subtasks && item.subtasks.length > 0 && (
              <span className="text-ks-secondary-paper px-1">
                {completedSubtasks}/{item.subtasks.length} subtasks
              </span>
            )}
          </div>
          <div className="flex flex-row gap-2 items-center">
            {item.deadline && (
              <div className="flex flex-row text-ks-secondary-paper pl-4 pr-4">
                <ClockIcon />
                <p>{new Date(item.deadline).toLocaleString()}</p>
              </div>
            )}
            <div className="flex flex-row gap-1 z-10 items-center text-white">
              <div className="tooltip" data-tip="Edit item">
                <ItemDetailsModal item={item} />
              </div>
              <div className="tooltip" data-tip="Delete item">
                <button
                  className="btn btn-square btn-ghost cursor-pointer"
                  onClick={() => deleteToDoTask(item.id!)}
                >
                  <TrashIcon variant="solid" />
                </button>
              </div>
            </div>
          </div>
        </div>
        <div className="flex flex-row gap-2">
          {item.tags?.map((tag) => {
            return (
              <div
                key={tag.id}
                className="badge badge-outline badge-sm bg-ks-secondary-paper border-ks-primary"
              >
                {tag.name}
              </div>
            );
          })}
        </div>
      </div>
      <div className="collapse-content">
        <div className="flex flex-col gap-2 bg-ks-secondary-paper p-2 mx-2 rounded-b-xl">
          {item.subtasks?.map((subtask) => {
            return <ToDoSubtaskItem key={subtask.id} item={subtask} />;
          })}
          <div className="flex flex-row items-center gap-1">
            <label className="input" aria-label="Add new subtask">
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
        </div>
      </div>
    </div>
  );
}
