import * as React from "react";
import AppBar, { AppBarProps } from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Link from "@mui/material/Link";
import Toolbar from "@mui/material/Toolbar";

import logo from "../../assets/logos/logo.png";
import Colors from "../../styles/Colors";

function TopBar({ ...props }: AppBarProps) {
  return (
    <Box sx={{ flexGrow: 0 }}>
      <AppBar
        {...props}
        position="fixed"
        sx={{
          background: Colors.background,
          borderBottom: 2,
          borderColor: "black",
          zIndex: (theme) => theme.zIndex.drawer + 1,
        }}
      >
        <Toolbar>
          <Link href="/">
            <Box
              component="img"
              sx={{
                height: 64,
                width: 64,
              }}
              src={logo}
            />
          </Link>
        </Toolbar>
      </AppBar>
    </Box>
  );
}

export default TopBar;
