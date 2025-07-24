import * as React from "react";

import { useGetToDoList } from "../../hooks/queries/ToDoList";
import { Navigate, useParams } from "react-router-dom";
import { ToDoListItems } from "./ToDoListItems";
import { AddTask } from "./AddTask";

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
      <div className="flex justify-center items-center h-full">
        <span className="loading loading-spinner loading-md" />
      </div>
    );
  }

  if (isError || !toDoList) {
    return (
      <div className="flex justify-center items-center h-full">
        Error loading ToDo list!
      </div>
    );
  }

  return (
    <div className="flex flex-col h-full">
      <header className="p-4">
        <h1 className="text-2xl font-semibold text-ks-primary text-center mb-2">
          {toDoList?.name}
        </h1>
        <div className="max-w-3xl mx-auto relative">
          <div className="w-full bg-gray-200 rounded-full h-5">
            <div
              className="bg-ks-primary h-5 rounded-full"
              style={{
                width: `${((toDoList.items || []).filter((item) => item.completed).length / (toDoList.items || []).length) * 100 || 0}%`,
              }}
            ></div>
          </div>
          <span className="absolute inset-0 flex items-center justify-center text-sm text-ks-secondary-dark">
            {(toDoList.items || []).filter((item) => item.completed).length}/
            {(toDoList.items || []).length}
          </span>
        </div>
      </header>
      <main className="flex-1 overflow-y-auto p-4">
        <div className="max-w-3xl mx-auto">
          <ToDoListItems listItems={toDoList.items} />
        </div>
      </main>
      <footer className="p-4">
        <AddTask listId={toDoList.id!} />
      </footer>
    </div>
  );
}
