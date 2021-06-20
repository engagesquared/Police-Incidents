# Install Azure PowerShell
# https://docs.microsoft.com/en-us/powershell/azure/install-az-ps?view=azps-5.2.0
# Install-Module -Name Az -AllowClobber -Scope CurrentUser

# Resource group should already exist
Param(
    [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
    [String]
    $resourceGroupName,

    [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
    [String]
    $subscriptionId
)

$ErrorActionPreference = "Stop"

Write-Host "Awaiting sign-in..."
$quiet = Connect-AzAccount -DeviceCode
Write-Host "Selecting subscription..."
$quiet = Set-AzContext -Subscription $subscriptionId

Write-Host "Applying ARM template..."
$deploymentResults = New-AzResourceGroupDeployment `
	-Name "$resourceGroupName-ServicesDeployment" `
	-ResourceGroupName $resourceGroupName `
	-TemplateFile .\azure-resources.json `
	-TemplateParameterFile .\azure-parameters.json `

Write-Host "";
Write-Host "App Domain: $($deploymentResults.Outputs.appDomain.Value)" -ForegroundColor Yellow;
Write-Host "";

Write-Host "Waiting for 2 minutes to ensure services are ready..."
Start-Sleep -Seconds (2 * 60)

$currentPath = (Get-Location).Path

Write-Host "Publishing Bot Web App..."
$pathToZip = "$currentPath\PoliceIncidents.Bot.zip"
$quiet = Publish-AzWebApp -ResourceGroupName $resourceGroupName -Name $deploymentResults.Outputs.botAppName.Value -ArchivePath $pathToZip -Force

Write-Host "Publishing Tab App..."
$pathToZip = "$currentPath\PoliceIncidents.Tab.zip"
$quiet = Publish-AzWebApp -ResourceGroupName $resourceGroupName -Name $deploymentResults.Outputs.tabWebAppName.Value -ArchivePath $pathToZip -Force

$quiet = Disconnect-AzAccount

Write-Host "Deployment completed."
