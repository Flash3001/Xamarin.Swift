<#
    .SYNOPSIS  
        This script

        Update nuspec, 
        Create package,
        Send package 
#>
param(
    [Parameter(Mandatory=$true)][string]$version,
    [Parameter(Mandatory=$true)][string]$releaseNotes,
    [Boolean]$sendPackage
)

$baseNuspec = Get-Content base.nuspec

foreach($folder in Get-ChildItem Xamarin.Swift4.* -Attributes Directory)
{
    $id = $folder.Name
    
    Set-Location $id
    Write-Host $id -ForegroundColor Yellow

    $lib = (Get-ChildItem ("Frameworks/*dylib" -f  $id) | Select-Object -First 1).Name
    $nuspec = "{0}.nuspec" -f  $id
    $package = "{0}.{1}.nupkg" -f $id, $version

    $baseNuspec.Replace('$id$', $id).Replace('$lib$', $lib).Replace('$version$', $version).Replace('$releaseNotes$', $releaseNotes) |
    Out-File $nuspec

    nuget pack $nuspec
    if ($sendPackage) {
        nuget push $package -Source https://www.nuget.org/api/v2/package -Timeout 1200
    }

    Set-Location ..
    Write-Output `n
}
