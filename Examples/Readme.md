# NOTICE:
When building examples, you will need to copy the files from the DriverDLLs/ folder to your project's output directory regardless of whether you are 32-bit or 64-bit, as the project is being compiled for x86 and not x64.
In your own projects, you can use either; however, your C# project must be being built for the x64 platform for the 64-bit drivers to work.

# MK Battery Wizard
- A simple program that shows detailed battery information about a connected device.