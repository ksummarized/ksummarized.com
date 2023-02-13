import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";

import logo from "../../assets/logo.png";
import Colors from "../../styles/Colors";

import LoginForm from "../../components/Forms/LoginForm/LoginForm";

export default function LoginPage(): JSX.Element {
  return (
    <Container component="main" maxWidth="sm">
      <CssBaseline />
      <Box
        sx={{
          marginTop: 8,
          paddingLeft: 8,
          paddingRight: 8,
          paddingBottom: 8,
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          background: Colors.firstLayer,
        }}
      >
        <Box
          component="img"
          sx={{
            height: 200,
            width: 200,
            maxHeight: { xs: 200, md: 167 },
            maxWidth: { xs: 200, md: 250 },
          }}
          src={logo}
        />
        <Typography component="h1" variant="h6">
          Just one step to organized life!
        </Typography>
        <LoginForm />
      </Box>
    </Container>
  );
}
