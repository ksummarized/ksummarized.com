import { useState, useEffect, useRef } from "react";
import Keycloak from "keycloak-js";
import { client } from "../client/client.gen";

const keycloakClient = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL,
  realm: import.meta.env.VITE_KEYCLOAK_REALM,
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID,
});

const useAuth = () => {
  const isRun = useRef(false);
  const [isLogin, setIsLogin] = useState(false);
  const [keycloak, setKeycloak] = useState<Keycloak | null>(null);

  const refreshToken = async () => {
    try {
      const refreshed = await keycloakClient.updateToken(5);
      if (refreshed) {
        console.log("Token refreshed");
        client.setConfig({ auth: () => keycloakClient.token });
      } else {
        console.log("Token is still valid");
      }
    } catch (error) {
      console.error(
        "Failed to refresh token, or the session has expired:",
        error,
      );
    }
  };

  const initializeAuth = async () => {
    try {
      keycloakClient.onTokenExpired = refreshToken;
      const authenticated = await keycloakClient.init({
        onLoad: "login-required",
      });
      setIsLogin(authenticated);
      if (keycloakClient.token !== undefined) {
        setKeycloak(keycloakClient);
        client.setConfig({ auth: () => keycloakClient.token });
      }
    } catch (error) {
      console.error("Failed to initialize adapter:", error);
    }
  };

  useEffect(() => {
    // Protection to not run useEffect twice
    if (isRun.current) {
      return;
    }
    isRun.current = true;
    initializeAuth();
  }, []);

  return { isLogin, keycloak };
};

export default useAuth;
