function Load-CalypsoLibraries([string] $dllPath, $shadowPath = "$($env:TEMP)\calypso\shadow") {
    gci $dllPath | % {
        $shadow = Shadow-Copy -File $_.FullName -ShadowPath $shadowPath
        [Reflection.Assembly]::LoadFrom($shadow)
    } | Out-Null
}

function ParseCultureAndRegion([string] $cultureName) {
    $registerCulture = New-Object Calypso.RegisterCulture

    return $registerCulture.ParseCultureAndRegion($cultureName)
}

function CustomCultureExists([string] $cultureName) {
     $registerCulture = New-Object Calypso.RegisterCulture

    return $registerCulture.CustomCultureExists($cultureName)
}

function RegisterAndBuild([string] $cultureName,
            [string] $cultureAndRegionModifier,
            [string] $existingRegionIsoCode,
            [string] $newRegionIsoCode) {    
    $registerCulture = New-Object Calypso.RegisterCulture

    $registerCulture.RegisterAndBuild($cultureName, $cultureAndRegionModifier, $existingRegionIsoCode, $newRegionIsoCode)
}

function CustomCultureExists([string] $cultureName) {
     $registerCulture = New-Object Calypso.RegisterCulture

    return $registerCulture.CustomCultureExists($cultureName)
}

Export-ModuleMember -Function ParseCultureAndRegion
Export-ModuleMember -Function CustomCultureExists
Export-ModuleMember -Function RegisterAndBuild