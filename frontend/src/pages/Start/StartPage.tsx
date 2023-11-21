import * as React from "react";

export default function HomePage(): JSX.Element {
  return (
    <div className="xl:container mx-auto mb-32 bg-gray-200">
      <div className="flex justify-center">
        <h1 className="text-5xl">Welcome to ksummarized!</h1>
      </div>
      <h2 className="text-3xl">
        Log in:&nbsp;
        <a className="text-blue-700" href="/login">
          here
        </a>
      </h2>
      <h2 className="text-3xl">
        Register:&nbsp;
        <a className="text-blue-700" href="/register">
          here
        </a>
      </h2>
      <h2 className="text-3xl">
        Home:&nbsp;
        <a className="text-blue-700" href="/home">
          here
        </a>
      </h2>
    </div>
  );
}
