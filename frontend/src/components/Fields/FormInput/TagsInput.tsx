import * as React from "react";
import { useFormContext, Controller } from "react-hook-form";
import { Tag } from "../../../client";
import { useCreateToDoTag } from "../../../hooks/queries/ToDoTag";

interface TagsInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  id: string;
  name: string;
  required: boolean;
  label: string;
  placeholder?: string;
  options?: Tag[];
}

function TagsInput({
  id,
  name,
  required,
  label,
  placeholder = "Type and press Enter to add a new tag",
  options = [],
  ...other
}: Readonly<TagsInputProps>) {
  const [inputValue, setInputValue] = React.useState("");
  const {
    control,
    formState: { errors },
  } = useFormContext();

  const { mutateAsync: createTag } = useCreateToDoTag();

  return (
    <Controller
      control={control}
      name={name}
      defaultValue={[]}
      render={({ field }) => {
        const isSelected = (tagId: number) => {
          return field.value.some((t: Tag) => t.id === tagId);
        };

        const unselectTag = (tag: Tag) => {
          field.onChange(field.value.filter((t: Tag) => t.id !== tag.id));
        };

        const selectTag = (tag: Tag) => {
          if (!isSelected(tag.id!)) {
            field.onChange([...field.value, { id: tag.id, name: tag.name }]);
          } else {
            unselectTag(tag);
          }
        };

        const handleKeyDown = async (
          e: React.KeyboardEvent<HTMLInputElement>,
        ) => {
          if (e.key === "Enter") {
            e.preventDefault();
            const newTag = await createTag({ name: inputValue.trim() });
            selectTag(newTag);
            setInputValue("");
          }
        };

        return (
          <div className="form-control">
            <label htmlFor={id} className="label">
              <span className="text-gray-700 text-sm font-bold mb-2">
                {label}
                {required ? " *" : ""}
              </span>
            </label>

            <div className="border border-gray-300 rounded p-2 min-h-[44px] flex flex-wrap gap-2 items-center">
              {field.value.map((tag: Tag) => (
                <div
                  key={tag.id}
                  className="badge badge-primary gap-1 cursor-default"
                >
                  {tag.name}
                  <button
                    type="reset"
                    onClick={() => unselectTag(tag)}
                    className="ml-1 text-white cursor-pointer"
                  >
                    ✕
                  </button>
                </div>
              ))}
              <div className="relative w-full flex flex-col gap-2">
                <div className="flex flex-col items-start z-10 mt-1 w-full bg-white border border-gray-300 rounded shadow max-h-48 overflow-y-auto text-black">
                  {options.map((option) => (
                    <button
                      key={option.id}
                      type="reset"
                      className={`px-3 py-2 cursor-pointer hover:bg-gray-100 w-full text-left ${isSelected(option.id!) ? "bg-gray-200" : ""}`}
                      onClick={() => {
                        selectTag(option);
                      }}
                    >
                      {option.name!}
                    </button>
                  ))}
                </div>
                <input
                  id={id}
                  value={inputValue}
                  onChange={(e) => {
                    setInputValue(e.target.value);
                  }}
                  onKeyDown={handleKeyDown}
                  placeholder={placeholder}
                  {...other}
                  className="input input-bordered w-full text-sm text-black"
                />
              </div>
            </div>

            {!!errors[name] && (
              <p className="text-red-500 text-xs italic mt-1">
                {errors[name]?.message as string}
              </p>
            )}
          </div>
        );
      }}
    />
  );
}

export default TagsInput;
