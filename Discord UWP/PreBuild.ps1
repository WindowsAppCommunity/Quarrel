param($ProjectDir, $ConfigurationName, $TargetDir, $TargetFileName, $PlatformName, $SolutionDir)

############################################################
#######   A simple pre-build script for Discord UWP   #######
#############################################################
echo "Running pre-build powershell script"

try{

# CD to the current directory (Solution directory), to simplify everything further down the line
cd $PSScriptRoot
# Hard-code the latest commit id, the latest commit date, and the "build number" (full commit count) into CommitInfo.txt
$CommitId = git log --format="%h" -n 1
$CommitDate = git log --format="%cI" -n 1
$CommitCount = git rev-list --all --count
Set-Content $PSScriptRoot/Assets/CommitInfo.txt "$CommitId`n$CommitDate`n$CommitCount"
echo "Wrote build details to CommitInfo.txt"

#echo "Platform name is $PlatformName"
#echo "Project dir is $ProjectDir"
#Copy sodiumC
#Copy-Item -force $PlatformName\SodiumC.dll $ProjectDir
#echo "Copied sodiumC.dll"
exit 0
}
catch{exit 1}



