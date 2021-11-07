param(
    [string]$assignee
)

$issue_titles = [ordered]@{}
$issues = ""
if ($assignee -eq "") {
    $issues = (gh issue list --json "number,title" | ConvertFrom-Json)
}
else {
    $issues = (gh issue list -a $assignee --json "number,title" | ConvertFrom-Json)
}
if ($issues.Length -eq 0) {
    exit
}

Write-Host "Nr.`tBranch"
foreach ($issue in $issues) {
    $issue_number = "$($issue.number)".PadLeft(5, '0')
    $branch = ("$($issue_number)_$($issue.title)").Replace(" ", "_").Replace(":", "")
    $issue_titles["$($issue.number)"] = $branch
    Write-Host "$($issue.number)`t$branch"
}

$choice = Read-Host -Prompt "Please select an issue on which You want to work"
git checkout -b $issue_titles[$choice]