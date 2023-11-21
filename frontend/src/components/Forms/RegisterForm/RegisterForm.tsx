import * as React from "react";
import { SubmitHandler, useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useNavigate } from "react-router-dom";

import TextFieldInput from "../../Fields/FormInput/TextFieldInput";
import fetchPlus from "../../../helpers/fetchPlus";
import Constants from "../../../helpers/Constants";
import StatusCode from "../../../helpers/StatusCode";

const validationSchema = z
  .object({
    email: z
      .string()
      .min(1, { message: "Email is required" })
      .email({ message: "Please enter a valid email address" }),
    password: z
      .string()
      .min(1, "Password is required")
      .min(8, "Password must be more than 8 characters")
      .max(32, "Password must be less than 32 characters"),
    confirmPassword: z
      .string()
      .min(1, { message: "Confirm Password is required" }),
  })
  .refine((data) => data.password === data.confirmPassword, {
    path: ["confirmPassword"],
    message: "The passwords don't match",
  });

type ValidationSchema = z.infer<typeof validationSchema>;

function RegisterForm() {
  const navigate = useNavigate();
  const defaultValues: ValidationSchema = {
    email: "",
    password: "",
    confirmPassword: "",
  };

  const methods = useForm<ValidationSchema>({
    resolver: zodResolver(validationSchema),
    defaultValues,
  });

  const onSubmitHandler: SubmitHandler<ValidationSchema> = async (
    data: ValidationSchema,
  ) => {
    try {
      const response = await fetchPlus(`${Constants.BASE_URL}/auth/register`, {
        body: JSON.stringify({
          email: data.email,
          password: data.password,
        }),
      });
      const responseData = await response.text();
      if (response.status !== StatusCode.OK) {
        throw new Error(responseData);
      }
      alert(responseData);
      navigate("/login", { replace: true });
    } catch (error) {
      alert(error);
    }
  };

  const onCancelHandler: React.MouseEventHandler = () => {
    navigate(Constants.NAVIGATE_PREVIOUS_PAGE);
  };

  return (
    <FormProvider {...methods}>
      <form onSubmit={methods.handleSubmit(onSubmitHandler)} noValidate>
        <TextFieldInput
          placeholder="Please enter your email address"
          required
          type="text"
          id="email"
          label="Email Address"
          name="email"
        />
        <TextFieldInput
          required
          placeholder="Password"
          id="password"
          name="password"
          label="Password"
          type="password"
        />
        <TextFieldInput
          required
          id="confirmPassword"
          name="confirmPassword"
          label="Confirm password"
          placeholder="Confirm password"
          type="password"
        />
        <button
          type="submit"
          className="w-full px-6 py-3 bg-slate-600 text-white font-medium uppercase rounded shadow-md hover:bg-gray-200 hover:shadow-lg focus:bg-blue-300 focus:outline-none focus:ring-0 active:bg-slate-900"
        >
          REGISTER
        </button>
        <button
          type="button"
          onClick={onCancelHandler}
          className="w-full px-6 py-3 bg-slate-300 text-white font-medium uppercase rounded shadow-md hover:bg-gray-200 hover:shadow-lg focus:bg-blue-300 focus:outline-none focus:ring-0 active:bg-slate-900"
        >
          CANCEL
        </button>
      </form>
    </FormProvider>
  );
}

export default RegisterForm;
