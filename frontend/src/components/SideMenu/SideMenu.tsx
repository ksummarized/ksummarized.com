import * as React from "react";

import { ListBulletIcon } from "../Icons/ListBulletIcon";
import { CalendarIcon } from "../Icons/CalendarIcon";
import { SquaresPlusIcon } from "../Icons/SquaresPlusIcon";
import { TodoListContent } from "./ToDoListContent";

function SideMenu() {
  const [isOpen, setIsOpen] = React.useState(false);

  return (
    <aside
      id="default-sidebar"
      className="fixed top-16 w-64 bg-ks-secondary-sand h-screen transition-transform -translate-x-full sm:translate-x-0"
      aria-label="Sidebar"
    >
      <div className="h-full px-3 py-4 overflow-y-auto">
        <ul className="space-y-2 font-medium">
          <li>
            <div className="collapse collapse-arrow text-gray-900 rounded-lg hover:bg-gray-100 group">
              <input type="checkbox" onChange={() => setIsOpen(!isOpen)} />
              <div className="collapse-title flex flex-row items-center p-2 font-semibold">
                <ListBulletIcon />
                <span className="ml-3">ToDoList</span>
              </div>
              <div className="collapse-content bg-ks-secondary-paper">
                <TodoListContent isOpen={isOpen} />
              </div>
            </div>
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
