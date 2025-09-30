import * as React from "react";
import { useFormContext, Controller } from "react-hook-form";

interface TextareaInputProps
  extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
  id: string;
  name: string;
  required: boolean;
  label: string;
  placeholder: string;
}

function TextareaInput({
  id,
  name,
  required,
  label,
  placeholder,
  ...other
}: Readonly<TextareaInputProps>) {
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
        <div>
          <label
            htmlFor={id}
            className="block text-gray-700 text-sm font-bold mb-2"
          >
            {label}
            {required === true ? "*" : ""}
          </label>
          <textarea
            id={id}
            placeholder={placeholder}
            {...field}
            {...other}
            className="form-control w-full px-3 py-1.5 text-gray-700 rounded border border-solid border-gray-300 focus:border-yellow-600 focus:outline-none"
          />
          {!!errors[name] && (
            <p className="text-red-500 text-xs italic">{`${errors[name]?.message as string}`}</p>
          )}
        </div>
      )}
    />
  );
}

export default TextareaInput;
