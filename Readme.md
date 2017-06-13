# SEScriptbuilder

[Download latest SESCriptBuilder.zip](https://github.com/Haarmees/SEVSTools/raw/master/SEScriptBuilder.zip)

[Check out this guide on how to use the tool in VS](https://github.com/Haarmees/SEVSTools/blob/master/SEScriptBuilder/docs/SetupVS.md)
## What it does
SEScript builder is a tool to inject code in a main program file. It works with a 
.csproj file which is also used by Visual Studio. It will check the project for a 
file with a class that extends MyGridProgram. If found it will analyze this file 
and inject any code used by the 'main' file. For example you can have a class file 
in your project or a referenced project:
```
// BatteryGroup.cs
using System; 
...
namespace MyProject {
    public Class BatteryGroup {

        List<IMyBatteryBlock> batteries;

        public BatteryGroup(List<IMyBatteryBlock> b)
        {
            this.batteries = b;
        }
        
        // other code

    }
}
```
Now assume you have a 'program'file as follows:
```
// MyShipProgram.cs
using System; 
...
namespace MyProject {
    public Class MyShipProgram : MyGridProgram {

        BatteryGroup batGroup;

        public MyShipProgram()
        {
            // constructor code (will become program function)
        }

        public void Main(){
        {
            this.batGroup = new BatteryGroup(aListWithBatteryBlocks)
        }
        
        // other code

    }
}
```
Running the tool in degub mode will creat a file called Script.cs with the following code:
```
// Script.cs
#if DEBUG
using System; 
...
namespace MyProject {
    public Class Program : MyGridProgram {
#endif
        BatteryGroup batGroup;

        public Program()
        {
            // constructor code (will become program function)
        }

        public void Main(){
        {
            this.batGroup = new BatteryGroup(aListWithBatteryBlocks)
        }
        
        // other code

        public Class BatteryGroup {

            List<IMyBatteryBlock> batteries;

            public BatteryGroup(List<IMyBatteryBlock> b)
            {
                this.batteries = b;
            }
        
            // other code

        }
#if DEBUG
    }
}
#endif
```
Note that the BatteryGroup class is injected in the program class. The class name and constructor are rewritten to 'Program'. The #if DEBUG and #endif allows you to simply copy all code to your program block. The program block will ignore these regions.

## The tool
The tool can be run from the command line. It has the following options:

Option | Required | Description
------ | -------- | -----------
-p, --project | Yes | The path to the .csproj file
-o, --out | Yes | The output path
-prod, --production | No | Add this flag to run in production mode

If you've added the .exe file to your [path in environment variables](https://www.howtogeek.com/118594/how-to-edit-your-system-path-for-easy-command-line-access/) 
you can run the build command on the script builder. For example:
```
SEScriptBuilder Build --project="C:\..\MyProject\MyProject.csproj" --out="C:\..\MyProject" --prod
```
Note that you need to call the 'Build' command and the quotation marks around the paths (which are shortened for readability). 

## Known limitations
  * Class names should be unique: classes are directly imported and conflicts occur if classes have the same name.
  
## Changelog
### 0.1.1
  * Fixed virtual and override methods not imported correctly
