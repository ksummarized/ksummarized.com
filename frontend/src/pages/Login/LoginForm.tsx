import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import FormControl from "@mui/material/FormControl";
import FormControlLabel from "@mui/material/FormControlLabel";
import Checkbox from "@mui/material/Checkbox";
import Link from "@mui/material/Link";
import Grid from "@mui/material/Grid";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import InputAdornment from "@mui/material/InputAdornment";
import InputLabel from "@mui/material/InputLabel";
import IconButton from "@mui/material/IconButton";
import FilledInput from "@mui/material/FilledInput";

import logo from "../../assets/logo.png";
import googleLogo from "../../assets/googleLogo.svg";
import githubLogo from "../../assets/githubLogo.svg";
import facebookLogo from "../../assets/facebookLogo.svg";
import twitterLogo from "../../assets/twitterLogo.svg";
import Colors from "../../styles/Colors";

interface State {
  password: string;
  showPassword: boolean;
}

export default function LoginForm(): JSX.Element {
  const [values, setValues] = React.useState<State>({
    password: "",
    showPassword: false,
  });

  const handleChange =
    (prop: keyof State) => (event: React.ChangeEvent<HTMLInputElement>) => {
      setValues({ ...values, [prop]: event.target.value });
    };

  const handleClickShowPassword = () => {
    setValues({
      ...values,
      showPassword: !values.showPassword,
    });
  };

  const handleMouseDownPassword = (
    event: React.MouseEvent<HTMLButtonElement>
  ) => {
    event.preventDefault();
  };

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
          <FormControl required fullWidth variant="filled">
            <InputLabel htmlFor="filled-adornment-password">
              Password
            </InputLabel>
            <FilledInput
              id="password"
              name="password"
              type={values.showPassword ? "text" : "password"}
              value={values.password}
              onChange={handleChange("password")}
              endAdornment={
                <InputAdornment position="end">
                  <IconButton
                    aria-label="toggle password visibility"
                    onClick={handleClickShowPassword}
                    onMouseDown={handleMouseDownPassword}
                    edge="end"
                  >
                    {values.showPassword ? <VisibilityOff /> : <Visibility />}
                  </IconButton>
                </InputAdornment>
              }
            />
          </FormControl>
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
