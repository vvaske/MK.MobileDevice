# MK.MobileDevice
MK.MobileDevice - A powerful, versatile library providing multiple interfaces to access and manipulate iOS devices over USB and WiFi (using both Apple drivers and custom drivers for Linux recompiled for Windows) written in C#. (Still being actively developed)

- Yes, it's open-source, licensed under the `GPLv3`; please see `LICENSE` for more information.
- It's GPL because there are too many closed source programs (iMazing, iExplorer, Syncios, etc.) that have similar functionality. I wanted an open community library and some open source tools.

# [Get Started](#install)

Driver DLLs for both 32-bit and 64-bit Windows are included, and you can build your very own set of native DLLs from the [libimobiledevice-win64](https://github.com/exaphaser/libimobiledevice-win64) project.

## Support
**Yes, I am willing to provide support, **but only as time permits**. Please post your question/issue in [Issues](https://github.com/exaphaser/MK.MobileDevice/issues)**

Thanks!

# Getting set up [See Install](#install)

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

#Install
- An **outdated** version of MK.MobileDevice is available on [NuGet](https://www.nuget.org/packages/MK.MobileDevice/)!
- I recommend either **cloning the repository** ([how](https://help.github.com/articles/cloning-a-repository/)) or **downloading the artifacts from [AppVeyor](https://ci.appveyor.com/project/0xFireball/mk-mobiledevice).**

## Once you have obtained a copy, proceed to the [Wiki](https://github.com/exaphaser/MK.MobileDevice/wiki) for more information.
