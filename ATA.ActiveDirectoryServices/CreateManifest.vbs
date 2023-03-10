Option Explicit
Dim oFS, oManifest, args, oFestures, iFeatureCount, iDWPCount, iTemplateFileCount

dim arFeatures(100)
iFeatureCount = 0

dim arDWPFiles(100)
iDWPCount = 0

dim arTemplateFiles(100)
iTemplateFileCount = 0

set args = WScript.Arguments

' Create the File System Object
Set oFS = CreateObject("Scripting.FileSystemObject")

Set oManifest = oFS.CreateTextFile(args(0) + "manifest.xml", true)
oManifest.WriteLine "<Solution xmlns='http://schemas.microsoft.com/sharepoint/' SolutionId='1C477B4E-F766-4ac9-82B8-3E465B660E3B'>"

dim oFile, oGacFolder, oBinFolder, oDWPFolder
set oGacFolder = oFS.GetFolder(args(0) + "DLLS\GAC")
set oBinFolder = oFS.GetFolder(args(0) + "DLLS")

            dim sSafeControls, oSafeXml

if (oGacFolder.Files.Count > 0 or oBinFolder.Files.Count > 0) then
    oManifest.WriteLine "    <Assemblies>"

    for each oFile in oGacFolder.Files
        oManifest.WriteLine "        <Assembly DeploymentTarget='GlobalAssemblyCache' Location='GAC\" + oFile.Name + "'>"
        if (oFS.FileExists(args(0) + "DLLS\SafeControls\" + oFile.Name + ".xml")) then
            set oSafeXml = oFS.OpenTextFile(args(0) + "DLLS\SafeControls\" + oFile.Name + ".xml")
            
            sSafeControls = oSafeXml.ReadAll
            oManifest.Write Mid(sSafeControls, 4)
            
            oManifest.WriteBlankLines 1
            oSafeXml.Close
        end if
        oManifest.WriteLine "        </Assembly>"
    next

    for each oFile in oBinFolder.Files
        oManifest.WriteLine "        <Assembly DeploymentTarget='WebApplication' Location='" + oFile.Name + "'>"
        if (oFS.FileExists(args(0) + "DLLS\SafeControls\" + oFile.Name + ".xml")) then
            set oSafeXml = oFS.OpenTextFile(args(0) + "DLLS\SafeControls\" + oFile.Name + ".xml")
            
            sSafeControls = oSafeXml.ReadAll
            oManifest.Write Mid(sSafeControls, 4)
            
            oManifest.WriteBlankLines 1
            oSafeXml.Close
        end if
        oManifest.WriteLine "        </Assembly>"
    next

    oManifest.WriteLine "    </Assemblies>"
end if


EnumFolder args(0) + "TEMPLATE", "TEMPLATE"


if (iDWPCount > 0) then
    oManifest.WriteLine "    <DwpFiles>"

    dim sDWPFile
    for each sDWPFile in arDWPFiles
        if (sDWPFile <> "") then oManifest.WriteLine sDWPFile
    next
    
    oManifest.WriteLine "    </DwpFiles>"
end if

if (iTemplateFileCount > 0) then

oManifest.WriteLine "    <TemplateFiles>"

dim sTemplateFile

for each sTemplateFile in arTemplateFiles
    if (sTemplateFile <> "") then oManifest.WriteLine sTemplateFile
next

oManifest.WriteLine "    </TemplateFiles>"
end if

if (iFeatureCount > 0) then

oManifest.WriteLine "    <FeatureManifests>"

' Add the features
dim sFeature

for each sFeature in arFeatures
    if (sFeature <> "") then oManifest.WriteLine sFeature
next

oManifest.WriteLine "    </FeatureManifests>"
end if

if (oFS.FileExists(args(0) + "cas.xml")) then
    dim sCasXML, oCasFile
    set oCasFile = oFS.OpenTextFile(args(0) + "cas.xml")
    
    sCasXml = oCasFile.ReadAll
    oManifest.Write Mid(sCasXml, 4)
    
    oManifest.WriteBlankLines 1
    oCasFile.Close
end if


oManifest.WriteLine "</Solution>"

oManifest.Close

sub EnumFolder(sFolder, sRelativePath)
    
    dim oFolder, oFolders, oSub, oFile
    set oFolder = oFS.GetFolder(sFolder)
    
    for each oFile in oFolder.Files
        
        dim sTestFolder, iPos
        
        iPos = InStrRev(sFolder, "TEMPLATE\FEATURES")
        if (iPos > 0) then
            sTestFolder = Mid(sFolder, iPos + 18)
        else
            sTestFolder = sFolder
        end if

        if (InStr(1, sTestFolder, "\") > 0) then
            if (InStr(1, oFile.Name, ".cs") = 0) then
                arTemplateFiles(iTemplateFileCount) = "        <TemplateFile Location='" + sRelativePath + "\" + oFile.Name + "' />"
                iTemplateFileCount = iTemplateFileCount + 1                
            end if
        end if
        
        if (lcase(oFile.Name) = "feature.xml") then
            arFeatures(iFeatureCount) = "        <FeatureManifest Location='" + sRelativePath + "\" + oFile.Name + "' />"
            iFeatureCount = iFeatureCount + 1
        end if
        
        if (InStr(1, oFile.Name, ".dwp") > 0) then
            arDWPFiles(iDWPCount) = "        <DwpFile Location='" + sRelativePath + "\" + oFile.Name + "' />"
            iDWPCount = iDWPCount + 1
        end if
        
    next

    if (sRelativePath = "TEMPLATE") then sRelativePath = ""
    if (sRelativePath = "FEATURES") then sRelativePath = ""

    if (sRelativePath <> "") then sRelativePath = sRelativePath + "\"

    for each oSub in oFolder.SubFolders
        EnumFolder oSub.Path, sRelativePath + oSub.Name
    next
    
end sub