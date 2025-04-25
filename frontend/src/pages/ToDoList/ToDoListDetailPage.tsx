import * as React from "react";

import { useParams } from "react-router-dom";

import { TodoItem } from "../../client";
import { useGetToDoList } from "../../hooks/queries/ToDoList";

type ToDoListItemProps = {
  item: TodoItem;
};
function ToDoListItem({ item }: Readonly<ToDoListItemProps>) {
  return (
    <div className="flex flex-col bg-ks-secondary-soil rounded-xl p-4 gap-2">
      <div className="flex flex-row gap-2">
        <input
          type="checkbox"
          checked={item.completed}
          className="checkbox size-6 rounded-full border-ks-primary bg-ks-secondary-paper cursor-default"
        />
        <p className="text-ks-secondary-paper">{item.name}</p>
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

export default function ToDoListDetailPage(): React.JSX.Element {
  const { listId } = useParams();
  const { data, isLoading, isError } = useGetToDoList(+listId!);

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
        Error loading ToDo list!
      </div>
    );
  }

  return (
    <div className="flex flex-col p-2 max-w-full max-h-full overflow-y-auto">
      <p className="text-xl text-ks-primary font-bold">{data.name}</p>
      <div className="flex flex-col gap-4">
        {data.items?.map((item) => {
          return <ToDoListItem key={item.id} item={item} />;
        })}
      </div>
    </div>
  );
}
