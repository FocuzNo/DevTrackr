$ErrorActionPreference = "SilentlyContinue"

function Get-DirSizeBytes {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        return 0
    }

    $sum = (Get-ChildItem -LiteralPath $Path -File -Force -Recurse -ErrorAction SilentlyContinue |
        Measure-Object -Property Length -Sum).Sum

    if ($null -eq $sum) {
        return 0
    }

    return [int64]$sum
}

function Clear-DirContents {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        return [pscustomobject]@{
            Path        = $Path
            BeforeGB    = 0
            AfterGB     = 0
            ReclaimedGB = 0
            Status      = "missing"
        }
    }

    $before = Get-DirSizeBytes -Path $Path

    Get-ChildItem -LiteralPath $Path -Force -ErrorAction SilentlyContinue | ForEach-Object {
        try {
            Remove-Item -LiteralPath $_.FullName -Recurse -Force -ErrorAction Stop
        }
        catch {
        }
    }

    $after = Get-DirSizeBytes -Path $Path

    [pscustomobject]@{
        Path        = $Path
        BeforeGB    = [math]::Round($before / 1GB, 2)
        AfterGB     = [math]::Round($after / 1GB, 2)
        ReclaimedGB = [math]::Round(($before - $after) / 1GB, 2)
        Status      = "cleaned"
    }
}

$cleanupTargets = @(
    "$env:LOCALAPPDATA\Temp",
    "$env:LOCALAPPDATA\NVIDIA\DXCache",
    "$env:LOCALAPPDATA\NVIDIA\GLCache",
    "$env:LOCALAPPDATA\npm-cache",
    "$env:LOCALAPPDATA\Perplexity\Comet\User Data\Default\Cache",
    "$env:LOCALAPPDATA\Perplexity\Comet\User Data\Default\Code Cache",
    "$env:LOCALAPPDATA\Yandex\YandexBrowser\User Data\Default\Cache",
    "$env:LOCALAPPDATA\Yandex\YandexBrowser\User Data\Default\Code Cache",
    "$env:LOCALAPPDATA\Yandex\YandexBrowser\User Data\Default\GPUCache",
    "$env:LOCALAPPDATA\Postman\Partitions"
)

$results = $cleanupTargets | ForEach-Object { Clear-DirContents -Path $_ }
$total = [math]::Round((($results | Measure-Object -Property ReclaimedGB -Sum).Sum), 2)

Write-Host ""
Write-Host "Cleanup results"
$results | Sort-Object ReclaimedGB -Descending | Format-Table -AutoSize
Write-Host ""
Write-Host ("Total reclaimed: {0} GB" -f $total)
Write-Host ""
Write-Host "Large non-cache directories to review manually:"

$reviewTargets = @(
    "$env:LOCALAPPDATA\Docker\wsl",
    "$env:LOCALAPPDATA\Android\Sdk\system-images",
    "$env:LOCALAPPDATA\Android\Sdk\ndk",
    "$env:LOCALAPPDATA\Packages\SpotifyAB.SpotifyMusic_zpdnekdrzrea0",
    "$env:LOCALAPPDATA\Programs"
)

$reviewTargets | ForEach-Object {
    [pscustomobject]@{
        Path   = $_
        SizeGB = [math]::Round((Get-DirSizeBytes -Path $_) / 1GB, 2)
    }
} | Sort-Object SizeGB -Descending | Format-Table -AutoSize

