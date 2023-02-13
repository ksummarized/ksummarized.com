import * as React from "react";
import { SubmitHandler, useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";
import Grid from "@mui/material/Grid";

import googleLogo from "../../../assets/googleLogo.svg";
import githubLogo from "../../../assets/githubLogo.svg";
import facebookLogo from "../../../assets/facebookLogo.svg";
import twitterLogo from "../../../assets/twitterLogo.svg";
import FormInput from "../../Fields/FormInput/FormInput";

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
});

type ValidationSchema = z.infer<typeof validationSchema>;

function Form() {
  const defaultValues: ValidationSchema = {
    email: "",
    password: "",
  };

  const methods = useForm<ValidationSchema>({
    resolver: zodResolver(validationSchema),
    defaultValues,
  });

  const onSubmitHandler: SubmitHandler<ValidationSchema> = (
    data: ValidationSchema
  ) => {
    console.log(JSON.stringify(data, null, 4));
  };

  return (
    <FormProvider {...methods}>
      <Box
        component="form"
        onSubmit={methods.handleSubmit(onSubmitHandler)}
        noValidate
        sx={{ mt: 1 }}
      >
        <FormInput
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
        <FormInput
          fullWidth
          required
          id="password"
          name="password"
          label="Password"
          variant="filled"
          type="password"
          sx={{ mb: 1 }}
        />
        <Button type="submit" fullWidth variant="contained" sx={{ mt: 6 }}>
          REGISTER
        </Button>
        <Button type="reset" fullWidth variant="outlined" sx={{ mt: 1, mb: 3 }}>
          CANCEL
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

export default Form;
