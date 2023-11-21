import * as React from "react";

import CalendarIcon from "../../assets/icons/CalendarIcon.svg";
import OrganizerIcon from "../../assets/icons/OrganizerIcon.svg";
import ToDoListIcon from "../../assets/icons/ToDoListIcon.svg";

function SideMenu() {
  return (
    <aside
      id="default-sidebar"
      className="fixed top-16 w-64 h-screen transition-transform -translate-x-full sm:translate-x-0"
      aria-label="Sidebar"
    >
      <div className="h-full px-3 py-4 overflow-y-auto bg-gray-50">
        <ul className="space-y-2 font-medium">
          <li>
            <a
              href="/"
              className="flex items-center p-2 text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700 group"
            >
              <img src={ToDoListIcon} alt="ToDoList icon" />
              <span className="ml-3">ToDoList</span>
            </a>
          </li>
          <li>
            <a
              href="/"
              className="flex items-center p-2 text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700 group"
            >
              <img src={CalendarIcon} alt="Calendar icon" />
              <span className="flex-1 ml-3 whitespace-nowrap">Calendar</span>
            </a>
          </li>
          <li>
            <a
              href="/"
              className="flex items-center p-2 text-gray-900 rounded-lg dark:text-white hover:bg-gray-100 dark:hover:bg-gray-700 group"
            >
              <img src={OrganizerIcon} alt="Organizer icon" />
              <span className="flex-1 ml-3 whitespace-nowrap">Organizer</span>
            </a>
          </li>
        </ul>
      </div>
    </aside>
  );
}

export default SideMenu;
