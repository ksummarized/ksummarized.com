import { createTheme } from "@mui/material/styles";
import Colors from "./Colors";

const darkTheme = createTheme({
  palette: {
    mode: "dark",
    primary: {
      main: Colors.primary,
    },
    secondary: {
      main: Colors.secondary,
    },
    background: {
      default: Colors.background,
    },
    error: {
      main: Colors.error,
    },
    text: {
      primary: Colors.text,
    },
  },
});

export default darkTheme;
