import React from "react";
import { useLocation, Navigate, Outlet } from "react-router-dom";

function RequireAuth() {
  const localStorageUser = localStorage.getItem("user");
  let user: any = null;
  if (localStorageUser) {
    user = JSON.parse(localStorageUser);
  }
  const location = useLocation();

  return user?.email ? (
    <Outlet />
  ) : (
    <Navigate to="/login" state={{ from: location }} replace />
  );
}

export default RequireAuth;
