# calypso #


Registers custom cultures without administrative privileges, can be run with Azure SaaS.

Nuget: `Install-Package calypso`

## Usage ##

The default NuGet package will not associate itself to a project and is meant to be used in build strips, though there is no reason it cannot be invoked directly by csharp.


> ./Install.ps1

    Write-Host "Importing modules"
    Get-ChildItem "packages" -Recurse -File -Include "*.psm1" | % { Import-Module $_ -DisableNameChecking }
    
    RegisterAndBuild "usTest" "None" "en-US" "en-US-TEST"

This script imports the NuGet package, then creates a language, with the name "usTest"  with the code of "en-US-TEST" based of the "en-US" language. The region modifier is set to "None"

There are several other self-explanatory methods available as well:
    
    CustomCultureExists "usTest"
    ParseCultureAndRegion "None" # used for testing enum's validity.
    UnregisterCulture "usTest"

This is meant for simple use cases of copying a custom language based off another language but not needing to create custom language. Works with Azure SaaS (Web Roles/Web Sites).

## Resources ##

[MSDN - Creating custom cultures without administrative privileges](http://msdn.microsoft.com/en-us/library/vstudio/ms404375(v=vs.100).aspx)

## ToDo ##

- Expand to include loading languages from YAML, [example](https://github.com/vube/i18n/tree/master/data/rules).
