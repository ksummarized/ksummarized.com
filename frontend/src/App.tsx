import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";

import "./index.css";
import HomePage from "./pages/Home/HomePage";
import StartPage from "./pages/Start/StartPage";
import TopBar from "./components/TopBar/TopBar";
import SideMenu from "./components/SideMenu/SideMenu";
import RequireAuth from "./helpers/RequireAuth";
import NotFound from "./pages/NotFound/NotFound";
import ToDoListPage from "./pages/ToDoList/ToDoListPage";

function App(): React.JSX.Element {
  return (
    <BrowserRouter>
      <div className="h-screen flex flex-col">
        <TopBar />
        <div className="flex flex-1 overflow-hidden">
          <SideMenu />
          <main className="flex-1 overflow-y-auto p-4">
            <Routes>
              <Route index element={<StartPage />} />
              <Route element={<RequireAuth />}>
                <Route path="home" element={<HomePage />} />
                <Route path="todo-list/:listId" element={<ToDoListPage />} />
              </Route>
              <Route path="*" element={<NotFound />} />
            </Routes>
          </main>
        </div>
      </div>
    </BrowserRouter>
  );
}

export default App;
