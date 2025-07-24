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
        <h1 className="text-2xl font-semibold text-ks-primary text-center">
          {toDoList?.name}
        </h1>
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
