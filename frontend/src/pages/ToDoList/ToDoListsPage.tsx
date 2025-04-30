import * as React from "react";

import {
  useGetAllToDoLists,
  useGetToDoList,
  useGetToDoTask,
} from "../../hooks/queries/ToDoList";
import ToDoListCreateForm from "../../components/Forms/ToDoLists/ToDoListCreateForm";
import { GetListResponse, TodoItem } from "../../client";

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
        className="fixed top-14 right-0 z-10 m-4 bg-ks-secondary-dark text-white p-2 rounded-full cursor-pointer"
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

function SubtaskItem({ item }: Readonly<{ item: TodoItem }>) {
  return (
    <div className="flex flex-col bg-ks-secondary-sand rounded-xl p-4 gap-2 cursor-pointer">
      <div className="flex flex-row gap-2 justify-between">
        <div className="flex flex-row gap-2">
          <input
            type="checkbox"
            checked={item.completed}
            className="checkbox size-6 rounded-full border-ks-primary bg-ks-secondary-paper cursor-default"
          />
          <p className="text-black">{item.name}</p>
        </div>
        <div className="flex flex-row gap-2">
          {item.deadline && (
            <div className="flex flex-row text-black pl-4">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                viewBox="0 0 24 24"
                fill="currentColor"
                className="size-6"
              >
                <path
                  fillRule="evenodd"
                  d="M12 2.25c-5.385 0-9.75 4.365-9.75 9.75s4.365 9.75 9.75 9.75 9.75-4.365 9.75-9.75S17.385 2.25 12 2.25ZM12.75 6a.75.75 0 0 0-1.5 0v6c0 .414.336.75.75.75h4.5a.75.75 0 0 0 0-1.5h-3.75V6Z"
                  clipRule="evenodd"
                />
              </svg>
              <p>{new Date(item.deadline).toLocaleDateString()}</p>
            </div>
          )}
          <div className="text-black cursor-pointer">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth={1.5}
              stroke="currentColor"
              className="size-6"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M12 6.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 12.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 18.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5Z"
              />
            </svg>
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
  );
}

type ToDoListItemProps = {
  item: TodoItem;
};
function ToDoListItem({ item }: Readonly<ToDoListItemProps>) {
  const [isOpen, setIsOpen] = React.useState(false);

  const { data, isLoading, isError } = useGetToDoTask(item.id!);

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (isError || !data) {
    return <div>Error loading ToDo item!</div>;
  }

  return (
    <div className="flex flex-col">
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="flex flex-col bg-ks-secondary-soil rounded-xl p-4 gap-2 cursor-pointer"
      >
        <div className="flex flex-row gap-2 justify-between">
          <div className="flex flex-row gap-2">
            <input
              type="checkbox"
              checked={item.completed}
              className="checkbox size-6 rounded-full border-ks-primary bg-ks-secondary-paper cursor-default"
            />
            <p className="text-ks-secondary-paper">{item.name}</p>
          </div>
          <div className="flex flex-row gap-2">
            {item.deadline && (
              <div className="flex flex-row text-ks-secondary-paper pl-4">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  viewBox="0 0 24 24"
                  fill="currentColor"
                  className="size-6"
                >
                  <path
                    fillRule="evenodd"
                    d="M12 2.25c-5.385 0-9.75 4.365-9.75 9.75s4.365 9.75 9.75 9.75 9.75-4.365 9.75-9.75S17.385 2.25 12 2.25ZM12.75 6a.75.75 0 0 0-1.5 0v6c0 .414.336.75.75.75h4.5a.75.75 0 0 0 0-1.5h-3.75V6Z"
                    clipRule="evenodd"
                  />
                </svg>
                <p>{new Date(item.deadline).toLocaleDateString()}</p>
              </div>
            )}
            <div className="text-ks-secondary-paper cursor-pointer">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={1.5}
                stroke="currentColor"
                className="size-6"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M12 6.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 12.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 18.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5Z"
                />
              </svg>
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
      </button>
      {isOpen && (
        <div className="flex flex-col gap-2 border bg-ks-secondary-paper border-ks-secondary-soil p-2 mx-2 rounded-b-xl">
          {data.subtasks?.map((subtask) => {
            return <SubtaskItem key={subtask.id} item={subtask} />;
          })}
        </div>
      )}
    </div>
  );
}

function ToDoListItems({ listId }: Readonly<{ listId: number }>) {
  const { data, isLoading, isError } = useGetToDoList(listId);

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (isError || !data) {
    return <div>Error loading ToDo list!</div>;
  }

  return (
    <div className="flex flex-col">
      {data.items?.length === 0 ? (
        <p>You do not have any tasks yet. Create one!</p>
      ) : (
        <div className="flex flex-col gap-4">
          {data.items?.map((item) => {
            return <ToDoListItem key={item.id} item={item} />;
          })}
        </div>
      )}
    </div>
  );
}

type ToDoListComponentProps = {
  toDoList: GetListResponse;
};
function ToDoListComponent({ toDoList }: Readonly<ToDoListComponentProps>) {
  const [isOpen, setIsOpen] = React.useState(false);

  return (
    <div className="collapse collapse-arrow border border-ks-secondary-dark bg-ks-secondary-paper rounded-xl">
      <input type="checkbox" onChange={() => setIsOpen(!isOpen)} />
      <div className="collapse-title text-2xl font-semibold text-ks-primary">
        {toDoList?.name}
      </div>
      <div className="collapse-content">
        {isOpen && <ToDoListItems listId={toDoList.id!} />}
      </div>
    </div>
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

  return (
    <div className="container flex max-w-full max-h-full overflow-y-auto">
      <CreateToDoListButton />
      <div className="flex flex-col gap-4 w-full">
        <p className="text-2xl text-ks-secondary-dark font-semibold pl-2">
          Your ToDo lists
        </p>
        <div className="flex flex-col p-2 gap-4">
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

// TODO:
// - W menu bocznym zrobić z przycisku "ToDo list" rozwijalny akordeon i tam będzie można wybrać sobie listę,
// - Potem po kliku normalnie detale do listy po prawej,
// - Dodać możliwość dodawania zadań do listy poprzez input na samym dole listy
// - Dodać możliwość usuwania listy koszykiem po prawej od nazwy
