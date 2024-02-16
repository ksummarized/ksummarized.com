import React from "react";
import { Outlet, useOutletContext } from "react-router-dom";
import Keycloak from "keycloak-js";

import useAuth from "../hooks/useAuth";
import Constants from "./Constants";

export default function RequireAuth() {
  const { isLogin, keycloak } = useAuth();

  const createUserInBackend = async () => {
    const response = await fetch(`${Constants.BASE_URL}/auth/create-user`, {
      headers: { Authorization: `Bearer ${keycloak!.token}` },
    });
    if (response.status !== 200) {
      const errorMessage = await response.text();
      throw new Error(errorMessage);
    }
  };

  if (isLogin) {
    createUserInBackend();
  }

  return isLogin ? <Outlet context={keycloak} /> : <p>Authenticating...</p>;
}

export function useKeycloak() {
  return useOutletContext<Keycloak>();
}
