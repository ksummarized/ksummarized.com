import * as React from "react";
import { useFormContext, Controller } from "react-hook-form";
import { TextField, TextFieldProps } from "@mui/material";

type FormInputProps = {
  name: string;
} & TextFieldProps;

function FormInput({ name, ...otherProps }: FormInputProps) {
  const {
    control,
    formState: { errors },
  } = useFormContext();

  return (
    <Controller
      control={control}
      name={name}
      defaultValue=""
      render={({ field }) => (
        <TextField
          {...field}
          {...otherProps}
          error={!!errors[name]}
          helperText={
            errors[name] ? (errors[name]?.message as unknown as string) : ""
          }
        />
      )}
    />
  );
}

export default FormInput;
