import * as React from "react";

import KsummarizedLogo from "../../assets/logos/KsummarizedLogo.png";

function TopBar() {
  return (
    <nav className="bg-gray-500 fixed w-full z-20 top-0 left-0 border-b border-gray-200">
      <div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto p-4">
        <a href="/home" className="flex items-center">
          <img src={KsummarizedLogo} className="h-8" alt="Ksummarized logo" />
        </a>
      </div>
    </nav>
  );
}

export default TopBar;
