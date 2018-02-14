if  $(PlatformName) == x86 (
  copy $(SolutionDir)\Win32\SodiumC.dll
) else if  $(PlatformName) == x64 (
  copy $(SolutionDir)\x64\SodiumC.dll
} else if $(PlatformName) == ARM (
  copy $(SolutionDir)\ARM\SodiumC.dll
) else (
  copy $(SolutionDir)\Win32\SodiumC.dll
)