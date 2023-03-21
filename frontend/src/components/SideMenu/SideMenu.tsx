import * as React from "react";
import Grid from "@mui/material/Grid";
import Drawer, { DrawerProps } from "@mui/material/Drawer";
import Toolbar from "@mui/material/Toolbar";
import Tooltip from "@mui/material/Tooltip";
import IconButton from "@mui/material/IconButton";
import ToDoListIcon from "@mui/icons-material/FormatListBulleted";
import CalendarIcon from "@mui/icons-material/DateRange";
import OrganizerIcon from "@mui/icons-material/Dashboard";

import Colors from "../../styles/Colors";

const sideMenuWidth = 64;

function SideMenu({ ...props }: DrawerProps) {
  return (
    <Drawer
      {...props}
      variant="permanent"
      PaperProps={{
        sx: {
          background: Colors.firstLayer,
          borderRight: 2,
          borderColor: "black",
          boxSizing: "border-box",
          minWidth: sideMenuWidth,
        },
      }}
      sx={{ flexShrink: 0, minWidth: sideMenuWidth }}
    >
      <Toolbar />
      <Grid
        container
        direction="column"
        overflow="auto"
        paddingTop={3}
        justifyContent="center"
        alignItems="center"
        spacing="30px"
      >
        <Grid key="toDoList" item>
          <Tooltip title="ToDoList">
            <IconButton aria-label="toDoList">
              <ToDoListIcon
                sx={{
                  fill: Colors.primary,
                }}
              />
            </IconButton>
          </Tooltip>
        </Grid>
        <Grid key="calendar" item>
          <Tooltip title="Calendar">
            <IconButton aria-label="calendar">
              <CalendarIcon
                sx={{
                  fill: Colors.primary,
                }}
              />
            </IconButton>
          </Tooltip>
        </Grid>
        <Grid key="organizer" item>
          <Tooltip title="Organizer">
            <IconButton aria-label="organizer">
              <OrganizerIcon
                sx={{
                  fill: Colors.primary,
                }}
              />
            </IconButton>
          </Tooltip>
        </Grid>
      </Grid>
    </Drawer>
  );
}

export default SideMenu;
