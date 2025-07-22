import * as React from "react";
import { useFormContext, Controller } from "react-hook-form";

interface CheckboxInputProps
  extends React.InputHTMLAttributes<HTMLInputElement> {
  id: string;
  name: string;
  label: string;
}

function CheckboxInput({
  id,
  name,
  label,
  ...other
}: Readonly<CheckboxInputProps>) {
  const { control } = useFormContext();

  return (
    <Controller
      control={control}
      name={name}
      defaultValue=""
      render={({ field }) => (
        <div className="flex flex-row items-center gap-2">
          <input
            id={id}
            type="checkbox"
            {...field}
            {...other}
            className="form-control w-6 h-6"
          />
          <label
            htmlFor={id}
            className="block text-gray-700 text-sm font-bold mb-2"
          >
            {label}
          </label>
        </div>
      )}
    />
  );
}

export default CheckboxInput;
