import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";

import logo from "../../assets/logo.png";
import Colors from "../../styles/Colors";
import ShowAndHidePasswordFilledField from "../../components/ShowAndHidePasswordFilledField/ShowAndHidePasswordFilledField";

export default function RegisterForm(): JSX.Element {
  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
  };

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
        <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
          <TextField
            margin="normal"
            required
            fullWidth
            id="email"
            label="Email Address"
            name="email"
            autoComplete="email"
            autoFocus
            variant="filled"
          />
          <ShowAndHidePasswordFilledField sx={{ mb: 1 }} />
          <ShowAndHidePasswordFilledField label="Confirm password" />
          <Button type="submit" fullWidth variant="contained" sx={{ mt: 6 }}>
            REGISTER
          </Button>
          <Button
            type="reset"
            fullWidth
            variant="outlined"
            sx={{ mt: 1, mb: 3 }}
          >
            CANCEL
          </Button>
        </Box>
      </Box>
    </Container>
  );
}
