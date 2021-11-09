param(
    [Parameter(Mandatory = $true)]
    [string]$Reviewer,
    [Parameter(Mandatory = $true)]
    [string]$BodyFile
)

$branches = (git branch -l)
$current_branch = $branches.Where({ $_.StartsWith('*') })
$issue_number = $current_branch.Substring(2, 5).TrimStart('0')

gh pr create -a `@me -r $Reviewer -t "Closes #$($issue_number)" -B master -F $BodyFile