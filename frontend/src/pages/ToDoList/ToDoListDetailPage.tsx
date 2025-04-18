import * as React from "react";

import { useParams } from "react-router-dom";

import { useGetToDoList } from "../../hooks/queries/ToDoList";

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
    <div className="container flex justify-center max-w-full max-h-full overflow-y-auto">
      {/* <CreateToDoListButton /> */}
      <p>{data.name}</p>
    </div>
  );
}
