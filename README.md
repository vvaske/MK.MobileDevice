
**It's free but there's no support. Use it as you like (within the GPLv3), but don't email me asking for support. Instead, look through the sample
code and the LibIMobileDevice source code, and try to diagnose any problems yourself. For samples, there is a sample project
included.**

## **If you want to waive the GPLv3's restrictions and use `MK.MobileDevice` in your commercial application, please consider [purchasing a license](https://exaphaser.binpress.com/product/mkmobiledevice/3765) to support this project and my future open source work. Thanks!

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

##Documentation
Documentation is available on [the github page](http://0xFireball.github.io/MK.MobileDevice/docs)

