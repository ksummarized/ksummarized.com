import * as React from "react";
import { useFormContext, Controller } from "react-hook-form";

type CheckboxInputProps = {
  id: string;
  name: string;
  label: string;
};

function CheckboxInput({ id, name, label }: CheckboxInputProps) {
  const { control } = useFormContext();

  return (
    <Controller
      control={control}
      name={name}
      defaultValue=""
      render={({ field }) => (
        <div>
          <input id={id} type="checkbox" {...field} className="form-control" />
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
