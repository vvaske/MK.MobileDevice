# MK.MobileDevice
MK.MobileDevice - A powerful, versatile library providing multiple interfaces to access and manipulate iOS devices over USB and WiFi (using both Apple drivers and custom drivers for Linux recompiled for Windows) written in C#. (Still being actively developed)

Driver DLLs for both 32-bit and 64-bit Windows are included, and you can build your very own set of native DLLs from the [libimobiledevice-win64](https://github.com/exaphaser/libimobiledevice-win64) project.

## Features
Use a simple API to do all kinds of cool things with your iDevice, over a USB cable or wirelessly (with iTunes WiFi Sync).

Here is a short list containing SOME (there are a LOT more features) of the many features of this library
- Full AFC access in the `/private/var/mobile/Media` directory
- Full developer disk image support and mounting
- Take screenshots remotely
- Remotely reboot, shut down, enter/exit recovery mode
- Deactivate device
- SSH Access (with Jailbreak)
- Manage Applications (Install, Uninstall, Archive, Restore, Remove Archives, etc...)
- Manage home screen layout
- Rename device name
- Extract metadata such as phone number, UDID, IMEI, etc.
- And more!

Requires certain components protected by Apple's EULA, so please install iTunes, or at least Apple Mobile Device Support and Apple Application Support.

Avaulable on NuGet.

##Documentation
Documentation is available on [the github page](http://exaphaser.github.io/MK.MobileDevice
