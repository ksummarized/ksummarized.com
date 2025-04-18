import * as React from "react";

import KsummarizedLogo from "../../assets/logos/KsummarizedLogo.png";

function TopBar() {
  return (
    <nav className="relative bg-ks-secondary-soil w-full z-50 top-0 left-0 border-b border-ks-secondary-dark">
      <div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto p-4">
        <a href="/home" className="flex items-center">
          <img src={KsummarizedLogo} className="h-8" alt="Ksummarized logo" />
        </a>
      </div>
    </nav>
  );
}

export default TopBar;
