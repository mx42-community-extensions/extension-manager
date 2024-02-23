$ErrorActionPreference = "Stop"

#########
$AssemblyName = "ExtensionManager"
$ExtensionName = "Extension Manager"
$ExtensionDescription = "Extension Manager"
$ExtensionId = "00000000-1337-1337-1234-000000000000"
#########

Write-Host "Starting packaging of the extension"

$packageFolder = "$($env:OutputPath)Package\"
$assembliesFolder = "$($packageFolder)Assemblies\"

if(Test-Path -PathType Container $packageFolder){
    Remove-Item -Recurse $packageFolder -Force | Out-Null
}
New-Item -ItemType Directory $packageFolder -Force | Out-Null

Write-Host "Copying assemblies"
@(
    "$($assembliesFolder)svc\bin", 
    "$($assembliesFolder)bin", 
    "$($assembliesFolder)ServiceRepository\BinaryComponents"
) | ForEach-Object {
    New-Item -ItemType Directory $_ -Force | Out-Null
    Copy-Item "$($env:OutputPath)$AssemblyName.dll" $_
}

if(Test-Path -PathType Leaf "$($env:ProjectDir)package.json"){
    Write-Host "Building frontend"
    $frontendFolder = "$($packageFolder)Files\WM\workspaces\$AssemblyName\"
    New-Item -ItemType Directory $frontendFolder -Force | Out-Null

    if($(Start-Process -WorkingDirectory $env:ProjectDir -wait -PassThru -nonewwindow "npm" -ArgumentList @("install")).ExitCode -ne 0){
        Write-Error "Frontend installation failed with exit code $($process.ExitCode)"
    }

    if($(Start-Process -WorkingDirectory $env:ProjectDir -wait -PassThru -nonewwindow "npm" -ArgumentList @("run-script", "build")).ExitCode -ne 0){
        Write-Error "Frontend build failed with exit code $($process.ExitCode)"
    }

    Copy-item "$($env:ProjectDir)dist/module.js" $frontendFolder -Force | Out-Null

    @{
      "description" = @{
        "name" = "$AssemblyName"
        "title" = "$AssemblyName"
      }
      "resources" = @( "module.js" )
    } | ConvertTo-Json | Out-File "$($frontendFolder)workspace.json" -Encoding utf8
}


Write-Host "Copying schema files"
$installFolder = "$($packageFolder)Install"
New-Item -ItemType Directory $installFolder -Force | Out-Null
Copy-Item "$($env:ProjectDir)Config\*" $installFolder

Write-Host "Creating schema installation manifest"

$oldWorkingDirectory = Get-Location
Set-Location $packageFolder
& "$($env:ProjectDir)Tools/CreateSchemaFile.exe" @( "install", "Install.xml", "install" ) | Out-Null
Set-Location $oldWorkingDirectory

Write-Host "Creating extension manifest"
$version = (Get-Item "$($env:OutputPath)$AssemblyName.dll").VersionInfo.ProductVersion
@{
    "Id" = $ExtensionId
    "Version" = $version
    "LastUpdatedDate" =  $(Get-Date -Format s).ToString()
    "Description" =  $ExtensionDescription
    "Prerequisites" =  @{
        "MinimalRequiredProductVersion" =  "12.0.3"
    }
    "Name" =  $ExtensionName
    "SetupDirectives" = @{
        "RecycleWebApplication" = $true
        "RestartM42WindowsServices" = $true
        "InstallationPSFiles" = $null
        "MaintenanceMode" = $false
    }
    "Vendor" =  "Matrix42 Community Extensions"
} | ConvertTo-Json | Out-File "$($packageFolder)package.json" -Encoding utf8

$packageZip = "$($env:OutputPath)$AssemblyName-$version.zip"
Compress-Archive -Path "$packageFolder*" -DestinationPath $packageZip -Force | Out-Null

#Optional cleanup
#Remove-Item $packageFolder -Force -Recurse

Write-Host "Done creating package to $packageZip"