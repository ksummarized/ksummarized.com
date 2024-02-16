import { useState, useEffect, useRef } from "react";
import Keycloak from "keycloak-js";

const keycloakClient = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL,
  realm: import.meta.env.VITE_KEYCLOAK_REALM,
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID,
});

const useAuth = () => {
  const isRun = useRef(false);
  const [isLogin, setIsLogin] = useState(false);
  const [keycloak, setKeycloak] = useState<Keycloak | null>(null);

  useEffect(() => {
    // Protection to not run useEffect twice
    if (isRun.current) {
      return;
    }
    isRun.current = true;

    const auth = async () => {
      try {
        keycloakClient.onTokenExpired = async () => {
          try {
            const refreshed = await keycloakClient.updateToken(5);
            if (refreshed) {
              console.log("Token refreshed");
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

        const authenticated = await keycloakClient.init({
          onLoad: "login-required",
        });
        setIsLogin(authenticated);
        if (keycloakClient.token !== undefined) {
          setKeycloak(keycloakClient);
        }
      } catch (error) {
        console.error("Failed to initialize adapter:", error);
      }
    };

    auth();
  }, []);

  return { isLogin, keycloak };
};

export default useAuth;
