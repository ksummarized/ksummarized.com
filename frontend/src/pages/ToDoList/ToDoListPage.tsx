import * as React from "react";

import {
  useGetToDoList,
  useRenameToDoList,
} from "../../hooks/queries/ToDoList";
import { Navigate, useParams } from "react-router-dom";
import { ToDoListItems } from "./ToDoListItems";
import { AddTask } from "./AddTask";

export default function ToDoListPage(): React.JSX.Element {
  const { listId } = useParams();
  const [isEditingName, setIsEditingName] = React.useState(false);
  const [newListName, setNewListName] = React.useState("");

  const {
    data: toDoList,
    isLoading,
    isError,
  } = useGetToDoList(Number(listId), true);

  const { mutate: renameToDoList } = useRenameToDoList();

  React.useEffect(() => {
    if (toDoList?.name) {
      setNewListName(toDoList.name);
    }
  }, [toDoList?.name]);

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

  if (isError) {
    return (
      <div className="flex justify-center items-center h-full">
        Error loading ToDo list!
      </div>
    );
  }

  if (!toDoList) {
    return (
      <div className="flex justify-center items-center h-full">
        ToDo list not found!
      </div>
    );
  }

  const handleNameChange = () => {
    if (newListName.trim() !== "" && newListName !== toDoList.name) {
      renameToDoList({ listId: toDoList.id!, newName: newListName });
    }
    setIsEditingName(false);
  };

  return (
    <div className="flex flex-col h-full">
      <header>
        {isEditingName ? (
          <div className="max-w-3xl mx-auto">
            <label className="input w-full mb-2">
              <input
                type="text"
                value={newListName}
                onChange={(e) => setNewListName(e.target.value)}
                onBlur={handleNameChange}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    handleNameChange();
                  }
                }}
                className="grow text-2xl font-semibold text-ks-primary text-center"
                autoFocus
              />
            </label>
          </div>
        ) : (
          <button
            className="text-2xl font-semibold text-ks-primary text-center cursor-pointer w-full bg-transparent border-none focus:outline-none mb-2"
            onClick={() => setIsEditingName(true)}
          >
            {toDoList?.name}
          </button>
        )}
        <div className="max-w-3xl mx-auto relative mb-2">
          <div className="w-full bg-gray-200 rounded-full h-5">
            <div
              className="bg-ks-primary h-5 rounded-full"
              style={{
                width: `${((toDoList.items || []).filter((item) => item.completed).length / (toDoList.items || []).length) * 100 || 0}%`,
              }}
            ></div>
          </div>
          <span className="absolute inset-x-0 top-1/2 -translate-y-1/2 flex items-center justify-center text-sm text-ks-secondary-dark">
            {(toDoList.items || []).filter((item) => item.completed).length}/
            {(toDoList.items || []).length}
          </span>
        </div>
      </header>
      <main className="flex-1 overflow-y-auto p-4">
        <div className="max-w-3xl mx-auto">
          {(toDoList.items || []).length === 0 ? (
            <div className="flex flex-col gap-2 items-center justify-center h-full">
              <p>You do not have any tasks yet. Create one!</p>
            </div>
          ) : (
            <ToDoListItems listItems={toDoList.items} />
          )}
        </div>
      </main>
      <footer className="p-4">
        <AddTask listId={toDoList.id!} />
      </footer>
    </div>
  );
}
