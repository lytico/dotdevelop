hints

in proj-files:
add
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'DebugGnome|AnyCPU' " />
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'ReleaseGnome|AnyCPU' " />

props see: monodevelop/main/msbuild

replace <ItemGroup Condition="$(OS) != 'Windows_NT'"> with:
  <ItemGroup Condition="$(HaveXamarinMac) == 'true'">

inside code:

#if MAC



-----------------
next step to get COMPILING (not running) on linux:




DebugWin32:
addins\MonoDevelop.TextEditor\MonoDevelop.TextEditor
