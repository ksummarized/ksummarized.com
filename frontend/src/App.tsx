import React from "react";
import { BrowserRouter, Routes, Route, createBrowserRouter, RouterProvider } from "react-router-dom";
import { ThemeProvider } from "@mui/material/styles";

import "./App.css";
import darkTheme from "./styles/Styles";
import LoginPage from "./pages/Login/LoginPage";
import HomePage from "./pages/Home/HomePage";
import RegisterPage from "./pages/Register/RegisterPage";
import StartPage from "./pages/Start/StartPage";
import RequireAuth from "./helpers/RequireAuth";
import GitHubCallbackPage, { CodeLoader } from "./pages/GitHubCallback/GitHubCallbackPage";

function App(): JSX.Element {
  const router = createBrowserRouter([
    {
      index: true,
      element: <StartPage />
    },
    {
      element: <RequireAuth />,
      children: [
        {
          path: "home",
          element: <HomePage />
        }
      ]
    },
    {
      path: "login",
      element: <LoginPage />
    },
    {
      path: "register",
      element: <RegisterPage />
    },
    {
      path: "github-callback",
      element: <GitHubCallbackPage />,
      loader: CodeLoader
    }

  ]);
  return (
    <ThemeProvider theme={darkTheme}>
      <RouterProvider router={router}/>
    </ThemeProvider>
  );
}

export default App;
