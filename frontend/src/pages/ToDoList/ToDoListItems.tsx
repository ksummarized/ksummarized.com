import * as React from "react";

import { TodoItem } from "../../client";
import { ToDoListItem } from "./ToDoListItem";

interface ToDoListItemsProps {
  listItems: TodoItem[] | null | undefined;
}

export function ToDoListItems({ listItems }: Readonly<ToDoListItemsProps>) {
  if (listItems === null || listItems === undefined) {
    return <span className="loading loading-spinner loading-md" />;
  }

  if (listItems.length === 0) {
    return (
      <div className="flex flex-col gap-2">
        <p>You do not have any tasks yet. Create one!</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-2">
      {listItems?.map((item) => {
        return <ToDoListItem key={item.id} item={item} />;
      })}
    </div>
  );
}
