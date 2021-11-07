param(
    [Parameter(Mandatory = $true)]
    [string]$reviewer,
    [Parameter(Mandatory = $true)]
    [string]$bodyFile
)

$branchs = (git branch -l)
$current_branch = $branchs.Where({ $_.StartsWith('*') })
$issue_number = $current_branch.Substring(2, 5).TrimStart('0')

gh pr create -a `@me -r $reviewer -t "Closes #$($issue_number)" -B master -F $bodyFile