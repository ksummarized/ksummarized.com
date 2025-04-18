import * as React from "react";
import { SubmitHandler, useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import TextFieldInput from "../../Fields/FormInput/TextFieldInput";
import { useCreateToDoTask } from "../../../hooks/queries/ToDoList";

const validationSchema = z.object({
  name: z.string().min(1, "Name is required"),
});

type ValidationSchema = z.infer<typeof validationSchema>;

type ToDoTaskCreateFormProps = {
  listId: number;
};
function ToDoTaskCreateForm({ listId }: Readonly<ToDoTaskCreateFormProps>) {
  const { mutate: createToDoTask } = useCreateToDoTask();

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
      createToDoTask({ ...data, listId, tags: [], subtasks: [] });
      methods.reset();
    } catch (error) {
      console.error("Error creating ToDo task:", error);
    }
  };

  return (
    <FormProvider {...methods}>
      <form onSubmit={methods.handleSubmit(onSubmitHandler)} noValidate>
        <TextFieldInput
          placeholder="Name of the task..."
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

export default ToDoTaskCreateForm;
