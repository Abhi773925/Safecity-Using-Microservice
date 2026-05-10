$projects = @(
    @{ Name = 'Gateway'; Path = 'SafeCity.Gateway\SafeCity.Gateway\SafeCity.Gateway.csproj' },
    @{ Name = 'IAM'; Path = 'SafeCity.IAM\SafeCity.IAM\SafeCity.IAM.csproj' },
    @{ Name = 'DCR'; Path = 'SafeCity.DCR\SafeCity.DCR\SafeCity.DCR.csproj' },
    @{ Name = 'EDRA'; Path = 'SafeCity.EDRA\SafeCity.EDRA\SafeCity.EDRA.csproj' },
    @{ Name = 'IRCM'; Path = 'SafeCity.IRCM\SafeCity.IRCM\SafeCity.IRCM.csproj' },
    @{ Name = 'PFOM'; Path = 'SafeCity.PFOM\SafeCity.PFOM\SafeCity.PFOM.csproj' }
)

$root = Split-Path -Parent $PSScriptRoot

foreach ($project in $projects) {
    $projectFile = Join-Path $root $project.Path

    if (-not (Test-Path $projectFile)) {
        Write-Warning "Skipping $($project.Name): project file not found at $projectFile"
        continue
    }

    $projectDirectory = Split-Path -Parent $projectFile
    $arguments = @(
        '-NoExit',
        '-Command',
        "Set-Location -LiteralPath '$projectDirectory'; dotnet watch run"
    )

    Start-Process -FilePath 'powershell.exe' -ArgumentList $arguments -WorkingDirectory $projectDirectory -WindowStyle Normal | Out-Null
}

Write-Host 'Started dotnet watch for the SafeCity services.'