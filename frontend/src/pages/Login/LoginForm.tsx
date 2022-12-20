import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import FormControlLabel from "@mui/material/FormControlLabel";
import Checkbox from "@mui/material/Checkbox";
import Link from "@mui/material/Link";
import Grid from "@mui/material/Grid";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";

import logo from "../../assets/logo.png";
import googleLogo from "../../assets/googleLogo.svg";
import githubLogo from "../../assets/githubLogo.svg";
import facebookLogo from "../../assets/facebookLogo.svg";
import twitterLogo from "../../assets/twitterLogo.svg";
import Colors from "../../styles/Colors";
import ShowAndHidePasswordFilledField from "../../components/ShowAndHidePasswordFilledField/ShowAndHidePasswordFilledField";

export default function LoginForm(): JSX.Element {
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
            sx={{ mb: 3 }}
          />
          <ShowAndHidePasswordFilledField />
          <FormControlLabel
            control={<Checkbox value="remember" color="primary" />}
            label="Remember me"
          />
          <Typography component="h1" variant="h6" align="center">
            Don&apos;t have an account?&nbsp;
            <Link href="/register" color="secondary">
              Sign Up
            </Link>
          </Typography>
          <Button
            type="submit"
            fullWidth
            variant="contained"
            sx={{ mt: 3, mb: 2 }}
          >
            LOGIN
          </Button>
          <Grid container spacing={5} paddingTop={2} justifyContent="center">
            <Grid item>
              <Box
                component="img"
                sx={{
                  height: 40,
                  width: 40,
                }}
                src={googleLogo}
              />
            </Grid>
            <Grid item>
              <Box
                component="img"
                sx={{
                  height: 40,
                  width: 40,
                }}
                src={githubLogo}
              />
            </Grid>
            <Grid item>
              <Box
                component="img"
                sx={{
                  height: 40,
                  width: 40,
                }}
                src={facebookLogo}
              />
            </Grid>
            <Grid item>
              <Box
                component="img"
                sx={{
                  height: 40,
                  width: 40,
                }}
                src={twitterLogo}
              />
            </Grid>
          </Grid>
        </Box>
      </Box>
    </Container>
  );
}
