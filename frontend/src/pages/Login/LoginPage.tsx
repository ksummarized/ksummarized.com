import * as React from "react";

import KsummarizedLogo from "../../assets/logos/KsummarizedLogo.png";

import LoginForm from "../../components/Forms/LoginForm/LoginForm";

export default function LoginPage(): JSX.Element {
  return (
    <div className="container">
      <div className="flex-col justify-items-center p-8">
        <div className="w-52 h-52">
          <img src={KsummarizedLogo} alt="ksummarized logo" />
        </div>
        <h6 className="text-1xl">Just one step to organized life!</h6>
        <LoginForm />
      </div>
    </div>
  );
}
