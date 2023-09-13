import * as React from "react";

import UndrawRelaxingAtHome from "../../assets/images/UndrawRelaxingAtHome.svg";
import TopBar from "../../components/TopBar/TopBar";
import SideMenu from "../../components/SideMenu/SideMenu";

export default function HomePage(): JSX.Element {
  return (
    <div className="flex">
      <TopBar />
      <SideMenu />
      <div className="flex flex-col p-4 sm:ml-64 sm:mt-16 justify-items-center content-center">
        <img src={UndrawRelaxingAtHome} alt="Relax" />
        <h6 className="text-3xl text-center">Nothing to do! Time to relax!</h6>
      </div>
    </div>
  );
}
