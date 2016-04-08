# Project Details

Stardew Farmhand is the new in-development API intended to replace SMAPI

##License

SMAPI Compatibility Layer: LGPL

Farmhand: MIT

xml2json: Not Specified (http://www.bjelic.net/2012/08/01/coding/convert-xml-to-json-using-xslt/)


##My First Build

Building the project for the first time can seem a little daunting given the number of projects and no doubt countless missing references you'll see but be unable to resolve. Fear not, it only takes about as many steps as SMAPI to be up and running! :)

1) Repair Stardew Valley and xTile references. These will be located in: 
- "Libraries/Farmhand" (Stardew Valley and xTile) 
- "Mods/ModLoaderMod" (xTile only)
- "Libraries/SmapiCompatibilityLayer" (xTile only) 
- (**do not touch the Stardew Farmhand references or try to add new references to fix errors**) 

2) Build the project "BuildTasks/BuildFarmhandFinal" in **Debug** mode, this should have fixed all reference errors and built the API in <RepositoryPath>/WorkingDirectory. Intermediate build binaries will be in <RepositoryPath>/Bin. See below for potential issues: 
- If you get Newtonsoft.Json errors: It's been known to incorrectly download the 4.5 version when we need to 4.0 version. To fix this, right click the Farmhand project, Manage NuGet Packages, uninstall Newtonsoft.Json and readd it.
- If you get warnings like " There was a mismatch between the processor architecture of the project being built "MSIL" and the processor architecture of the reference ", right click the project causing it, click properties, and in the Build tab set Platform Target to "x86".

3) To ease your workflow, copy the Stardew Valley Contents folder into <RepositoryPath>/WorkingDirectory, and the same with Stardew Valley's DLLs (i.e. xTile.dll and Steamworks.NET.dll). You are now ready to start working on the API! :)

4) For your reference, Stardew Farmhand.int1.dll is generated by BuildFarmhandIntermediate. This bundles together the Farmhand core code into Stardew Valley. This is then used by second pass build libraries such as Farmhand UI so it can take advantage of methods injected into Stardew during the first pass.


##Working on the core API

The core API is located inside Libraries/Farmhand. For changes to this library to be reflected, you'll need to manually trigger BuildFarmhandFinal. Alternatively you can add BuildFarmhandFinal as a build dependency to Tools/FarmhandDebugger. This is not done as default as it would lower iteration times for developers working on mods - who do not need to constantly rebuild the API.

Note, that if you are working on both Farmhand and a second-pass API library (Farmhand UI), you will need to manually build BuildFarmhandIntermediate to fix intellisence reference warnings. However, these warnings can just be ignored and will do away upon trigging a final build if the code is correct.

##Build Tasks

These projects are responsible for handling build flow. 

- BuildFarmhandIntermediate packages Farmhand Core into Stardew and applies the appropriate hooks. This is the project you should click "Rebuild" on when altering the engine, as it will force all builds to trigger in the correct order.

- BuildFarmhandFinal packages the projects dependent on the results of BuildFarmhandIntermediate into Stardew and produces the final Stardew Farmhand.exe

##Installers

- Farmhand Installer - Console (EXE). This runs FarmhandPatcher, expecting the Stardew Valley and Stardew Farmhand files to be adjacent to it. 

- Farmhand Installer - UI (EXE). This runs FarmhandPatcher too. Once it's complete, this will be the file we actually distribute out. It should automatically bundle the required binaries (Farmhand, FarmhandUI, FarmhandPatcherCommon, FarmhandPatcherFirstPass, FarmhandPatcherSecondPass) and extract them at runtime. At the final stages, this should allow for updated binaries to be fetched from the internet.

- FarmhandPatcherCommon/FirstPass/SecondPass. These are used by the installer.They are separated into their own libraries to prevent conflicts due to the missing (not yet built) Stardew Farmhand intermediate

##Mods

- Logging Mod (DLL). A mod, just used to log events when they fire

- Mod Loader Mod (DLL). A mod which will provide ingame loading and unloading of other mods.

- Test Content Mod (DLL). This mod demonstrates using manifests to make a content only mod. As I wanted it to automatically put it's files in the right place, it's a class library with a post-build action to delete the empty .DLL and .PDB 

- Jumino Deposit Mod (DLL). A test functional mod. This is mainly used to test UI event callbacks, and address crash issues.

##Libraries

- Farmhand (DLL). The core code which is injected into Stardew

- FarmhandUI (DLL). This is an extension of Farmhand which provides easy to use overrides for the inbuild UI classes.

##Tools

- Farmhand Debugger (EXE). Just serves as an entry point for me to step through and debug StardewR code in the event I inject something incorrectly. This should be set as your startup project when not debugging the build process.

