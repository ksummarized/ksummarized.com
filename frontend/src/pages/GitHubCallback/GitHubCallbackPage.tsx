import React, { useEffect } from "react"
import Constants from "../../helpers/Constants";
import { redirect, useLoaderData, useNavigate} from "react-router-dom";

export default function GitHubCallbackPage() {
    const _ = useLoaderData();
    const navigate = useNavigate();
    useEffect(()=>{
        navigate("/home");
    }, []);
    return <h1>Loading...</h1>
}

export async function CodeLoader({ request }: any) {
    const url = new URL(request.url);
    const code = url.searchParams.get("code");
    const response = await fetch(`${Constants.BASE_URL}/auth/login-github-callback?code=${code}`);
    if (response.status !== 200) {
        const errorMessage = await response.text();
        throw new Error(errorMessage);
    }
    const responseData = await response.json();
    const token = responseData?.token;
    const refreshToken = responseData?.refreshToken;
    localStorage.setItem(
        "user",
        JSON.stringify({ email: "tmp@tmp.com", token, refreshToken })
    );
    return null;
}