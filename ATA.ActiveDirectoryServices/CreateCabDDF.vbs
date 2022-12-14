Option Explicit
Dim oFS, oDDF, args, oFestures, iFeatureCount

dim arFeatures(20)
iFeatureCount = 0

set args = WScript.Arguments

' Create the File System Object
Set oFS = CreateObject("Scripting.FileSystemObject")

Set oDDF = oFS.CreateTextFile(args(0) + "cab.ddf", true)
oDDF.WriteLine ";"
oDDF.WriteLine ".Set CabinetNameTemplate=" + args(1) + ".wsp"
oDDF.WriteLine ".set DiskDirectoryTemplate=CDROM ; All cabinets go in a single directory"
oDDF.WriteLine ".Set CompressionType=MSZIP;** All files are compressed in cabinet files"
oDDF.WriteLine ".Set UniqueFiles='ON'"
oDDF.WriteLine ".Set Cabinet=on"
oDDF.WriteLine ".Set DiskDirectory1=."


' Add the assemblies
EnumFolder args(0) + "TEMPLATE", "TEMPLATE"
EnumFolder args(0) + "CONFIG", "CONFIG"
EnumFolder args(0) + "DLLS", ""

' oDDF.WriteLine args(0) + "manifest.xml" + vbTab + "manifest.xml"
oDDF.WriteLine """" + args(0) +"manifest.xml"+ """" + vbTab + """" + "manifest.xml"""

oDDF.WriteLine ";*** <the end>"

oDDF.Close

sub EnumFolder(sFolder, sRelativePath)
    
    dim oFolder, oFolders, oSub, oFile
    set oFolder = oFS.GetFolder(sFolder)
    
    if (sRelativePath = "TEMPLATE") then sRelativePath = ""
    if (sRelativePath = "FEATURES") then sRelativePath = ""
    if (sRelativePath <> "") then sRelativePath = sRelativePath + "\"

    for each oFile in oFolder.Files
        oDDF.WriteLine """" + oFile.Path + """" + vbTab + """" + sRelativePath + oFile.Name + """"
    next

    if (sRelativePath <> "" and InStr(1, sFolder, "FEATURES") > 0) then
        sRelativePath = "FEATURES\" + sRelativePath
    end if

    for each oSub in oFolder.SubFolders
        EnumFolder oSub.Path, sRelativePath + oSub.Name
    next
    
end sub