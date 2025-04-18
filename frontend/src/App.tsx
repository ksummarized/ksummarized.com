import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";

import "./index.css";
import HomePage from "./pages/Home/HomePage";
import StartPage from "./pages/Start/StartPage";
import TopBar from "./components/TopBar/TopBar";
import SideMenu from "./components/SideMenu/SideMenu";
import RequireAuth from "./helpers/RequireAuth";
import NotFound from "./pages/NotFound/NotFound";
import ToDoListPage from "./pages/ToDoList/ToDoListsPage";
import ToDoListDetailPage from "./pages/ToDoList/ToDoListDetailPage";

function App(): React.JSX.Element {
  return (
    <BrowserRouter>
      <div className="h-screen flex flex-col">
        <TopBar />
        <SideMenu />
        <div className="ml-64 overflow-y-auto">
          <Routes>
            <Route index element={<StartPage />} />
            <Route element={<RequireAuth />}>
              <Route path="home" element={<HomePage />} />
              <Route path="todo-list" element={<ToDoListPage />} />
              <Route
                path="todo-list/:listId"
                element={<ToDoListDetailPage />}
              />
            </Route>
            <Route path="*" element={<NotFound />} />
          </Routes>
        </div>
      </div>
    </BrowserRouter>
  );
}

export default App;
