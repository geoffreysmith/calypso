
if (Test-Path "build") {
  Remove-Item "build" -Recurse -Force
}

mkdir build
nuget\nuget pack calypso.nuspec -OutputDirectory build
