import * as React from "react";

import { TodoItem } from "../../client";
import { PencilSquareIcon } from "../../components/Icons/PencilSquareIcon";
import ToDoTaskDetailsForm from "../../components/Forms/ToDoTasks/ToDoTaskDetailsForm";

interface ItemDetailsModalProps {
  item: TodoItem;
}

export function ItemDetailsModal({ item }: Readonly<ItemDetailsModalProps>) {
  const modalId = `item-details-modal-${item.id}`;

  return (
    <>
      <button
        className="btn btn-square btn-ghost"
        onClick={() => {
          const dialog = document.getElementById(modalId) as HTMLDialogElement;
          dialog?.showModal();
        }}
      >
        <PencilSquareIcon variant="solid" />
        <span className="sr-only">Edit Task</span>
      </button>
      <dialog id={modalId} className="modal">
        <div className="modal-box">
          <form method="dialog">
            {/* if there is a button in form, it will close the modal */}
            <button className="text-black btn btn-sm btn-circle btn-ghost absolute right-2 top-2">
              ✕
            </button>
          </form>
          <h1 className="text-black font-bold text-lg">Task Details</h1>
          <ToDoTaskDetailsForm item={item} />
        </div>
      </dialog>
    </>
  );
}
