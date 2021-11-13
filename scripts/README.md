# Scripts

This directory contains scripts which automate common tasks in this repository.
Right now there are this scripts:

- `create_branch.ps1`
- `clean_branches.ps1`
- `create_pr.ps1`

Two out of those (`create_branch.ps1` and `create_pr.ps1`) require user to have GitHub CLI installed and to be logged in.

## Usage:

- `create_branch.ps1` it creates branch which follows naming convention based on issue number and title.
  By default it asks based on issues assigned to the user. This can be changed with `-Assignee` flag.
- `clean_branches.ps1` clears all of the local branches except for master and the current one.
- `create_pr.ps1` creates a PR in this repo
