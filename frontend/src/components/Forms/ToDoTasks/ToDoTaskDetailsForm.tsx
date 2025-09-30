import * as React from "react";
import { SubmitHandler, useForm, FormProvider } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import TextareaInput from "../../Fields/FormInput/TextareaInput";
import TextFieldInput from "../../Fields/FormInput/TextFieldInput";
import { useUpdateToDoTask } from "../../../hooks/queries/ToDoTask";
import { TodoItem } from "../../../client";
import CheckboxInput from "../../Fields/FormInput/CheckboxInput";
import TagsInput from "../../Fields/FormInput/TagsInput";
import { useGetAllToDoTags } from "../../../hooks/queries/ToDoTag";

const formatDateForInput = (isoString?: string | null) => {
  if (!isoString) {
    return "";
  }
  const date = new Date(isoString);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");
  const seconds = String(date.getSeconds()).padStart(2, "0");
  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
};

const validationSchema = z.object({
  name: z.string().min(1, "Name is required"),
  completed: z.boolean().optional(),
  deadline: z.string().optional().nullable(),
  notes: z.string().optional(),
  tags: z.array(z.object({ id: z.number(), name: z.string() })).optional(),
});

type ValidationSchema = z.infer<typeof validationSchema>;

type ToDoTaskDetailsFormProps = {
  item: TodoItem;
};
function ToDoTaskDetailsForm({ item }: Readonly<ToDoTaskDetailsFormProps>) {
  const { data: allTags, isLoading } = useGetAllToDoTags();
  const { mutate: updateToDoTask } = useUpdateToDoTask();

  const defaultValues: ValidationSchema = {
    name: item.name ?? "",
    completed: item.completed || false,
    deadline: formatDateForInput(item.deadline),
    notes: item.notes ?? "",
    tags: item.tags?.map((tag) => ({ id: tag.id!, name: tag.name! })) || [],
  };

  const methods = useForm<ValidationSchema>({
    resolver: zodResolver(validationSchema),
    defaultValues,
  });

  if (isLoading) {
    return <span className="loading loading-spinner loading-md" />;
  }
  const onSubmitHandler: SubmitHandler<ValidationSchema> = (
    data: ValidationSchema,
  ) => {
    try {
      if (data.deadline !== "" && data.deadline != null) {
        data.deadline = new Date(new Date(data.deadline)).toISOString();
      }
      if (data.deadline === "") {
        data.deadline = null; // Set to null if deadline is empty
      }
      updateToDoTask({
        ...item,
        ...data,
      });
    } catch (error) {
      console.error("Error updating ToDo task:", error);
    }
  };

  return (
    <FormProvider {...methods}>
      <form
        onSubmit={methods.handleSubmit(onSubmitHandler)}
        noValidate
        className="flex flex-col gap-2"
      >
        <TextFieldInput
          placeholder="Name of the task..."
          required
          id="name"
          label="Name"
          type="text"
          name="name"
        />
        <CheckboxInput id="completed" label="Completed" name="completed" />
        <TextFieldInput
          placeholder="Deadline"
          required={false}
          id="deadline"
          label="Deadline"
          type="datetime-local"
          name="deadline"
          step={1} // Allows seconds to be set
        />
        <TextareaInput
          placeholder="Notes ..."
          required={false}
          id="notes"
          label="Notes"
          name="notes"
        />
        <TagsInput
          id="tags"
          label="Tags"
          name="tags"
          required={false}
          options={allTags}
        />
        <button
          type="submit"
          className="bg-ks-secondary-dark text-white py-1 px-2 w-fit cursor-pointer"
        >
          Update
        </button>
      </form>
    </FormProvider>
  );
}

export default ToDoTaskDetailsForm;
