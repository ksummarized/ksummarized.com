import * as React from "react";

import { ListBulletIcon } from "../Icons/ListBulletIcon";
import { CalendarIcon } from "../Icons/CalendarIcon";
import { SquaresPlusIcon } from "../Icons/SquaresPlusIcon";
import { ChevronRightIcon } from "../Icons/ChevronRightIcon";
import { TodoListContent } from "./ToDoListContent";

function SideMenu() {
  const [isToDoListOpen, setIsToDoListOpen] = React.useState(false);

  return (
    <aside
      id="default-sidebar"
      className="w-64 bg-ks-secondary-sand h-full hidden sm:block"
      aria-label="Sidebar"
    >
      <div className="h-full px-3 py-4 overflow-y-auto">
        <ul className="space-y-2 font-medium">
          <li>
            <button
              className="flex flex-row items-center p-2 text-gray-900 rounded-lg hover:bg-gray-100 group w-full"
              onClick={() => setIsToDoListOpen(!isToDoListOpen)}
            >
              <ListBulletIcon />
              <span className="ml-3 flex-1 text-left">ToDoList</span>
              <ChevronRightIcon
                className={`w-5 h-5 transition-transform ${isToDoListOpen ? "rotate-90" : ""}`}
              />
            </button>
            {isToDoListOpen && (
              <div className="bg-ks-secondary-paper rounded-b-lg">
                <TodoListContent />
              </div>
            )}
          </li>
          <li>
            <a
              href="/"
              className="flex items-center p-2 text-gray-900 rounded-lg hover:bg-gray-100 group"
            >
              <CalendarIcon />
              <span className="flex-1 ml-3 whitespace-nowrap">Calendar</span>
            </a>
          </li>
          <li>
            <a
              href="/"
              className="flex items-center p-2 text-gray-900 rounded-lg hover:bg-gray-100 group"
            >
              <SquaresPlusIcon />
              <span className="flex-1 ml-3 whitespace-nowrap">Organizer</span>
            </a>
          </li>
        </ul>
      </div>
    </aside>
  );
}

export default SideMenu;
