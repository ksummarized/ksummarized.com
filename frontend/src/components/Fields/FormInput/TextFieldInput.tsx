import * as React from "react";
import { useFormContext, Controller } from "react-hook-form";
import { TextField, TextFieldProps } from "@mui/material";

type TextFieldInputProps = {
  name: string;
} & TextFieldProps;

function TextFieldInput({ name, ...otherProps }: TextFieldInputProps) {
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

export default TextFieldInput;
