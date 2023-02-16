import * as React from "react";
import { SubmitHandler, useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Grid from "@mui/material/Grid";
import Typography from "@mui/material/Typography";
import { Link as ReactLink } from "@mui/material";
import FormControlLabel from "@mui/material/FormControlLabel";
import { useNavigate, useLocation } from "react-router-dom";
import { isAxiosError } from "axios";

import googleLogo from "../../../assets/googleLogo.svg";
import githubLogo from "../../../assets/githubLogo.svg";
import facebookLogo from "../../../assets/facebookLogo.svg";
import twitterLogo from "../../../assets/twitterLogo.svg";
import TextFieldInput from "../../Fields/FormInput/TextFieldInput";
import CheckboxInput from "../../Fields/FormInput/CheckboxInput";
import useAxiosPrivate from "../../../hooks/useAxiosPrivate";

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
  const axiosPrivate = useAxiosPrivate();
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
    data: ValidationSchema
  ) => {
    try {
      const response = await axiosPrivate.post("/auth/login", {
        email: data.email,
        password: data.password,
      });
      const token = response?.data?.token;
      const refreshToken = response?.data?.refreshToken;
      localStorage.setItem(
        "user",
        JSON.stringify({ email: data.email, token, refreshToken })
      );
      navigate(from, { replace: true });
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
        <FormControlLabel
          control={
            <CheckboxInput
              value="remember"
              name="remember"
              id="remember"
              color="primary"
            />
          }
          label="Remember me"
        />
        <Typography component="h1" variant="h6" align="center">
          Don&apos;t have an account?&nbsp;
          <ReactLink href="/register" color="secondary">
            Sign Up
          </ReactLink>
        </Typography>
        <Button
          type="submit"
          fullWidth
          variant="contained"
          sx={{ mt: 3, mb: 2 }}
        >
          LOGIN
        </Button>
      </Box>
      <Grid container spacing={5} paddingTop={2} justifyContent="center">
        <Grid item>
          <Box
            component="img"
            sx={{
              height: 40,
              width: 40,
            }}
            src={googleLogo}
          />
        </Grid>
        <Grid item>
          <Box
            component="img"
            sx={{
              height: 40,
              width: 40,
            }}
            src={githubLogo}
          />
        </Grid>
        <Grid item>
          <Box
            component="img"
            sx={{
              height: 40,
              width: 40,
            }}
            src={facebookLogo}
          />
        </Grid>
        <Grid item>
          <Box
            component="img"
            sx={{
              height: 40,
              width: 40,
            }}
            src={twitterLogo}
          />
        </Grid>
      </Grid>
    </FormProvider>
  );
}

export default LoginForm;
