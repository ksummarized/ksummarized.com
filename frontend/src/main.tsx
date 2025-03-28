import React from "react";
import ReactDOM from "react-dom/client";
import "./index.css";
import App from "./App";
import clientSetup from "./clientSetup";

clientSetup();

const rootElement = document.getElementById("root");
if (!rootElement) {
  throw new Error("Failed to find root element");
}
const root = ReactDOM.createRoot(rootElement);

root.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
);
