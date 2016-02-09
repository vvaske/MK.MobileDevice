using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using PlistCS;

namespace MK.MobileDevice.XEDevice
{
    // Token: 0x02000009 RID: 9
    public unsafe class iPhoneXE
    {
        // Token: 0x0600002A RID: 42 RVA: 0x000029D4 File Offset: 0x00000BD4
        public iPhoneXE(iPhoneXEDelegate delObj)
        {
            this._delegateObject = delObj;
        }

        // Token: 0x0600002B RID: 43 RVA: 0x00002A64 File Offset: 0x00000C64
        public iPhoneXE(ConnectEventArgs deviceArgs, bool allowAFC2, iPhoneXEDelegate delObj)
        {
            this._delegateObject = delObj;
            this.iPhoneHandle = deviceArgs.Device;
            string newService = "com.apple.afc";
            if (allowAFC2)
            {
                newService = "com.apple.afc2";
            }
            this.nolock_ConnectToService(newService, false);
        }

        // Token: 0x06000063 RID: 99 RVA: 0x00004A40 File Offset: 0x00002C40
        public unsafe Dictionary<string, XEDeviceApplicationInfo> AppBundles()
        {
            Dictionary<string, XEDeviceApplicationInfo> result;
            try
            {
                lock (this)
                {
                    if (this.appAndBundleDict != null && this.appAndBundleDict.Count > 0)
                    {
                        result = this.appAndBundleDict;
                    }
                    else
                    {
                        MobileDeviceBase.__CFDictionary* root_dict = null;
                        if (MobileDeviceBase.AMDeviceLookupApplications(this.iPhoneHandle, null, ref root_dict) != 0)
                        {
                            Console.WriteLine("Error loading app bundles: attempt 1");
                            this.currentBundleID = null;
                            this.nolock_ConnectToService("com.apple.afc", false);
                            if (MobileDeviceBase.AMDeviceLookupApplications(this.iPhoneHandle, null, ref root_dict) != 0)
                            {
                                throw new Exception("Couldn't load app bundles.");
                            }
                        }
                        Dictionary<string, object> dictionary = CFDictParser.ParseAppDictionaryData((void*)root_dict);
                        Dictionary<string, XEDeviceApplicationInfo> dictionary2 = new Dictionary<string, XEDeviceApplicationInfo>();
                        foreach (KeyValuePair<string, object> current in dictionary)
                        {
                            Dictionary<string, object> infoDict = (Dictionary<string, object>)current.Value;
                            XEDeviceApplicationInfo XEDeviceApplicationInfo = new XEDeviceApplicationInfo(current.Key, infoDict);
                            if (XEDeviceApplicationInfo.IsAppTypeUser)
                            {
                                dictionary2.Add(current.Key, XEDeviceApplicationInfo);
                            }
                        }
                        this.appAndBundleDict = dictionary2;
                        result = dictionary2;
                    }
                }
            }
            catch (Exception ex)
            {
                LogViewer.LogEvent(0, ex.ToString());
                result = new Dictionary<string, XEDeviceApplicationInfo>();
            }
            return result;
        }

        // Token: 0x06000065 RID: 101 RVA: 0x00004CB4 File Offset: 0x00002EB4
        public string AppImageCachedPath(string service)
        {
            if (!this._appImageDictionary.ContainsKey(service))
            {
                string text = Path.ChangeExtension(Path.GetTempFileName(), ".png");
                if (!this.IOS8OrLater() && this.CopyFromDeviceToComputer("/iTunesArtwork", text, service))
                {
                    this._appImageDictionary.Add(service, text);
                }
                if (!this._appImageDictionary.ContainsKey(service))
                {
                    byte[] array = this.SpringboardIconPNGData(service);
                    if (array != null)
                    {
                        File.WriteAllBytes(text, array);
                        this._appImageDictionary.Add(service, text);
                    }
                    else
                    {
                        this._appImageDictionary.Add(service, null);
                    }
                }
            }
            return this._appImageDictionary[service];
        }

        // Token: 0x0600006D RID: 109 RVA: 0x0000208A File Offset: 0x0000028A
        public void CancelTransfer()
        {
            this._transferCancelled = true;
        }

        // Token: 0x06000061 RID: 97 RVA: 0x00004954 File Offset: 0x00002B54
        public void CloseFile(string path, string service)
        {
            lock (this)
            {
                if (path == this._AFCFilePath && service == this._AFCFileService)
                {
                    this.nolock_AFCFileClose();
                }
            }
        }

        // Token: 0x06000067 RID: 103 RVA: 0x00004F20 File Offset: 0x00003120
        private static string ColorStringBlackOrWhite(ProductInfo.ColorType color)
        {
            string result;
            switch (color)
            {
                case ProductInfo.ColorType.Black:
                case ProductInfo.ColorType.SpaceGray:
                    {
                        IL_1E:
                        result = "Black";
                        return result;
                    }
                case ProductInfo.ColorType.White:
                case ProductInfo.ColorType.Silver:
                case ProductInfo.ColorType.Gold:
                    {
                        result = "White";
                        return result;
                    }
            }
            //goto IL_1E;
            result = "Black";
            return result;
        }

        // Token: 0x06000069 RID: 105 RVA: 0x00004FA0 File Offset: 0x000031A0
        private static string ColorStringColorful(ProductInfo.ColorType color)
        {
            switch (color)
            {
                case ProductInfo.ColorType.White:
                case ProductInfo.ColorType.SpaceGray:
                case ProductInfo.ColorType.Silver:
                case ProductInfo.ColorType.Gold:
                    {
                        IL_2A:
                        string result = "White";
                        return result;
                    }
                case ProductInfo.ColorType.Pink:
                    {
                        string result = "Pink";
                        return result;
                    }
                case ProductInfo.ColorType.Yellow:
                    {
                        string result = "Yellow";
                        return result;
                    }
                case ProductInfo.ColorType.Blue:
                    {
                        string result = "Blue";
                        return result;
                    }
                case ProductInfo.ColorType.Green:
                    {
                        string result = "Green";
                        return result;
                    }
                default:
                    {
                        string result = "White";
                        return result;
                    }
            }
            //goto IL_2A;
        }

        // Token: 0x06000068 RID: 104 RVA: 0x00004F5C File Offset: 0x0000315C
        private static string ColorStringGoldSilverOrGray(ProductInfo.ColorType color)
        {
            switch (color)
            {
                case ProductInfo.ColorType.Black:
                case ProductInfo.ColorType.SpaceGray:
                    {
                        IL_1E:
                        string result = "Gray";
                        return result;
                    }
                case ProductInfo.ColorType.White:
                case ProductInfo.ColorType.Silver:
                    {
                        string result = "Silver";
                        return result;
                    }
                case ProductInfo.ColorType.Gold:
                    {
                        string result = "Gold";
                        return result;
                    }
                default:
                    {
                        string result = "Gray";
                        return result;
                    }
            }
            //goto IL_1E;
        }

        // Token: 0x0600002C RID: 44 RVA: 0x00002B18 File Offset: 0x00000D18
        public bool ConnectToPhone(ConnectEventArgs deviceArgs, bool allowAFC2)
        {
            bool result;
            if (deviceArgs.Message == NotificationMessage.Connected)
            {
                this.iPhoneHandle = deviceArgs.Device;
                string newService = "com.apple.afc";
                if (allowAFC2)
                {
                    newService = "com.apple.afc2";
                }
                bool flag = false;
                lock (this)
                {
                    flag = this.nolock_ConnectToService(newService, false);
                }
                result = flag;
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x0600003C RID: 60 RVA: 0x0000325C File Offset: 0x0000145C
        public byte[] copyBytesFromDevice(string sourcePathOnDevice, long thisOffset, uint thisLength, string service)
        {
            byte[] result;
            if (sourcePathOnDevice.Length == 0)
            {
                result = null;
            }
            else
            {
                if (sourcePathOnDevice[0] != '/')
                {
                    sourcePathOnDevice = "/" + sourcePathOnDevice;
                }
                byte[] array = new byte[thisLength];
                uint num = thisLength;
                lock (this)
                {
                    long handle;
                    if (this.nolock_AFCFileOpen(sourcePathOnDevice, service, OpenMode.Read, out handle) && MobileDeviceBase.AFCFileRefSeek(this.AFCHandle, handle, thisOffset, 0L) == 0 && MobileDeviceBase.AFCFileRefRead(this.AFCHandle, handle, array, ref num) == 0)
                    {
                        if (num == thisLength)
                        {
                            result = array;
                            return result;
                        }
                        byte[] array2 = new byte[num];
                        Array.Copy(array, array2, (long)((ulong)num));
                        result = array2;
                        return result;
                    }
                }
                result = null;
            }
            return result;
        }

        // Token: 0x0600002F RID: 47 RVA: 0x00002C60 File Offset: 0x00000E60
        public string CopyDeviceValue(string keyStr)
        {
            string text = null;
            if (!this._copyValueDictionary.ContainsKey(keyStr))
            {
                lock (this)
                {
                    text = MobileDeviceBase.AMDeviceCopyValue(this.iPhoneHandle, keyStr);
                }
                if (text != null)
                {
                    this._copyValueDictionary.Add(keyStr, text);
                }
            }
            else
            {
                text = this._copyValueDictionary[keyStr];
            }
            return text;
        }

        // Token: 0x0600003A RID: 58 RVA: 0x00002EE8 File Offset: 0x000010E8
        private bool copyFileFromComputerToDevice(string sourcePathOnComputer, string targetPathOnDevice, string service)
        {
            bool flag = false;
            bool result;
            try
            {
                byte[] array = new byte[655360];
                FileInfo fileInfo = new FileInfo(sourcePathOnComputer);
                long length = fileInfo.Length;
                long num = 0L;
                if (this.Exists(targetPathOnDevice, service))
                {
                    if (iPhoneXE.AskBeforeOverwritingSameFile)
                    {
                        //DialogResult dialogResult = MessageBox.Show("A file with the specified file name already exists in the target directory.  Would you like to overwrite it: \r\n\r\n" + targetPathOnDevice, "Overwrite Existing?", MessageBoxButtons.OKCancel);
                        if (true)
                        {
                            result = false;
                            return result;
                        }
                        this.DeleteFile(targetPathOnDevice, service);
                    }
                    else
                    {
                        this.DeleteFile(targetPathOnDevice, service);
                    }
                }
                lock (this)
                {
                    FileStream fileStream;
                    try
                    {
                        fileStream = File.OpenRead(sourcePathOnComputer);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("Unable to read file " + sourcePathOnComputer + ".  Error code = " + ex.ToString());
                        throw ex;
                        result = false;
                        return result;
                    }
                    long handle;
                    if (this.nolock_AFCFileOpen(targetPathOnDevice, service, OpenMode.Write, out handle))
                    {
                        int num2 = 0;
                        while (num <= length)
                        {
                            if (!this._transferCancelled)
                            {
                                int num3 = fileStream.Read(array, 0, array.Length);
                                if (num3 > 0 && MobileDeviceBase.AFCFileRefWrite(this.AFCHandle, handle, array, (uint)num3) == 0)
                                {
                                    num += (long)num3;
                                    num2++;
                                    if (num2 < 5 && num < length)
                                    {
                                        continue;
                                    }
                                    num2 = 0;
                                    if (this._manager != null)
                                    {
                                        iPhoneXEManager iPhoneXEManager = (iPhoneXEManager)this._manager.Target;
                                        iPhoneXEManager.NotifyUpdate(false, this.deviceID, sourcePathOnComputer, targetPathOnDevice, service, (double)num / (double)length);
                                        continue;
                                    }
                                    continue;
                                }
                            }
                            else
                            {
                                flag = false;
                            }
                            IL_174:
                            this.nolock_AFCFileClose();
                            flag = (this.FileSize(targetPathOnDevice, service) == (ulong)length);
                            if (this._manager != null && flag)
                            {
                                iPhoneXEManager iPhoneXEManager = (iPhoneXEManager)this._manager.Target;
                                iPhoneXEManager.NotifyUpdate(false, this.deviceID, sourcePathOnComputer, targetPathOnDevice, service, 1.0);
                                goto IL_1C4;
                            }
                            goto IL_1C4;
                        }
                        //goto IL_174;
                    }
                    IL_1C4:
                    fileStream.Close();
                }
                if (this._transferCancelled)
                {
                    if (this.Exists(targetPathOnDevice, service))
                    {
                        this.DeleteFile(targetPathOnDevice, service);
                    }
                    flag = false;
                }
            }
            catch (Exception ex2)
            {
                flag = false;
                LogViewer.LogEvent(2, ex2.ToString());
            }
            result = flag;
            return result;
        }

        // Token: 0x0600003F RID: 63 RVA: 0x00003518 File Offset: 0x00001718
        private bool copyFileFromDeviceToComputer(string sourcePathOnDevice, string targetPathOnComputer, string service)
        {
            bool result = false;
            try
            {
                byte[] buffer = new byte[655360];
                uint num = 655360u;
                ulong num2 = this.FileSize(sourcePathOnDevice, service);
                ulong num3 = 0uL;
                if (File.Exists(targetPathOnComputer))
                {
                    File.Delete(targetPathOnComputer);
                }
                if (!Directory.Exists(Path.GetDirectoryName(targetPathOnComputer)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPathOnComputer));
                }
                lock (this)
                {
                    if (this.nolock_ConnectToService(service, false))
                    {
                        FileStream fileStream = File.OpenWrite(targetPathOnComputer);
                        long handle;
                        if (this.nolock_AFCFileOpen(sourcePathOnDevice, service, OpenMode.Read, out handle))
                        {
                            while (num3 <= num2)
                            {
                                this._delegateObject.UpdateFileTransferProgress(num3 / num2);
                                if (this._transferCancelled || MobileDeviceBase.AFCFileRefRead(this.AFCHandle, handle, buffer, ref num) != 0 || num <= 0u)
                                {
                                    break;
                                }
                                fileStream.Write(buffer, 0, (int)num);
                                num3 += (ulong)num;
                                num = 655360u;
                            }
                            this.nolock_AFCFileClose();
                        }
                        fileStream.Close();
                    }
                }
                if (File.Exists(targetPathOnComputer))
                {
                    if (this._transferCancelled)
                    {
                        File.Delete(targetPathOnComputer);
                        result = false;
                    }
                    else
                    {
                        FileInfo fileInfo = new FileInfo(targetPathOnComputer);
                        result = (fileInfo.Length == (long)num2);
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                LogViewer.LogEvent(2, ex.ToString());
                result = false;
            }
            return result;
        }

        // Token: 0x06000040 RID: 64 RVA: 0x000036A8 File Offset: 0x000018A8
        private bool copyFileFromDeviceToComputer(string sourcePathOnDevice, Stream targetFS, string service)
        {
            bool result = false;
            try
            {
                byte[] buffer = new byte[655360];
                uint num = 655360u;
                ulong num2 = this.FileSize(sourcePathOnDevice, service);
                ulong num3 = 0uL;
                lock (this)
                {
                    if (this.nolock_ConnectToService(service, false))
                    {
                        this._delegateObject.UpdateFileTransferProgress(0.0);
                        long handle;
                        if (this.nolock_AFCFileOpen(sourcePathOnDevice, service, OpenMode.Read, out handle))
                        {
                            while (num3 <= num2)
                            {
                                if (!this._transferCancelled)
                                {
                                    if (MobileDeviceBase.AFCFileRefRead(this.AFCHandle, handle, buffer, ref num) == 0 && num > 0u)
                                    {
                                        targetFS.Write(buffer, 0, (int)num);
                                        num3 += (ulong)num;
                                        num = 655360u;
                                        this._delegateObject.UpdateFileTransferProgress(num3 / num2);
                                        continue;
                                    }
                                }
                                else
                                {
                                    result = false;
                                }
                                IL_C8:
                                this.nolock_AFCFileClose();
                                this._delegateObject.UpdateFileTransferProgress(0.999);
                                goto IL_E2;
                            }
                            //goto IL_C8;
                        }
                        IL_E2:
                        targetFS.Close();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogViewer.LogEvent(2, ex.ToString());
                result = false;
            }
            return result;
        }

        // Token: 0x0600003B RID: 59 RVA: 0x0000315C File Offset: 0x0000135C
        public bool CopyFromComputerToDevice(string sourcePathOnComputer, string targetPathOnDevice, string service)
        {
            LogViewer.LogEvent(2, "copyFromComputerToDevice:" + sourcePathOnComputer + " targetPathOnDevice:" + targetPathOnDevice);
            this._transferCancelled = false;
            bool result = false;
            if (File.Exists(sourcePathOnComputer) || Directory.Exists(sourcePathOnComputer))
            {
                result = true;
                if (Directory.Exists(sourcePathOnComputer))
                {
                    this.CreateDirectory(targetPathOnDevice, service);
                    List<string> list = new List<string>();
                    list.AddRange(Directory.GetDirectories(sourcePathOnComputer));
                    list.AddRange(Directory.GetFiles(sourcePathOnComputer));
                    using (List<string>.Enumerator enumerator = list.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            if (this._transferCancelled)
                            {
                                result = false;
                                IL_BB:
                                goto IL_D5;
                            }
                            string fileName = Path.GetFileName(current);
                            if (!this.CopyFromComputerToDevice(Path.Combine(sourcePathOnComputer, fileName), XEPathInfo.UnixPathCombine(targetPathOnDevice, fileName), service))
                            {
                                result = false;
                            }
                        }
                        //goto IL_BB;
                    }
                }
                result = this.copyFileFromComputerToDevice(sourcePathOnComputer, targetPathOnDevice, service);
            }
            IL_D5:
            if (this._transferCancelled)
            {
                result = false;
            }
            return result;
        }

        // Token: 0x06000042 RID: 66 RVA: 0x000038F0 File Offset: 0x00001AF0
        public bool CopyFromDeviceToComputer(string sourcePathOnDevice, string targetPathOnComputer, string serviceName)
        {
            bool result;
            if (sourcePathOnDevice.Length == 0)
            {
                result = false;
            }
            else
            {
                if (sourcePathOnDevice[0] != '/')
                {
                    sourcePathOnDevice = "/" + sourcePathOnDevice;
                }
                bool flag = false;
                this._transferCancelled = false;
                if (this.Exists(sourcePathOnDevice, serviceName))
                {
                    flag = true;
                    if (this.IsDirectory(sourcePathOnDevice, serviceName))
                    {
                        Directory.CreateDirectory(targetPathOnComputer);
                        Array contents = this.GetContents(sourcePathOnDevice, serviceName, true);
                        IEnumerator enumerator = contents.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                string text = (string)enumerator.Current;
                                if (this._transferCancelled)
                                {
                                    flag = false;
                                    IL_DD:
                                    goto IL_101;
                                }
                                if (!(text == ".") && !(text == ".."))
                                {
                                    string path = this.internationalFileName(text);
                                    if (!this.CopyFromDeviceToComputer(XEPathInfo.UnixPathCombine(sourcePathOnDevice, text), Path.Combine(targetPathOnComputer, path), serviceName))
                                    {
                                        flag = false;
                                    }
                                }
                            }
                            //goto IL_DD;
                        }
                        finally
                        {
                            IDisposable disposable = enumerator as IDisposable;
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                        }
                    }
                    flag = this.copyFileFromDeviceToComputer(sourcePathOnDevice, targetPathOnComputer, serviceName);
                }
                IL_101:
                if (this._transferCancelled)
                {
                    flag = false;
                }
                result = flag;
            }
            return result;
        }

        // Token: 0x06000043 RID: 67 RVA: 0x00003A20 File Offset: 0x00001C20
        public bool CopyFromDeviceToComputer(string sourcePathOnDevice, Stream outputStream, string serviceName)
        {
            bool result;
            if (sourcePathOnDevice.Length == 0)
            {
                result = false;
            }
            else
            {
                if (sourcePathOnDevice[0] != '/')
                {
                    sourcePathOnDevice = "/" + sourcePathOnDevice;
                }
                bool flag = false;
                this._transferCancelled = false;
                if (this.Exists(sourcePathOnDevice, serviceName))
                {
                    flag = (!this.IsDirectory(sourcePathOnDevice, serviceName) && this.copyFileFromDeviceToComputer(sourcePathOnDevice, outputStream, serviceName));
                }
                if (this._transferCancelled)
                {
                    flag = false;
                }
                result = flag;
            }
            return result;
        }

        // Token: 0x06000050 RID: 80 RVA: 0x00004174 File Offset: 0x00002374
        public bool CreateDirectory(string path, string service)
        {
            LogViewer.LogEvent(1, "Creating directory: " + path);
            bool result = false;
            lock (this)
            {
                if (this.nolock_ConnectToService(service, false))
                {
                    this.nolock_AFCFileClose();
                    if (MobileDeviceBase.AFCDirectoryCreate(this.hAFC, Encoding.UTF8.GetBytes(path)) == 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00004154 File Offset: 0x00002354
        public ulong DateAsInt(string path, string service)
        {
            ulong num;
            bool flag;
            ulong result;
            this.GetFileInfo(path, out num, out flag, out result, service);
            return result;
        }

        // Token: 0x06000055 RID: 85 RVA: 0x00004340 File Offset: 0x00002540
        public bool DeleteDirectory(string path, string service)
        {
            path = path.Trim(new char[]
            {
                '/'
            });
            path = "/" + path + "/";
            bool result = false;
            if (this.IsDirectory(path, service))
            {
                lock (this)
                {
                    if (this.nolock_ConnectToService(service, false))
                    {
                        this.nolock_AFCFileClose();
                        result = (MobileDeviceBase.AFCRemovePath(this.hAFC, Encoding.UTF8.GetBytes(path)) == 0);
                    }
                }
            }
            return result;
        }

        // Token: 0x06000056 RID: 86 RVA: 0x000043E0 File Offset: 0x000025E0
        public bool DeleteDirectory(string path, bool recursive, string service)
        {
            bool result;
            if (true)
            {
                //DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete the specified directory and all its contents? \r\n\r\n" + path, "Confirm Delete", MessageBoxButtons.OKCancel);
                if (true)
                {
                    result = false;
                    return result;
                }
            }
            if (!recursive)
            {
                result = this.DeleteDirectory(path, service);
            }
            else
            {
                result = (this.IsDirectory(path, service) && this.publicDeleteDirectory(path, service));
            }
            return result;
        }

        // Token: 0x06000057 RID: 87 RVA: 0x00004440 File Offset: 0x00002640
        public bool DeleteFile(string path, string service)
        {
            bool result;
            
            bool flag = false;
            if (this.Exists(path, service))
            {
                if (this.nolock_ConnectToService(service, false))
                {
                    this.nolock_AFCFileClose();
                    flag = (MobileDeviceBase.AFCRemovePath(this.hAFC, Encoding.UTF8.GetBytes(path)) == 0);
                }
            }
            else
            {
                flag = true;
            }
            result = flag;
            return result;
        }

        // Token: 0x06000059 RID: 89 RVA: 0x000044B8 File Offset: 0x000026B8
        public void Disconnect()
        {
            lock (this)
            {
                MobileDeviceBase.AMDeviceDisconnect(this.iPhoneHandle);
            }
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00002E3C File Offset: 0x0000103C
        public unsafe void EnsureFileExists(string pathOnDevice, string service)
        {
            lock (this)
            {
                this.nolock_ConnectToService(service, false);
                void* dict = null;
                this.nolock_AFCFileClose();
                int num = MobileDeviceBase.AFCFileInfoOpen(this.hAFC, Encoding.UTF8.GetBytes(pathOnDevice), ref dict);
                if (num == 0)
                {
                    MobileDeviceBase.AFCKeyValueClose(dict);
                }
                if (num != 0)
                {
                    long num2;
                    if (this.nolock_AFCFileOpen(pathOnDevice, service, OpenMode.Read, out num2))
                    {
                        this.nolock_AFCFileClose();
                    }
                    else if (this.nolock_AFCFileOpen(pathOnDevice, service, OpenMode.ReadSafe, out num2))
                    {
                        this.nolock_AFCFileClose();
                    }
                }
            }
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00002DA8 File Offset: 0x00000FA8
        public bool eventArgsContainDeviceHandle(ConnectEventArgs args)
        {
            return args.Device == this.iPhoneHandle;
        }

        // Token: 0x06000053 RID: 83 RVA: 0x00004284 File Offset: 0x00002484
        public unsafe bool Exists(string path, string service)
        {
            bool result;
            if (service == "APPS_DIR")
            {
                result = true;
            }
            else if (path == null)
            {
                result = false;
            }
            else
            {
                bool flag = false;
                lock (this)
                {
                    if (this.nolock_ConnectToService(service, false))
                    {
                        this.nolock_AFCFileClose();
                        void* dict = null;
                        if (MobileDeviceBase.AFCFileInfoOpen(this.hAFC, Encoding.UTF8.GetBytes(path), ref dict) == 0)
                        {
                            MobileDeviceBase.AFCKeyValueClose(dict);
                            flag = true;
                        }
                    }
                }
                result = flag;
            }
            return result;
        }

        // Token: 0x0600004D RID: 77 RVA: 0x000040C8 File Offset: 0x000022C8
        public ulong FileSize(string path, string service)
        {
            ulong result;
            bool flag;
            ulong num;
            this.GetFileInfo(path, out result, out flag, out num, service);
            return result;
        }

        // Token: 0x06000046 RID: 70 RVA: 0x00003B80 File Offset: 0x00001D80
        public unsafe string[] GetContents(string path, string serviceName, bool showHiddenFiles)
        {
            List<string> list = new List<string>();
            lock (this)
            {
                if (this.nolock_ConnectToService(serviceName, false))
                {
                    void* dir = null;
                    this.nolock_AFCFileClose();
                    int num = MobileDeviceBase.AFCDirectoryOpen(this.hAFC, Encoding.UTF8.GetBytes(path), ref dir);
                    if (num != 0)
                    {
                        int num2 = 4;
                        while (num2 > 0 && num != 0)
                        {
                            if (this.nolock_ConnectToService(serviceName, true))
                            {
                                num = MobileDeviceBase.AFCDirectoryOpen(this.hAFC, Encoding.UTF8.GetBytes(path), ref dir);
                            }
                            num2--;
                        }
                    }
                    if (num == 0)
                    {
                        string text = null;
                        MobileDeviceBase.IDDirectoryRead(this.hAFC, dir, ref text);
                        while (text != null)
                        {
                            if ((showHiddenFiles || !text.StartsWith(".")) && text != "." && text != "..")
                            {
                                list.Add(text.ToString());
                            }
                            MobileDeviceBase.IDDirectoryRead(this.hAFC, dir, ref text);
                        }
                        MobileDeviceBase.AFCDirectoryClose(this.hAFC, dir);
                    }
                }
            }
            if (!this.connected)
            {
                throw new Exception("Not connected to phone");
            }
            return list.ToArray();
        }

        // Token: 0x06000049 RID: 73 RVA: 0x00003DE0 File Offset: 0x00001FE0
        public unsafe Dictionary<string, string> GetDeviceInfo()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            void* ptr = null;
            lock (this)
            {
                this.nolock_AFCFileClose();
                if (MobileDeviceBase.AFCDeviceInfoOpen(this.hAFC, ref ptr) == 0 && ptr != null)
                {
                    void* ptr2;
                    void* ptr3;
                    while (MobileDeviceBase.AFCKeyValueRead(ptr, out ptr2, out ptr3) == 0 && ptr2 != null && ptr3 != null)
                    {
                        string key = Marshal.PtrToStringAnsi(new IntPtr(ptr2));
                        string value = Marshal.PtrToStringAnsi(new IntPtr(ptr3));
                        dictionary.Add(key, value);
                    }
                    MobileDeviceBase.AFCKeyValueClose(ptr);
                }
            }
            return dictionary;
        }

        // Token: 0x06000045 RID: 69 RVA: 0x00003B0C File Offset: 0x00001D0C
        public string[] GetDirectories(string path, string service, bool includeHiddenDirectories)
        {
            string[] array = this.GetContents(path, service, includeHiddenDirectories);
            if (array != null)
            {
                List<string> list = new List<string>(array.Length);
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string text = array2[i];
                    string path2 = path + "/" + text;
                    if (this.IsDirectory(path2, service))
                    {
                        list.Add(text);
                    }
                }
                array = list.ToArray();
            }
            return array;
        }

        // Token: 0x06000052 RID: 82 RVA: 0x00004270 File Offset: 0x00002470
        private string GetDirectoryRoot(string path)
        {
            return "/";
        }

        // Token: 0x0600004A RID: 74 RVA: 0x00003E9C File Offset: 0x0000209C
        public unsafe Dictionary<string, string> GetFileInfo(string path, string service)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            void* ptr = null;
            lock (this)
            {
                if (this.nolock_ConnectToService(service, false))
                {
                    this.nolock_AFCFileClose();
                    if (MobileDeviceBase.AFCFileInfoOpen(this.hAFC, Encoding.UTF8.GetBytes(path), ref ptr) == 0 && ptr != null)
                    {
                        void* ptr2;
                        void* ptr3;
                        while (MobileDeviceBase.AFCKeyValueRead(ptr, out ptr2, out ptr3) == 0 && ptr2 != null && ptr3 != null)
                        {
                            string key = Marshal.PtrToStringAnsi(new IntPtr(ptr2));
                            string value = Marshal.PtrToStringAnsi(new IntPtr(ptr3));
                            dictionary.Add(key, value);
                        }
                        MobileDeviceBase.AFCKeyValueClose(ptr);
                    }
                }
            }
            return dictionary;
        }

        // Token: 0x0600004C RID: 76 RVA: 0x00003F98 File Offset: 0x00002198
        public unsafe void GetFileInfo(string path, out ulong size, out bool directory, out ulong dateInt, string service)
        {
            Dictionary<string, string> fileInfo = this.GetFileInfo(path, service);
            if (fileInfo.ContainsKey("st_size"))
            {
                size = ulong.Parse(fileInfo["st_size"]);
            }
            else
            {
                size = 0uL;
            }
            if (fileInfo.ContainsKey("st_mtime"))
            {
                dateInt = ulong.Parse(fileInfo["st_mtime"]);
            }
            else
            {
                dateInt = 0uL;
            }
            bool flag = false;
            directory = false;
            if (fileInfo.ContainsKey("st_ifmt"))
            {
                string text = fileInfo["st_ifmt"];
                if (text != null)
                {
                    if (!(text == "S_IFDIR"))
                    {
                        if (text == "S_IFLNK")
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        directory = true;
                    }
                }
            }
            if (flag)
            {
                void* dir = null;
                lock (this)
                {
                    this.nolock_AFCFileClose();
                    if (directory = (MobileDeviceBase.AFCDirectoryOpen(this.hAFC, Encoding.UTF8.GetBytes(path), ref dir) == 0))
                    {
                        MobileDeviceBase.AFCDirectoryClose(this.hAFC, dir);
                    }
                }
            }
        }

        // Token: 0x06000044 RID: 68 RVA: 0x00003A9C File Offset: 0x00001C9C
        public string[] GetFiles(string path, string service, bool showHiddenFiles)
        {
            string[] array = this.GetContents(path, service, showHiddenFiles);
            if (array != null)
            {
                List<string> list = new List<string>(array.Length);
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string text = array2[i];
                    string path2 = path + "/" + text;
                    if (!this.IsDirectory(path2, service))
                    {
                        list.Add(text);
                    }
                }
                array = list.ToArray();
            }
            return array;
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00003F70 File Offset: 0x00002170
        public ulong GetFileSize(string path, string service)
        {
            ulong result = 0uL;
            bool flag;
            ulong num;
            this.GetFileInfo(path, out result, out flag, out num, service);
            return result;
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00003D74 File Offset: 0x00001F74
        public long getFreeDeviceCapacity()
        {
            long result = 0L;
            lock (this)
            {
                Dictionary<string, string> deviceInfo = this.GetDeviceInfo();
                string s = null;
                deviceInfo.TryGetValue("FSFreeBytes", out s);
                long.TryParse(s, out result);
            }
            return result;
        }

        // Token: 0x06000047 RID: 71 RVA: 0x00003CE8 File Offset: 0x00001EE8
        public ulong getTotalDeviceCapacity()
        {
            if (this._totalDeviceCapacity == 0uL)
            {
                ulong totalDeviceCapacity = 0uL;
                lock (this)
                {
                    Dictionary<string, string> deviceInfo = this.GetDeviceInfo();
                    string s = null;
                    deviceInfo.TryGetValue("FSTotalBytes", out s);
                    ulong.TryParse(s, out totalDeviceCapacity);
                }
                this._totalDeviceCapacity = totalDeviceCapacity;
            }
            return this._totalDeviceCapacity;
        }

        // Token: 0x06000041 RID: 65 RVA: 0x000037E4 File Offset: 0x000019E4
        private string internationalFileName(string filePath)
        {
            string result;
            try
            {
                result = Path.GetFileName(filePath);
            }
            catch (Exception ex)
            {
                LogViewer.LogEvent(2, "internationalFileName: " + ex.ToString());
                string[] array = filePath.Split(new char[]
                {
                    '\\'
                });
                string text = array[array.Length - 1];
                char[] array2 = Path.GetInvalidPathChars();
                for (int i = 0; i < array2.Length; i++)
                {
                    char c = array2[i];
                    text = text.Replace(c.ToString(), "");
                }
                array2 = Path.GetInvalidFileNameChars();
                for (int i = 0; i < array2.Length; i++)
                {
                    char c = array2[i];
                    text = text.Replace(c.ToString(), "");
                }
                if (text.Length == 0)
                {
                    text = Path.GetRandomFileName();
                }
                else if (Path.GetFileNameWithoutExtension(text).Length == 0)
                {
                    text = Path.GetRandomFileName() + Path.GetExtension(text);
                }
                result = text;
            }
            return result;
        }

        // Token: 0x06000072 RID: 114 RVA: 0x00005390 File Offset: 0x00003590
        private bool IOS8OrLater()
        {
            string[] array = this.ProductVersion.Split(new char[]
            {
                '.'
            });
            int num;
            return int.TryParse(array[0], out num) && num >= 8;
        }

        // Token: 0x06000062 RID: 98 RVA: 0x000049B4 File Offset: 0x00002BB4
        public bool IsCoreFoundationOK()
        {
            string fileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles) + "\\Apple\\Apple Application Support\\CoreFoundation.dll";
            FileVersionInfo versionInfo;
            bool result;
            try
            {
                versionInfo = FileVersionInfo.GetVersionInfo(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = true;
                return result;
            }
            string[] badCoreFoundationVersions = this.BadCoreFoundationVersions;
            for (int i = 0; i < badCoreFoundationVersions.Length; i++)
            {
                string b = badCoreFoundationVersions[i];
                if (versionInfo.FileVersion == b)
                {
                    result = false;
                    return result;
                }
            }
            result = true;
            return result;
        }

        // Token: 0x06000054 RID: 84 RVA: 0x00004324 File Offset: 0x00002524
        public bool IsDirectory(string path, string service)
        {
            ulong num;
            bool result;
            ulong num2;
            this.GetFileInfo(path, out num, out result, out num2, service);
            return result;
        }

        // Token: 0x0600005F RID: 95 RVA: 0x000048BC File Offset: 0x00002ABC
        private void nolock_AFCFileClose()
        {
            if (this._AFCFilePath != null)
            {
                MobileDeviceBase.AFCFileRefClose(this.AFCHandle, this._AFCFileHandle);
                this._AFCFileHandle = 0L;
                this._AFCFileMode = OpenMode.None;
                this._AFCFilePath = null;
                this._AFCFileService = null;
            }
        }

        // Token: 0x0600005E RID: 94 RVA: 0x000048A4 File Offset: 0x00002AA4
        private bool nolock_AFCFileOpen(string path, string service, OpenMode mode)
        {
            long num;
            return this.nolock_AFCFileOpen(path, service, mode, out num);
        }

        // Token: 0x0600005D RID: 93 RVA: 0x000047B0 File Offset: 0x000029B0
        private bool nolock_AFCFileOpen(string path, string service, OpenMode mode, out long handle)
        {
            bool result;
            if (this._AFCFilePath != null)
            {
                if (this._AFCFilePath == path && this._AFCFileService == service && this._AFCFileMode == mode)
                {
                    handle = this._AFCFileHandle;
                    result = true;
                    return result;
                }
                this.nolock_AFCFileClose();
            }
            handle = 0L;
            if (!this.nolock_ConnectToService(service, false))
            {
                result = false;
            }
            else
            {
                if (MobileDeviceBase.AFCFileRefOpen(this.AFCHandle, Encoding.UTF8.GetBytes(path), (ulong)((long)mode), out this._AFCFileHandle) != 0)
                {
                    if (mode != OpenMode.Read)
                    {
                        result = false;
                        return result;
                    }
                    if (MobileDeviceBase.AFCFileRefOpen(this.AFCHandle, Encoding.UTF8.GetBytes(path), 1uL, out this._AFCFileHandle) != 0)
                    {
                        result = false;
                        return result;
                    }
                }
                this._AFCFilePath = path;
                this._AFCFileService = service;
                this._AFCFileMode = mode;
                handle = this._AFCFileHandle;
                result = true;
            }
            return result;
        }

        // Token: 0x0600005A RID: 90 RVA: 0x000044FC File Offset: 0x000026FC
        private bool nolock_ConnectToDevice()
        {
            MobileDeviceBase.AMDeviceDisconnect(this.iPhoneHandle);
            return MobileDeviceBase.AMDeviceConnect(this.iPhoneHandle) == 0 && MobileDeviceBase.AMDeviceIsPaired(this.iPhoneHandle) == 1 && MobileDeviceBase.AMDeviceValidatePairing(this.iPhoneHandle) == 0 && MobileDeviceBase.AMDeviceStartSession(this.iPhoneHandle) == 0;
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00004568 File Offset: 0x00002768
        private unsafe bool nolock_ConnectToService(string newService, bool forceReconnect = false)
        {
            if (forceReconnect)
            {
                this.currentBundleID = null;
            }
            bool flag = this.currentBundleID != null;
            bool result;
            if (newService == null || newService.Length <= 1)
            {
                result = false;
            }
            else
            {
                if (!(newService == this.currentBundleID) || this.currentBundleID == null)
                {
                    this.nolock_AFCFileClose();
                    if (this.hAFC != null)
                    {
                        MobileDeviceBase.AFCConnectionClose(this.hAFC);
                        this.hAFC = null;
                    }
                    for (int i = 3; i > 0; i--)
                    {
                        if (flag)
                        {
                            void* ptr = MobileDeviceBase.__CFStringMakeConstantString(MobileDeviceBase.StringToCString(newService));
                            bool flag2;
                            if (newService == "com.apple.afc" || newService == "com.apple.afc2")
                            {
                                flag2 = (MobileDeviceBase.AMDeviceStartService(this.iPhoneHandle, ptr, ref this.hService, null) == 0);
                            }
                            else
                            {
                                flag2 = (this.StartHouseArrest(ptr) == 0);
                            }
                            if (flag2)
                            {
                                this.connected = (MobileDeviceBase.AFCConnectionOpen(this.hService, 0u, ref this.hAFC) == 0);
                                if (this.connected)
                                {
                                    this.currentBundleID = newService;
                                    IL_17B:
                                    result = this.connected;
                                    return result;
                                }
                            }
                        }
                        this.currentBundleID = null;
                        int num = 3;
                        while (num > 0 && MobileDeviceBase.AMDeviceDisconnect(this.iPhoneHandle) != 0)
                        {
                            num--;
                        }
                        this.connected = false;
                        int num2 = 3;
                        while (num2 > 0 && !this.nolock_ConnectToDevice())
                        {
                            num2--;
                        }
                        if (num2 == 0)
                        {
                            this.connected = false;
                            result = false;
                            return result;
                        }
                        flag = true;
                    }
                    //goto IL_17B;
                }
                result = true;
            }
            return result;
        }

        // Token: 0x06000060 RID: 96 RVA: 0x0000490C File Offset: 0x00002B0C
        public bool OpenFile(string path, string service, OpenMode mode)
        {
            bool result = false;
            lock (this)
            {
                result = this.nolock_AFCFileOpen(path, service, mode);
            }
            return result;
        }

        // Token: 0x06000064 RID: 100 RVA: 0x00004BD0 File Offset: 0x00002DD0
        private bool publicDeleteDirectory(string path, string service)
        {
            if (path.EndsWith("/"))
            {
                path = path.Remove(path.Length - 1);
            }
            try
            {
                if (this.IsDirectory(path, service))
                {
                    string[] array = this.GetFiles(path, service, true);
                    for (int i = 0; i < array.Length; i++)
                    {
                        this.DeleteFile(path + "/" + array[i], service);
                    }
                    array = this.GetDirectories(path, service, true);
                    if (array != null)
                    {
                        for (int i = 0; i < array.Length; i++)
                        {
                            string path2 = XEPathInfo.UnixPathCombine(path, array[i]);
                            if (this.IsDirectory(path2, service))
                            {
                                this.publicDeleteDirectory(path + "/" + array[i], service);
                            }
                        }
                    }
                }
                this.DeleteDirectory(path, service);
            }
            catch (Exception ex)
            {
                LogViewer.LogEvent(0, ex.ToString());
            }
            return true;
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00003344 File Offset: 0x00001544
        public long ReadBytesFromFile(string pathOnDevice, string service, long fileOffset, long dataLength, byte[] buffer, bool leaveOpen = false)
        {
            long result;
            if (pathOnDevice.Length == 0)
            {
                result = 0L;
            }
            else
            {
                if (pathOnDevice[0] != '/')
                {
                    pathOnDevice = "/" + pathOnDevice;
                }
                uint num = (uint)this.FileSize(pathOnDevice, service);
                dataLength = Math.Min(dataLength, (long)((ulong)num));
                uint num2 = (uint)dataLength;
                lock (this)
                {
                    long handle;
                    if (this.nolock_AFCFileOpen(pathOnDevice, service, OpenMode.Read, out handle))
                    {
                        if (MobileDeviceBase.AFCFileRefSeek(this.AFCHandle, handle, fileOffset, 0L) != 0 || MobileDeviceBase.AFCFileRefRead(this.AFCHandle, handle, buffer, ref num2) != 0)
                        {
                        }
                        if (!leaveOpen)
                        {
                            this.nolock_AFCFileClose();
                        }
                    }
                }
                result = dataLength;
            }
            return result;
        }

        // Token: 0x06000071 RID: 113 RVA: 0x000052E4 File Offset: 0x000034E4
        private unsafe Dictionary<string, object> ReadXMLReply(IntPtr service)
        {
            int i = 0;
            int num = Winsock.recv(service, (IntPtr)((void*)(&i)), 4, 0);
            Dictionary<string, object> result;
            if (4 != num)
            {
                result = null;
            }
            else
            {
                i = IPAddress.NetworkToHostOrder(i);
                if (i == 0)
                {
                    result = null;
                }
                else
                {
                    byte[] array = new byte[i];
                    byte[] array2;
                    byte* ptr;
                    if ((array2 = array) != null && array2.Length != 0)
                    {
                        fixed (byte* _ptr = &array2[0])
                        {
                            ptr = _ptr;
                        }
                    }
                    else
                    {
                        ptr = null;
                    }
                    byte* ptr2 = ptr;
                    while (i > 0)
                    {
                        num = Winsock.recv(service, (IntPtr)((void*)ptr2), i, 0);
                        if (num == 0)
                        {
                            result = null;
                            return result;
                        }
                        i -= num;
                        ptr2 += num;
                    }
                    ptr = null;
                    result = (Dictionary<string, object>)PlistCS.Plist.readPlist(array);
                }
            }
            return result;
        }

        // Token: 0x0600002D RID: 45 RVA: 0x00002B90 File Offset: 0x00000D90
        private static string RemoveInvalXEFileSystemCharactersFromFileName(string fileName)
        {
            string text = fileName;
            if (text != null)
            {
                char[] invalXEFileNameChars = Path.GetInvalidFileNameChars();
                char[] array = invalXEFileNameChars;
                for (int i = 0; i < array.Length; i++)
                {
                    char c = array[i];
                    text = text.Replace(c.ToString(), "");
                }
            }
            return text;
        }

        // Token: 0x06000051 RID: 81 RVA: 0x000041F4 File Offset: 0x000023F4
        public bool Rename(string sourceName, string destName, string service)
        {
            bool result = false;
            lock (this)
            {
                if (this.nolock_ConnectToService(service, false))
                {
                    this.nolock_AFCFileClose();
                    if (MobileDeviceBase.AFCRenamePath(this.hAFC, Encoding.UTF8.GetBytes(sourceName), Encoding.UTF8.GetBytes(destName)) == 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        // Token: 0x0600006F RID: 111 RVA: 0x00005198 File Offset: 0x00003398
        private unsafe Dictionary<string, object> SendRequestAndWaitForSingleReply(string serviceName, Dictionary<string, object> message)
        {
            Dictionary<string, object> result = null;
            lock (this)
            {
                void* ptr = null;
                void* service_name = MobileDeviceBase.__CFStringMakeConstantString(MobileDeviceBase.StringToCString(serviceName));
                int num = MobileDeviceBase.AMDeviceStartService(this.iPhoneHandle, service_name, ref ptr, null);
                try
                {
                    if (num == 0 && this.SendXMLRequest((IntPtr)ptr, message))
                    {
                        result = this.ReadXMLReply((IntPtr)ptr);
                    }
                }
                finally
                {
                    if (ptr != null)
                    {
                        Winsock.closesocket((IntPtr)ptr);
                    }
                }
            }
            return result;
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00005248 File Offset: 0x00003448
        private unsafe bool SendXMLRequest(IntPtr service, Dictionary<string, object> message)
        {
            string text = PlistCS.Plist.writeXml(message);
            bool result;
            if (string.IsNullOrEmpty(text))
            {
                result = false;
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                int num = IPAddress.HostToNetworkOrder(bytes.Length);
                int num2 = Winsock.send(service, (IntPtr)((void*)(&num)), 4, 0);
                if (num2 != 4)
                {
                    result = false;
                }
                else
                {
                    byte[] array;
                    byte* ptr;
                    if ((array = bytes) != null && array.Length != 0)
                    {
                        fixed (byte* _ptr = &array[0])
                        {
                            ptr = _ptr;
                        }
                    }
                    else
                    {
                        ptr = null;
                    }
                    num2 = Winsock.send(service, (IntPtr)((void*)ptr), bytes.Length, 0);
                    ptr = null;
                    result = (num2 == bytes.Length);
                }
            }
            return result;
        }

        // Token: 0x0600004E RID: 78 RVA: 0x000040E8 File Offset: 0x000022E8
        public void SetFileSize(string path, string service, ulong size)
        {
            lock (this)
            {
                long handle;
                if (this.nolock_ConnectToService(service, false) && this.nolock_AFCFileOpen(path, service, OpenMode.ReadWrite, out handle))
                {
                    MobileDeviceBase.AFCFileRefSetFileSize(this.AFCHandle, handle, (uint)size);
                    this.nolock_AFCFileClose();
                }
            }
        }

        // Token: 0x06000058 RID: 88 RVA: 0x0000207C File Offset: 0x0000027C
        public void SetManager(iPhoneXEManager mgr)
        {
            this._manager = new WeakReference(mgr);
        }

        // Token: 0x0600006E RID: 110 RVA: 0x0000512C File Offset: 0x0000332C
        public byte[] SpringboardIconPNGData(string bundleId)
        {
            Dictionary<string, object> message = new Dictionary<string, object>
            {
                {
                    "command",
                    "getIconPNGData"
                },
                {
                    "bundleId",
                    bundleId
                }
            };
            Dictionary<string, object> dictionary = this.SendRequestAndWaitForSingleReply("com.apple.springboardservices", message);
            byte[] result;
            if (dictionary != null && dictionary.ContainsKey("pngData"))
            {
                result = (byte[])dictionary["pngData"];
            }
            else
            {
                result = null;
            }
            return result;
        }

        // Token: 0x0600005C RID: 92 RVA: 0x00004708 File Offset: 0x00002908
        private unsafe int StartHouseArrest(void* serviceCFString)
        {
            int num = MobileDeviceBase.AMDeviceStartHouseArrestService(this.iPhoneHandle, serviceCFString, null, ref this.hService, 0);
            if (num != 0)
            {
                void* ptr = MobileDeviceBase.__CFStringMakeConstantString(MobileDeviceBase.StringToCString("Command"));
                void* ptr2 = MobileDeviceBase.__CFStringMakeConstantString(MobileDeviceBase.StringToCString("VendDocuments"));
                void*[] keys = new void*[]
                {
                    ptr
                };
                void*[] values = new void*[]
                {
                    ptr2
                };
                void* ptr3 = MobileDeviceBase.CFDictionaryCreate(null, keys, values, 1L, null, null);
                num = MobileDeviceBase.AMDeviceStartHouseArrestService(this.iPhoneHandle, serviceCFString, ptr3, ref this.hService, 0);
                MobileDeviceBase.CFRelease(ptr3);
            }
            return num;
        }

        // Token: 0x0600003E RID: 62 RVA: 0x00003428 File Offset: 0x00001628
        public long WriteBytesToFile(string pathOnDevice, string service, long fileOffset, long dataLength, byte[] buffer, bool leaveOpen = false)
        {
            long result;
            if (pathOnDevice.Length == 0)
            {
                result = 0L;
            }
            else
            {
                if (pathOnDevice[0] != '/')
                {
                    pathOnDevice = "/" + pathOnDevice;
                }
                long num = -1L;
                lock (this)
                {
                    long handle;
                    if (this.nolock_ConnectToService(service, false) && this.nolock_AFCFileOpen(pathOnDevice, service, OpenMode.ReadWrite, out handle) && MobileDeviceBase.AFCFileRefSeek(this.AFCHandle, handle, fileOffset, 0L) == 0)
                    {
                        if (MobileDeviceBase.AFCFileRefWrite(this.AFCHandle, handle, buffer, (uint)dataLength) == 0)
                        {
                            num = dataLength;
                        }
                        else
                        {
                            num = 0L;
                        }
                        if (!leaveOpen)
                        {
                            this.nolock_AFCFileClose();
                        }
                    }
                }
                result = num;
            }
            return result;
        }

        // Token: 0x1700000F RID: 15
        public string ActivationState
        {
            // Token: 0x06000031 RID: 49 RVA: 0x00002D40 File Offset: 0x00000F40
            get
            {
                string result;
                lock (this)
                {
                    result = MobileDeviceBase.AMDeviceCopyValue(this.iPhoneHandle, "ActivationState");
                }
                return result;
            }
        }

        // Token: 0x17000013 RID: 19
        public unsafe void* AFCHandle
        {
            // Token: 0x06000036 RID: 54 RVA: 0x00002DFC File Offset: 0x00000FFC
            get
            {
                return this.hAFC;
            }
        }

        // Token: 0x17000012 RID: 18
        public unsafe void* Device
        {
            // Token: 0x06000035 RID: 53 RVA: 0x00002DE4 File Offset: 0x00000FE4
            get
            {
                return this.iPhoneHandle;
            }
        }

        // Token: 0x17000017 RID: 23
        public bool DeviceHasCamera
        {
            // Token: 0x0600006A RID: 106 RVA: 0x00005000 File Offset: 0x00003200
            get
            {
                if (!this._deviceCameraStatusCached)
                {
                    string text = this.CopyDeviceValue("ProductType").ToLower();
                    foreach (string value in new ArrayList
                    {
                        "ipod1",
                        "ipod2",
                        "ipod3",
                        "ipad1"
                    })
                    {
                        if (text.Contains(value))
                        {
                            this._deviceHasCamera = false;
                            break;
                        }
                    }
                    this._deviceCameraStatusCached = true;
                }
                return this._deviceHasCamera;
            }
        }

        // Token: 0x17000018 RID: 24
        public bool DeviceHasPhone
        {
            // Token: 0x0600006B RID: 107 RVA: 0x000050CC File Offset: 0x000032CC
            get
            {
                string text = this.CopyDeviceValue("ProductType").ToLower();
                return text.StartsWith("iphone");
            }
        }

        // Token: 0x1700000E RID: 14
        public string deviceID
        {
            // Token: 0x06000030 RID: 48 RVA: 0x00002CDC File Offset: 0x00000EDC
            get
            {
                if (this._deviceID == null)
                {
                    lock (this)
                    {
                        this._deviceID = MobileDeviceBase.AMDeviceCopyValue(this.iPhoneHandle, "UniqueDeviceID");
                    }
                }
                return this._deviceID;
            }
        }

        // Token: 0x17000016 RID: 22
        public Uri DeviceImageUri
        {
            // Token: 0x06000066 RID: 102 RVA: 0x00004D54 File Offset: 0x00002F54
            get
            {
                if (this._deviceImageUriCached == null)
                {
                    ProductInfo productInfoForDevice = ProductInfo.GetProductInfoForDevice(this);
                    ProductInfo.ModelType model = productInfoForDevice.Model;
                    string arg;
                    string arg2;
                    if (model <= ProductInfo.ModelType.iPhoneUnknown)
                    {
                        if (model != ProductInfo.ModelType.Unknown)
                        {
                            switch (model)
                            {
                                case ProductInfo.ModelType.iPhone:
                                case ProductInfo.ModelType.iPhone3G:
                                case ProductInfo.ModelType.iPhone3Gs:
                                    arg = "iPhone3";
                                    arg2 = iPhoneXE.ColorStringBlackOrWhite(productInfoForDevice.Color);
                                    goto IL_19F;
                                case ProductInfo.ModelType.iPhone4:
                                case ProductInfo.ModelType.iPhone4s:
                                    arg = "iPhone4";
                                    arg2 = iPhoneXE.ColorStringBlackOrWhite(productInfoForDevice.Color);
                                    goto IL_19F;
                                case ProductInfo.ModelType.iPhone5:
                                    arg = "iPhone5";
                                    arg2 = iPhoneXE.ColorStringBlackOrWhite(productInfoForDevice.Color);
                                    goto IL_19F;
                                case ProductInfo.ModelType.iPhone5c:
                                    arg = "iPhone5c";
                                    arg2 = iPhoneXE.ColorStringColorful(productInfoForDevice.Color);
                                    goto IL_19F;
                                case ProductInfo.ModelType.iPhone5s:
                                    arg = "iPhone5s";
                                    arg2 = iPhoneXE.ColorStringGoldSilverOrGray(productInfoForDevice.Color);
                                    goto IL_19F;
                                case ProductInfo.ModelType.iPhone6:
                                case ProductInfo.ModelType.iPhone6Plus:
                                case ProductInfo.ModelType.iPhoneUnknown:
                                    break;
                                default:
                                    goto IL_157;
                            }
                        }
                        arg = "iPhone6";
                        arg2 = iPhoneXE.ColorStringGoldSilverOrGray(productInfoForDevice.Color);
                        goto IL_19F;
                    }
                    switch (model)
                    {
                        case ProductInfo.ModelType.iPad:
                            arg = "iPad1";
                            arg2 = iPhoneXE.ColorStringBlackOrWhite(productInfoForDevice.Color);
                            goto IL_19F;
                        case ProductInfo.ModelType.iPad2:
                        case ProductInfo.ModelType.iPad3:
                        case ProductInfo.ModelType.iPad4:
                        case ProductInfo.ModelType.iPadAir:
                        case ProductInfo.ModelType.iPadAir2:
                        case ProductInfo.ModelType.iPadMini:
                        case ProductInfo.ModelType.iPadMini2:
                        case ProductInfo.ModelType.iPadMini3:
                        case ProductInfo.ModelType.iPadUnknown:
                            arg = "iPad2";
                            arg2 = iPhoneXE.ColorStringBlackOrWhite(productInfoForDevice.Color);
                            goto IL_19F;
                        default:
                            switch (model)
                            {
                                case ProductInfo.ModelType.iPodTouch:
                                case ProductInfo.ModelType.iPodTouch2:
                                case ProductInfo.ModelType.iPodTouch3:
                                case ProductInfo.ModelType.iPodTouch4:
                                case ProductInfo.ModelType.iPodTouch5:
                                case ProductInfo.ModelType.iPodTouchUnknown:
                                    arg = "iTouch";
                                    arg2 = iPhoneXE.ColorStringBlackOrWhite(productInfoForDevice.Color);
                                    goto IL_19F;
                            }
                            break;
                    }
                    IL_157:
                    arg = "iPhone6";
                    arg2 = "Gray";
                    IL_19F:
                    this._deviceImageUriCached = new Uri(string.Format("/MK.MobileDevice.XEDevice;component/DeviceIcons/{0}{1}.png", arg, arg2), UriKind.Relative);
                }
                return this._deviceImageUriCached;
            }
        }

        // Token: 0x1700000D RID: 13
        public string deviceName
        {
            // Token: 0x0600002E RID: 46 RVA: 0x00002BE0 File Offset: 0x00000DE0
            get
            {
                if (this.myDeviceName == null)
                {
                    this.myDeviceName = this.CopyDeviceValue("DeviceName");
                    this.myDeviceName = iPhoneXE.RemoveInvalXEFileSystemCharactersFromFileName(this.myDeviceName);
                    if (this.myDeviceName.Length <= 0)
                    {
                        this.myDeviceName = "iOS Device";
                    }
                }
                string result;
                if (this.myDeviceName == "Unknown")
                {
                    result = "iOS Device";
                }
                else
                {
                    result = this.myDeviceName;
                }
                return result;
            }
        }

        // Token: 0x17000010 RID: 16
        public bool IsAFCHandleNotNull
        {
            // Token: 0x06000032 RID: 50 RVA: 0x00002D8C File Offset: 0x00000F8C
            get
            {
                return this.hAFC != null;
            }
        }

        // Token: 0x17000011 RID: 17
        public bool IsConnected
        {
            // Token: 0x06000034 RID: 52 RVA: 0x00002DD0 File Offset: 0x00000FD0
            get
            {
                return this.connected;
            }
        }

        // Token: 0x17000014 RID: 20
        public bool IsJailbreak
        {
            // Token: 0x06000037 RID: 55 RVA: 0x00002E14 File Offset: 0x00001014
            get
            {
                return this.wasAFC2;
            }
        }

        // Token: 0x17000015 RID: 21
        public bool IsUsingAFC2
        {
            // Token: 0x06000038 RID: 56 RVA: 0x00002E28 File Offset: 0x00001028
            get
            {
                return this.isOnAFC2;
            }
        }

        // Token: 0x17000019 RID: 25
        public string ProductVersion
        {
            // Token: 0x0600006C RID: 108 RVA: 0x000050F8 File Offset: 0x000032F8
            get
            {
                if (this._productVersion == null)
                {
                    this._productVersion = this.CopyDeviceValue("ProductVersion");
                }
                return this._productVersion;
            }
        }

        // Token: 0x0400002A RID: 42
        private Dictionary<string, XEDeviceApplicationInfo> appAndBundleDict;

        // Token: 0x04000030 RID: 48
        public static bool AskBeforeOverwritingSameFile = true;

        // Token: 0x04000037 RID: 55
        private string[] BadCoreFoundationVersions = new string[]
        {
            "1,630,14,0"
        };

        // Token: 0x0400001E RID: 30
        public const string BundleID_AFC = "com.apple.afc";

        // Token: 0x0400001F RID: 31
        public const string BundleID_AFC2 = "com.apple.afc2";

        // Token: 0x04000021 RID: 33
        public const string BundleID_AppsDir = "APPS_DIR";

        // Token: 0x04000020 RID: 32
        public const string BundleID_Springboard = "com.apple.springboardservices";

        // Token: 0x04000027 RID: 39
        private bool connected;

        // Token: 0x0400002B RID: 43
        public string currentBundleID;

        // Token: 0x04000025 RID: 37
        public unsafe void* hAFC;

        // Token: 0x04000026 RID: 38
        public unsafe void* hService;

        // Token: 0x04000024 RID: 36
        public unsafe void* iPhoneHandle;

        // Token: 0x04000029 RID: 41
        private bool isOnAFC2 = false;

        // Token: 0x0400002D RID: 45
        private string myDeviceName;

        // Token: 0x04000023 RID: 35
        public bool showHiddenFilesAndFolders = false;

        // Token: 0x04000028 RID: 40
        private bool wasAFC2 = false;

        // Token: 0x04000035 RID: 53
        private long _AFCFileHandle;

        // Token: 0x04000036 RID: 54
        private OpenMode _AFCFileMode;

        // Token: 0x04000033 RID: 51
        private string _AFCFilePath;

        // Token: 0x04000034 RID: 52
        private string _AFCFileService;

        // Token: 0x04000038 RID: 56
        private Dictionary<string, string> _appImageDictionary = new Dictionary<string, string>();

        // Token: 0x0400002E RID: 46
        private Dictionary<string, string> _copyValueDictionary = new Dictionary<string, string>();

        // Token: 0x04000022 RID: 34
        private iPhoneXEDelegate _delegateObject;

        // Token: 0x0400003A RID: 58
        private bool _deviceCameraStatusCached = false;

        // Token: 0x0400003B RID: 59
        private bool _deviceHasCamera = true;

        // Token: 0x0400002F RID: 47
        private string _deviceID;

        // Token: 0x04000039 RID: 57
        private Uri _deviceImageUriCached = null;

        // Token: 0x04000032 RID: 50
        private WeakReference _manager;

        // Token: 0x0400003C RID: 60
        private string _productVersion = null;

        // Token: 0x04000031 RID: 49
        private ulong _totalDeviceCapacity = 0uL;

        // Token: 0x0400002C RID: 44
        private bool _transferCancelled = false;
    }
}
