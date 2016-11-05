**Update** - End of support

Due to my increasing frustration with Apple, this project will no longer be actively developed or maintained. I now use an Android device, and will never go back to iOS. Everything that currently works should keep on working fine, and iTMD APIs should be compatible with all iOS in the foreseeable future, as they use iTunes libraries. Unfortunately, Apple doesn't care about is users, and they probably wouldn't think twice before making breaking changes to the API...

Don't lose hope yet! The version of LibiMD I had been using was https://github.com/exaphaser/libimobiledevice-win64, and you can try to merge the changes from the official repo and compile, then continue using MK.MobileDevice, as it binds to LibiMD's API.






<img src="https://raw.githubusercontent.com/0xFireball/MK.MobileDevice/master/icon.png" width="120" height="120" />
### The best iOS device communication library! Absolutely free and open source! Program your iOS device to do your bidding! Create awesome automation programs that detect your iPhone wirelessly!


# MK.MobileDevice
MK.MobileDevice - A powerful, versatile library providing multiple interfaces to access and manipulate iOS devices over USB and WiFi (using both Apple drivers and custom drivers for Linux recompiled for Windows) written in C#. (Still being actively developed)

- Yes, it's open-source, licensed under the `GPLv3`; please see `LICENSE` for more information.
- It's GPL because there are too many closed source programs (iMazing, iExplorer, Syncios, etc.) that have similar functionality. I wanted an open community library and some open source tools.

## Features
Use a **Simple, concise API** to do all kinds of cool things with your iDevice, over a USB cable or **wirelessly** (with iTunes WiFi Sync). You can use it as a sort of remote-control iTunes or something. Make your device do your bidding with **MK.MobileDevice**!

Here is a short list containing SOME (there are a LOT more features) of the many features of this library
- **Full AFC access in the `/private/var/mobile/Media` directory** - transfer files back and forth!
- Full developer disk image support and mounting (EVEN ON WINDOWS/Linux!!!!)
- Take screenshots remotely! (you must mount developer disk first)
- Connect to device - USB and even **Wirelessly**!!!
- **Remotely reboot, shut down, enter/exit recovery mode!**
- Manage iCloud activation lock!
- Activate a [hidden Semi-Sleep/Lock state](https://0xfireball.github.io/MK.MobileDevice/html/9b17a6a7-9c58-c770-230c-5981aeca32f6.htm) (not yet tested on iOS 10, but it may work!)
- Manage Applications (Install, Uninstall, Archive, Restore, Remove Archives, etc...)
- Manage home screen layout (extract layouts, send layouts)!
- Rename device name!
- Full GasGauge battery diagnostics!
- Extract metadata such as phone number, UDID, IMEI, etc.
- Query apple property services for information like iCloud activation, etc.
- And lots more! If there's anything important I didn't mention here, let me know!

- Some of the APIs require certain components protected by Apple's EULA, so please install iTunes, or at least Apple Mobile Device Support and Apple Application Support. I do not believe I am allowed to redistribute them.

# Planned features!
- SSH Access (with Jailbreak)
- Device port forwarding!

# [Get Started](#install)

Driver DLLs for both 32-bit and 64-bit Windows are included, and you can build your very own set of native DLLs from the [libimobiledevice-win64](https://github.com/exaphaser/libimobiledevice-win64) project.

# Getting set up [See Install](#install)

##Documentation
Documentation is available on [the github page](http://0xFireball.github.io/MK.MobileDevice/docs)

#Install
- An **outdated** version of MK.MobileDevice is available on [NuGet](https://www.nuget.org/packages/MK.MobileDevice/) :( Please don't use this.
- I recommend either **cloning the repository** ([how](https://help.github.com/articles/cloning-a-repository/)) or **downloading the artifacts from [AppVeyor](https://ci.appveyor.com/project/0xFireball/mk-mobiledevice).**

## Once you have obtained a copy, proceed to the [Wiki](https://github.com/exaphaser/MK.MobileDevice/wiki) for more information.
