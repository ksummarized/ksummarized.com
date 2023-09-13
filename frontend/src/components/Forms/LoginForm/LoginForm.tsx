import * as React from "react";
import { SubmitHandler, useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useNavigate, useLocation } from "react-router-dom";

import IconGoogleLogo from "../../../assets/logos/IconGoogleLogo.svg";
import IconGithubLogo from "../../../assets/logos/IconGithubLogo.svg";
import IconFacebookLogo from "../../../assets/logos/IconFacebookLogo.svg";
import IconXLogo from "../../../assets/logos/IconXLogo.svg";
import StatusCode from "../../../helpers/StatusCode";
import Constants from "../../../helpers/Constants";
import useFetchPlus from "../../../hooks/useFetchPlus";
import TextFieldInput from "../../Fields/FormInput/TextFieldInput";
import CheckboxInput from "../../Fields/FormInput/CheckboxInput";

const validationSchema = z.object({
  email: z
    .string()
    .min(1, { message: "Email is required" })
    .email({ message: "Please enter a valid email address" }),
  password: z
    .string()
    .min(1, "Password is required")
    .min(8, "Password must be more than 8 characters")
    .max(32, "Password must be less than 32 characters"),
  remember: z.boolean(),
});

type ValidationSchema = z.infer<typeof validationSchema>;

function LoginForm() {
  const fetchPlus = useFetchPlus();
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";

  const defaultValues: ValidationSchema = {
    email: "",
    password: "",
    remember: false,
  };

  const methods = useForm<ValidationSchema>({
    resolver: zodResolver(validationSchema),
    defaultValues,
  });

  const onSubmitHandler: SubmitHandler<ValidationSchema> = async (
    data: ValidationSchema,
  ) => {
    try {
      const response = await fetchPlus(`${Constants.BASE_URL}/auth/login`, {
        body: JSON.stringify({
          email: data.email,
          password: data.password,
        }),
      });
      if (response.status !== StatusCode.OK) {
        const errorMessage = await response.text();
        throw new Error(errorMessage);
      }
      const responseData = await response.json();
      const token = responseData?.token;
      const refreshToken = responseData?.refreshToken;
      localStorage.setItem(
        "user",
        JSON.stringify({ email: data.email, token, refreshToken }),
      );
      navigate(from, { replace: true });
    } catch (error) {
      alert(error);
    }
  };

  return (
    <FormProvider {...methods}>
      <form onSubmit={methods.handleSubmit(onSubmitHandler)} noValidate>
        <TextFieldInput
          required
          placeholder="Please enter your email address"
          id="email"
          label="Email Address"
          name="email"
          type="text"
        />
        <TextFieldInput
          required
          id="password"
          name="password"
          label="Password"
          type="password"
          placeholder="Password"
        />
        <CheckboxInput name="remember" id="remember" label="Remember me" />
        <h6>
          Don&apos;t have an account?&nbsp;
          <a href="/register" className="text-blue-600">
            Sign up
          </a>
        </h6>
        <button
          type="submit"
          className="w-full px-6 py-3 bg-slate-600 text-white font-medium uppercase rounded shadow-md hover:bg-gray-200 hover:shadow-lg focus:bg-blue-300 focus:outline-none focus:ring-0 active:bg-slate-900"
        >
          LOGIN
        </button>
      </form>
      <div className="grid grid-cols-4 gap-4 h-52 w-52">
        <img src={IconGoogleLogo} alt="google logo" />
        <img src={IconFacebookLogo} alt="facebook logo" />
        <img src={IconGithubLogo} alt="github logo" />
        <img src={IconXLogo} alt="x logo" />
      </div>
    </FormProvider>
  );
}

export default LoginForm;
