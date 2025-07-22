import * as React from "react";

import { useGetToDoList } from "../../hooks/queries/ToDoList";
import { Navigate, useParams } from "react-router-dom";
import { ToDoListItems } from "./ToDoListItems";

export default function ToDoListPage(): React.JSX.Element {
  const { listId } = useParams();

  const {
    data: toDoList,
    isLoading,
    isError,
  } = useGetToDoList(Number(listId), true);

  if (!listId) {
    return <Navigate to="/404" />;
  }

  if (isLoading) {
    return (
      <div className="container flex justify-center max-w-full max-h-full overflow-y-auto">
        <span className="loading loading-spinner loading-md" />
      </div>
    );
  }

  if (isError || !toDoList) {
    return (
      <div className="container flex justify-center max-w-full max-h-full overflow-y-auto">
        Error loading ToDo list!
      </div>
    );
  }

  return (
    <div className="container flex max-w-full max-h-full overflow-y-auto">
      <div className="flex flex-col gap-4 w-full">
        <div className="flex flex-col p-2 gap-4">
          <div className="flex flex-col border border-ks-secondary-dark rounded-xl">
            <div className="flex flex-row justify-between items-center p-4">
              <h1 className="text-2xl font-semibold text-ks-primary">
                {toDoList?.name}
              </h1>
            </div>
            <div className="p-4">
              <ToDoListItems listItems={toDoList.items} listId={toDoList.id!} />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
