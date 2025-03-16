import globals from "globals";
import pluginJs from "@eslint/js";
import { configs as tseslint } from "typescript-eslint";
import pluginReact from "eslint-plugin-react";
import eslintPluginPrettierRecommended from "eslint-plugin-prettier/recommended";
import typescriptParser from "@typescript-eslint/parser";
import vitest from "@vitest/eslint-plugin";
import importPlugin from "eslint-plugin-import";

/** @type {import('eslint').Linter.Config[]} */
export default [
  { ignores: ["**/*.csv", "**/*.svg", "**/*.png", "**/src/client/*"] },
  { files: ["**/*.{js,mjs,cjs,ts,jsx,tsx}"] },
  { files: ["**/*.js"], languageOptions: { sourceType: "script" } },
  {
    languageOptions: {
      globals: {
        ...globals.browser,
        ...globals.es2021,
        React: true,
        JSX: true,
      },
      ecmaVersion: 12,
      parser: typescriptParser,
      parserOptions: {
        ecmaFeatures: {
          jsx: true,
        },
        ecmaVersion: 12,
        sourceType: "module",
      },
    },
  },
  pluginJs.configs.recommended,
  ...tseslint.recommended,
  pluginReact.configs.flat.recommended,
  pluginReact.configs.flat["jsx-runtime"],
  vitest.configs.recommended,
  importPlugin.flatConfigs.recommended,
  importPlugin.flatConfigs.typescript,
  {
    settings: {
      react: {
        version: "detect",
      },
      "import/resolver": {
        node: {
          extensions: [".js", ".jsx", ".ts", ".tsx"],
        },
        typescript: {},
      },
    },
    rules: {
      indent: ["error", 2],
      "no-use-before-define": "off",
      "no-shadow": "off",
      "no-undef": "off",
      "no-unused-vars": "off",
      "no-param-reassign": ["error", { props: false }],
      "@typescript-eslint/no-use-before-define": ["error"],
      "react/jsx-filename-extension": ["warn", { extensions: [".tsx"] }],
      "react/jsx-props-no-spreading": "off",
      "@typescript-eslint/no-shadow": ["error"],
      "prettier/prettier": [
        "error",
        {
          endOfLine: "auto",
        },
      ],
      "import/extensions": [
        "error",
        "ignorePackages",
        {
          ts: "never",
          tsx: "never",
        },
      ],
      "global-require": "off",
      "import/no-extraneous-dependencies": [
        "error",
        {
          devDependencies: true,
        },
      ],
      "react/jsx-uses-vars": "error",
      "react/jsx-uses-react": "error",
    },
  },
  eslintPluginPrettierRecommended,
];
