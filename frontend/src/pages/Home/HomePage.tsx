import * as React from "react";
import Box from "@mui/material/Box";
import IconButton from "@mui/material/IconButton";
import Toolbar from "@mui/material/Toolbar";
import CssBaseline from "@mui/material/CssBaseline";
import Typography from "@mui/material/Typography";
import Tooltip from "@mui/material/Tooltip";
import AddIcon from "@mui/icons-material/Add";

import undrawRelaxingAtHome from "../../assets/images/undrawRelaxingAtHome.svg";
import TopBar from "../../components/TopBar/TopBar";
import SideMenu from "../../components/SideMenu/SideMenu";
import Colors from "../../styles/Colors";

export default function HomePage(): JSX.Element {
  return (
    <Box sx={{ display: "flex" }}>
      <CssBaseline />
      <TopBar />
      <SideMenu />
      <Box
        component="main"
        sx={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          flexGrow: 1,
          paddingTop: 10,
          height: "100vh",
          background: Colors.thirdLayer,
        }}
      >
        <Toolbar />
        <Box
          component="img"
          sx={{
            opacity: 0.4,
            height: "auto",
            maxWidth: "100%",
            padding: 0,
            margin: 0,
          }}
          src={undrawRelaxingAtHome}
        />
        <Typography component="h1" variant="h6" fontSize={36} marginTop="30px">
          Nothing to do. Time to relax!
        </Typography>
      </Box>
      <Tooltip title="Add">
        <IconButton
          size="large"
          sx={{
            position: "fixed",
            bottom: "5%",
            right: "5%",
            background: Colors.primary,
          }}
        >
          <AddIcon />
        </IconButton>
      </Tooltip>
    </Box>
  );
}
