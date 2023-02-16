import * as React from "react";
import { SubmitHandler, useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import { useNavigate } from "react-router-dom";
import { isAxiosError } from "axios";

import TextFieldInput from "../../Fields/FormInput/TextFieldInput";
import useAxiosPrivate from "../../../hooks/useAxiosPrivate";

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
  const axiosPrivate = useAxiosPrivate();
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
    data: ValidationSchema
  ) => {
    try {
      const response = await axiosPrivate.post("/auth/register", {
        email: data.email,
        password: data.password,
      });
      alert(response.data);
      navigate("/login", { replace: true });
    } catch (error) {
      console.error(error);
      if (isAxiosError(error)) {
        alert(error.response?.data);
      }
    }
  };

  return (
    <FormProvider {...methods}>
      <Box
        component="form"
        onSubmit={methods.handleSubmit(onSubmitHandler)}
        noValidate
        sx={{ mt: 1 }}
      >
        <TextFieldInput
          helperText="Please enter your email address"
          margin="normal"
          required
          fullWidth
          id="email"
          label="Email Address"
          name="email"
          autoComplete="email"
          autoFocus
          variant="filled"
        />
        <TextFieldInput
          fullWidth
          required
          id="password"
          name="password"
          label="Password"
          variant="filled"
          type="password"
          sx={{ mb: 1 }}
        />
        <TextFieldInput
          fullWidth
          required
          id="confirmPassword"
          name="confirmPassword"
          label="Confirm password"
          variant="filled"
          type="password"
        />
        <Button type="submit" fullWidth variant="contained" sx={{ mt: 6 }}>
          REGISTER
        </Button>
        <Button type="reset" fullWidth variant="outlined" sx={{ mt: 1, mb: 3 }}>
          CANCEL
        </Button>
      </Box>
    </FormProvider>
  );
}

export default RegisterForm;
