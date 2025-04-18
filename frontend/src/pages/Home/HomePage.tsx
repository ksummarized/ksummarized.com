import * as React from "react";

import UndrawRelaxingAtHome from "../../assets/images/UndrawRelax.svg";
import { useKeycloak } from "../../helpers/RequireAuth";

export default function HomePage(): React.JSX.Element {
  const keycloak = useKeycloak();

  return (
    <div className="flex bg-ks-secondary-paper overflow-y-auto h-screen w-full">
      <div className="flex flex-col p-4 justify-items-center content-center">
        <img src={UndrawRelaxingAtHome} alt="Relax" />
        <h6 className="text-3xl text-center">Nothing to do! Time to relax!</h6>
        <button
          type="button"
          className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
          onClick={() =>
            keycloak.logout({ redirectUri: "http://localhost:8888/" })
          }
        >
          Logout
        </button>
      </div>
    </div>
  );
}
