import * as React from "react";
import { SubmitHandler, useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import TextFieldInput from "../../Fields/FormInput/TextFieldInput";
import { useCreateToDoList } from "../../../hooks/queries/ToDoList";

const validationSchema = z.object({
  name: z.string().min(1, "Name is required"),
});

type ValidationSchema = z.infer<typeof validationSchema>;

function ToDoListCreateForm() {
  const { mutate: createToDoList } = useCreateToDoList();

  const defaultValues: ValidationSchema = {
    name: "",
  };

  const methods = useForm<ValidationSchema>({
    resolver: zodResolver(validationSchema),
    defaultValues,
  });

  const onSubmitHandler: SubmitHandler<ValidationSchema> = (
    data: ValidationSchema,
  ) => {
    try {
      createToDoList(data);
      methods.reset();
    } catch (error) {
      console.error("Error creating ToDo list:", error);
    }
  };

  return (
    <FormProvider {...methods}>
      <form onSubmit={methods.handleSubmit(onSubmitHandler)} noValidate>
        <TextFieldInput
          placeholder="Name of the list..."
          required
          id="name"
          label="Name"
          type="text"
          name="name"
        />
        <button type="submit" className="bg-ks-secondary-dark text-white">
          Create
        </button>
      </form>
    </FormProvider>
  );
}

export default ToDoListCreateForm;
