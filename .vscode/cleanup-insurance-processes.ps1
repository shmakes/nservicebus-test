$processes = Get-CimInstance Win32_Process | Where-Object {
    ($_.Name -eq "dotnet.exe" -and ($_.CommandLine -like "*Insurance.Api*" -or $_.CommandLine -like "*Insurance.Workflows*")) -or
    ($_.Name -eq "netcoredbg.exe" -and ($_.CommandLine -like "*Insurance.Api*" -or $_.CommandLine -like "*Insurance.Workflows*"))
}

if ($processes) {
    $processes | ForEach-Object { Stop-Process -Id $_.ProcessId -Force }
}
