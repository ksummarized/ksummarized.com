module.exports = {
  env: {
    browser: true,
    es2021: true,
    jest: true,
  },
  extends: [
    "plugin:@typescript-eslint/recommended",
    "airbnb",
    "plugin:react/recommended",
    "plugin:prettier/recommended",
  ],
  globals: {
    React: true,
    JSX: true,
    jest: true,
  },
  parser: "@typescript-eslint/parser",
  parserOptions: {
    ecmaFeatures: {
      jsx: true,
    },
    ecmaVersion: 12,
    sourceType: "module",
  },
  plugins: ["react", "@typescript-eslint", "jest", "prettier"],
  settings: {
    "import/resolver": {
      typescript: {},
    },
  },
  rules: {
    "prettier/prettier": "error",
    "no-use-before-define": "off",
    "@typescript-eslint/no-use-before-define": ["error"],
    "react/jsx-filename-extension": ["warn", { extensions: [".tsx"] }],
    "import/extensions": [
      "error",
      "ignorePackages",
      {
        ts: "never",
        tsx: "never",
      },
    ],
    "no-shadow": "off",
    "@typescript-eslint/no-shadow": ["error"],
    "no-unused-vars": "off",
  },
};
