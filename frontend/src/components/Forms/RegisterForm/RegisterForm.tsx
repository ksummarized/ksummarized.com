import * as React from "react";
import { SubmitHandler, useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
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

  const onSubmitHandler: SubmitHandler<ValidationSchema> = (
    data: ValidationSchema
  ) => {
    fetchPlus(`${Constants.BASE_URL}/auth/register`, {
      body: JSON.stringify({
        email: data.email,
        password: data.password,
      }),
    })
      .then((response: Response) => {
        // TODO: When the backend API will be fixed, changed the status code to 201 (CREATED)
        if (response.status !== StatusCode.OK) {
          return response.text().then((text) => {
            throw new Error(text);
          });
        }
        return response.text();
      })
      .then((responseData: string) => {
        alert(responseData);
        navigate("/login", { replace: true });
      })
      .catch((error: Error) => {
        alert(error);
      });
  };

  const onCancelHandler: React.MouseEventHandler = () => {
    navigate(Constants.NAVIGATE_PREVIOUS_PAGE);
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
        <Button
          type="button"
          onClick={onCancelHandler}
          fullWidth
          variant="outlined"
          sx={{ mt: 1, mb: 3 }}
        >
          CANCEL
        </Button>
      </Box>
    </FormProvider>
  );
}

export default RegisterForm;
