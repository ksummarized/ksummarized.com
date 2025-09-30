import React from "react";

interface PlusIconProps {
  fill?: string;
  viewBox?: string;
  strokeWidth?: number;
  stroke?: string;
  className?: string;
}
export const PlusIcon = ({
  fill = "none",
  viewBox = "0 0 24 24",
  strokeWidth = 1.5,
  stroke = "currentColor",
  className = "size-6",
}: PlusIconProps) => {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      fill={fill}
      viewBox={viewBox}
      strokeWidth={strokeWidth}
      stroke={stroke}
      className={className}
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        d="M12 4.5v15m7.5-7.5h-15"
      />
    </svg>
  );
};
