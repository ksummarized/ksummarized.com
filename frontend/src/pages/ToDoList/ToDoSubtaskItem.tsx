import * as React from "react";

import {
  useDeleteToDoTask,
  useUpdateToDoTask,
} from "../../hooks/queries/ToDoTask";
import { TodoItem } from "../../client";
import { ClockIcon } from "../../components/Icons/ClockIcon";
import { TrashIcon } from "../../components/Icons/TrashIcon";
import { DocumentIcon } from "../../components/Icons/DocumentIcon";
import { ItemDetailsModal } from "./ItemDetailsModal";

interface ToDoSubtaskItemProps {
  item: TodoItem;
}

export function ToDoSubtaskItem({ item }: Readonly<ToDoSubtaskItemProps>) {
  const { mutate: updateToDoTask } = useUpdateToDoTask();
  const { mutate: deleteToDoTask } = useDeleteToDoTask();

  return (
    <div className="flex flex-col bg-ks-secondary-sand rounded-xl p-4 gap-2">
      <div className="flex flex-row gap-2 justify-between items-center">
        <div className="flex flex-row gap-2">
          <input
            type="checkbox"
            checked={item.completed}
            className="checkbox size-6 rounded-full border-ks-primary bg-ks-secondary-paper cursor-pointer hover:bg-ks-secondary-sand"
            onChange={(e) => {
              e.preventDefault();
              updateToDoTask({ ...item, completed: !item.completed });
            }}
          />
          <p className="text-black break-all">{item.name}</p>
        </div>
        {item.notes && (
          <div className="flex flex-row text-black px-1">
            <DocumentIcon />
            <span className="text-black">Note</span>
          </div>
        )}
        <div className="flex flex-row gap-2 items-center">
          {item.deadline && (
            <div className="flex flex-row text-black pl-4">
              <ClockIcon />
              <p>{new Date(item.deadline).toLocaleString()}</p>
            </div>
          )}
          <div className="flex flex-row gap-1 z-10 items-center text-black">
            <div className="tooltip" data-tip="Edit item">
              <ItemDetailsModal item={item} />
            </div>
            <div className="tooltip" data-tip="Delete item">
              <button
                className="btn btn-square btn-ghost cursor-pointer text-black"
                onClick={() => deleteToDoTask(item.id!)}
              >
                <TrashIcon variant="solid" />
              </button>
            </div>
          </div>
        </div>
      </div>
      <div className="flex flex-row gap-2 flex-wrap">
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
  );
}
