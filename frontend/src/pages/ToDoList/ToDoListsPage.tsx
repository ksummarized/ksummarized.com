import * as React from "react";

import { Link } from "react-router-dom";
import { useGetAllToDoLists } from "../../hooks/queries/ToDoList";
import ToDoListCreateForm from "../../components/Forms/ToDoLists/ToDoListCreateForm";
import { GetListResponse } from "../../client";

function CreateToDoListButton() {
  return (
    <>
      <button
        type="button"
        onClick={() =>
          (
            document.getElementById(
              "create-todo-list-modal",
            ) as HTMLDialogElement
          ).showModal()
        }
        className="fixed top-16 right-0 m-4 bg-ks-secondary-dark text-white p-2 rounded-full cursor-pointer"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          strokeWidth="1.5"
          stroke="currentColor"
          className="size-6"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M12 4.5v15m7.5-7.5h-15"
          />
        </svg>
      </button>
      <dialog id="create-todo-list-modal" className="modal">
        <div className="modal-box">
          <ToDoListCreateForm />
        </div>
        <form method="dialog" className="modal-backdrop">
          <button>Close</button>
        </form>
      </dialog>
    </>
  );
}

type ToDoListComponentProps = {
  toDoList: GetListResponse;
};
function ToDoListComponent({ toDoList }: Readonly<ToDoListComponentProps>) {
  return (
    <Link to={`/todo-list/${toDoList.id}`}>
      <div className="flex flex-col w-full">
        <div className="divider"></div>
        <p className="text-xl text-ks-primary font-semibold">
          {toDoList?.name}
        </p>
      </div>
    </Link>
  );
}

export default function ToDoListsPage(): React.JSX.Element {
  const { data, isLoading, isError } = useGetAllToDoLists();

  if (isLoading) {
    return (
      <div className="container flex justify-center max-w-full max-h-full overflow-y-auto">
        Loading...
      </div>
    );
  }

  if (isError || !data) {
    return (
      <div className="container flex justify-center max-w-full max-h-full overflow-y-auto">
        Error loading ToDo lists!
      </div>
    );
  }

  if (data.length === 0) {
    return (
      <>
        <div className="container flex justify-center max-w-full max-h-full overflow-y-auto">
          No ToDo lists found!
        </div>
        <CreateToDoListButton />
      </>
    );
  }

  return (
    <div className="container flex justify-center max-w-full max-h-full overflow-y-auto">
      <CreateToDoListButton />
      <div className="flex flex-col gap-4">
        <p className="text-2xl text-ks-secondary-dark font-semibold">
          Your ToDo lists
        </p>
        <div className="flex flex-col">
          {data.length === 0 ? (
            <p>You do not have any lists yet. Create one!</p>
          ) : (
            data.map((toDoList) => {
              return (
                <ToDoListComponent key={toDoList.id} toDoList={toDoList} />
              );
            })
          )}
        </div>
      </div>
    </div>
  );
}
