import * as React from "react";
import { useFormContext, Controller } from "react-hook-form";
import { Checkbox, CheckboxProps } from "@mui/material";

type CheckboxInputProps = {
  name: string;
} & CheckboxProps;

function CheckboxInput({ name, ...otherProps }: CheckboxInputProps) {
  const { control } = useFormContext();

  return (
    <Controller
      control={control}
      name={name}
      defaultValue=""
      render={({ field }) => <Checkbox {...field} {...otherProps} />}
    />
  );
}

export default CheckboxInput;
