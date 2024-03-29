#!/usr/bin/env pwsh
$branches = (git branch -l)
foreach ($branch in $branches) {
    $branch_name = $branch.Trim()
    if (!($branch_name -eq "master" -or $branch_name.StartsWith("*"))) {
        git branch -D $branch_name
    }
}