import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";

import "./index.css";
import HomePage from "./pages/Home/HomePage";
import StartPage from "./pages/Start/StartPage";
import RequireAuth from "./helpers/RequireAuth";
import NotFound from "./pages/NotFound/NotFound";

function App(): React.JSX.Element {
  return (
    <BrowserRouter>
      <Routes>
        <Route index element={<StartPage />} />
        <Route element={<RequireAuth />}>
          <Route path="home" element={<HomePage />} />
        </Route>
        <Route path="*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
