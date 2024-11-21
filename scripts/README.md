# Scripts

This directory contains scripts which automate common tasks in this repository.
Right now there are this scripts:

- `create_branch.ps1`
- `clean_branches.ps1`
- `create_pr.ps1`

Two out of those (`create_branch.ps1` and `create_pr.ps1`) require user to have GitHub CLI installed and to be logged in.

## Usage

- `create_pr.ps1` - creates a PR in this repo.

  - Params:

    - Reviewer [string] (required) - the person assigned as a reviewer for this PR.
    - BodyFile [file path] (required) - a path to the file with content for PR description written in markdown.

  - Examples:

    - `.\scripts\create_pr.ps1 -Reviewer sojusan -BodyFile .\text.md`

- `create_branch.ps1` - it creates branch which follows naming convention based on issue number and title.

  - Params:

    - Assignee [string] (defaults to current user) - person who's issues will be presented as options

  - Examples:
    - `.\scripts\create_branch.ps1 -Assignee sojusan`

- `clean_branches.ps1` - clears all of the local branches except for master and the current one.
