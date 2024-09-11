import React from "react";
import { Outlet, useOutletContext } from "react-router-dom";
import Keycloak from "keycloak-js";

import useAuth from "../hooks/useAuth";
import Constants from "./Constants";

export default function RequireAuth() {
  const { isLogin, keycloak } = useAuth();
  return isLogin ? <Outlet context={keycloak} /> : <p>Authenticating...</p>;
}

export function useKeycloak() {
  return useOutletContext<Keycloak>();
}
