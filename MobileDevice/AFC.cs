using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using imobileDeviceiDevice;

namespace imobileDeviceiDevice
{
    class AFC
    {
        internal enum AFCError
        {
            AFC_E_SUCCESS = 0,
            AFC_E_UNKNOWN_ERROR = 1,
            AFC_E_OP_HEADER_INVALID = 2,
            AFC_E_NO_RESOURCES = 3,
            AFC_E_READ_ERROR = 4,
            AFC_E_WRITE_ERROR = 5,
            AFC_E_UNKNOWN_PACKET_TYPE = 6,
            AFC_E_INVALID_ARG = 7,
            AFC_E_OBJECT_NOT_FOUND = 8,
            AFC_E_OBJECT_IS_DIR = 9,
            AFC_E_PERM_DENIED = 10,
            AFC_E_SERVICE_NOT_CONNECTED = 11,
            AFC_E_OP_TIMEOUT = 12,
            AFC_E_TOO_MUCH_DATA = 13,
            AFC_E_END_OF_DATA = 14,
            AFC_E_OP_NOT_SUPPORTED = 15,
            AFC_E_OBJECT_EXISTS = 16,
            AFC_E_OBJECT_BUSY = 17,
            AFC_E_NO_SPACE_LEFT = 18,
            AFC_E_OP_WOULD_BLOCK = 19,
            AFC_E_IO_ERROR = 20,
            AFC_E_OP_INTERRUPTED = 21,
            AFC_E_OP_IN_PROGRESS = 22,
            AFC_E_INTERNAL_ERROR = 23,
            AFC_E_MUX_ERROR = 30,
            AFC_E_NO_MEM = 31,
            AFC_E_NOT_ENOUGH_DATA = 32,
            AFC_E_DIR_NOT_EMPTY = 33,
            AFC_E_FORCE_SIGNED_TYPE = -1
        }
        public enum FileOpenMode
        {
            AFC_FOPEN_RDONLY = 0x00000001,
            AFC_FOPEN_RW = 0x00000002,
            AFC_FOPEN_WRONLY = 0x00000003,
            AFC_FOPEN_WR = 0x00000004,
            AFC_FOPEN_APPEND = 0x00000005,
            AFC_FOPEN_RDAPPEND = 0x00000006
        }
        public enum afc_link_type_t { 
		  AFC_HARDLINK = 1, 
		  AFC_SYMLINK = 2 
		}

        static string photoDatabasePath;

        #region DllImport
        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_client_start_service(IntPtr device, out IntPtr client, string label);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_read_directory(IntPtr client, string path, out IntPtr directoryInfo);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_get_file_info(IntPtr client, string fileName, out IntPtr fileInfo);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_file_open(IntPtr client, string fileName, FileOpenMode fileMode, out ulong handle);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_file_close(IntPtr client, ulong handle);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_file_read(IntPtr client, ulong handle, byte[] data, uint length, out uint bytesRead);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_client_free(IntPtr client);

        [DllImport(LibiMobileDevice.LibimobiledeviceDllPath, CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_dictionary_free(IntPtr dictionary);

        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_file_write(IntPtr afcClient, ulong fileHandle, byte[] data, uint length, out uint bytesWritten);

        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_remove_path(IntPtr afcClient, string filePath);
        
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_rename_path(IntPtr afcClient, string originalPath, string newPath);
        
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_remove_path_and_contents (IntPtr afcClient, string path);
        
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_make_directory (IntPtr afcClient, string path);
        
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_make_link (IntPtr afcClient,  afc_link_type_t linktype, string target, string linkname);
        
        [DllImport("libimobiledevice.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern AFCError afc_get_device_info (IntPtr afcClient, out IntPtr infoPtr);
        #endregion

        #region Main Functions
        public static AFCError CollectData(iDevice device, string savePath)
        {
            //CollectionForm.logWriter.WriteLine("[INFO] Starting AFC client.");
            IntPtr afcClient;
            AFCError returnCode = afc_client_start_service(device.Handle, out afcClient, "iOSLibDataCollector");
            if (returnCode != AFCError.AFC_E_SUCCESS || afcClient == IntPtr.Zero)
            {
                //CollectionForm.logWriter.WriteLine("[ERROR] Couldn't start AFC client. AFC error " + (int)returnCode + ": " + returnCode + ".");
                return returnCode;
            }
            //CollectionForm.logWriter.WriteLine("[INFO] AFC client has been successfully started.");

            int fileNumber = 0;
            string iOSVersion = device.iOSVersion.Replace(".", "_");
            string fileName;
            do
            {
                fileName = iOSVersion + (fileNumber != 0 ? " (" + fileNumber + ")" : "");
                fileNumber++;
            } while (File.Exists(savePath + @"\" + fileName + ".sqlite")
                || File.Exists(savePath + @"\" + fileName + ".sqlite-shm")
                || File.Exists(savePath + @"\" + fileName + ".sqlite-wal")
                || File.Exists(savePath + @"\" + fileName + ".txt"));
            savePath += @"\" + fileName;

            StreamWriter treeWriter = new StreamWriter(savePath + ".txt");

            //CollectionForm.logWriter.WriteLine("[INFO] Saving directory tree.");
            photoDatabasePath = "";
            string lastDirectory;
            if ((returnCode = saveDirectoryTree(afcClient, "/", treeWriter, out lastDirectory)) != AFCError.AFC_E_SUCCESS)
            {
                //CollectionForm.logWriter.WriteLine("[ERROR] Couldn't save directory tree. An error occurred while reading \"" + lastDirectory
                    //+ "\". AFC error " + (int)returnCode + ": " + returnCode + ".");
            }
            //CollectionForm.logWriter.WriteLine("[INFO] Directory saving has been finished.");

            if (photoDatabasePath != "")
            {
                //CollectionForm.logWriter.WriteLine("[INFO] Photos database file is located at " + photoDatabasePath + ".");
            }

            else
            {
                //CollectionForm.logWriter.WriteLine("[ERROR] Couldn't find photo database file.");
            }

            treeWriter.WriteLine("\n\r" + photoDatabasePath);
            treeWriter.Close();

            //CollectionForm.logWriter.WriteLine("[INFO] Saving photos database.");
            returnCode = savePhotosDatabase(afcClient, savePath + ".sqlite");

            afc_client_free(afcClient);
            return returnCode;
        }

        static AFCError saveDirectoryTree(IntPtr afcClient, string directoryPath, StreamWriter streamWriter, out string lastDirectory)
        {
            lastDirectory = directoryPath;

            AFCError returnCode;
            IntPtr directoryListPtr;
            returnCode = AFC.afc_read_directory(afcClient, directoryPath, out directoryListPtr);
            if (returnCode == AFCError.AFC_E_READ_ERROR)
            {
                return AFCError.AFC_E_SUCCESS;
            }
            else if (returnCode != AFCError.AFC_E_SUCCESS)
            {
                return returnCode;
            }

            List<string> directoryList = LibiMobileDevice.PtrToStringList(directoryListPtr, 2);
            afc_dictionary_free(directoryListPtr);

            if (directoryPath == "/")
            {
                directoryPath = "";
            }

            int tabNumber = directoryPath.Count(x => x == '/');
            directoryList.Sort();
            foreach (string currDirectory in directoryList)
            {
                if (currDirectory == "Photos.sqlite")
                {
                    photoDatabasePath = directoryPath + "/" + currDirectory;
                }

                streamWriter.WriteLine(String.Concat(Enumerable.Repeat("\t", tabNumber)) + currDirectory);
                if ((returnCode = saveDirectoryTree(afcClient, directoryPath + "/" + currDirectory, streamWriter, out lastDirectory))
                    != AFCError.AFC_E_SUCCESS)
                {
                    break;
                }
            }

            return returnCode;
        }

        static AFCError savePhotosDatabase(IntPtr afcClient, string savePath)
        {
            AFCError returnCode;
            if ((returnCode = saveFile(afcClient, photoDatabasePath, savePath)) != AFCError.AFC_E_SUCCESS)
            {
                //CollectionForm.logWriter.WriteLine("[ERROR] Couldn't save photos database. AFC error " + (int)returnCode + ": " + returnCode + ".");
                afc_client_free(afcClient);
                return returnCode;
            }
            //CollectionForm.logWriter.WriteLine("[INFO] Photos database has been saved successfully.");

            saveFile(afcClient, photoDatabasePath + "-shm", savePath + "-shm");
            saveFile(afcClient, photoDatabasePath + "-wal", savePath + "-wal");

            return returnCode;
        }
        #endregion

        internal static AFCError copyToDisk(IntPtr afcClient, string filePath, string savePath)
        {
            IntPtr infoPtr;
            AFCError returnCode = afc_get_file_info(afcClient, filePath, out infoPtr);
            if (returnCode != AFCError.AFC_E_SUCCESS)
            {
                return returnCode;
            }

            List<string> infoList = LibiMobileDevice.PtrToStringList(infoPtr, 0);
            long fileSize = Convert.ToInt64(infoList[infoList.FindIndex(x => x == "st_size") + 1]);

            ulong fileHandle;
            returnCode = afc_file_open(afcClient, filePath, FileOpenMode.AFC_FOPEN_RDONLY, out fileHandle);
            if (returnCode != AFCError.AFC_E_SUCCESS)
            {
                return returnCode;
            }

            FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            const int bufferSize = 4194304;
            for (int i = 0; i < fileSize / bufferSize + 1; i++)
            {
                uint bytesRead;

                long remainder = fileSize - i * bufferSize;
                int currBufferSize = remainder >= bufferSize ? bufferSize : (int)remainder;
                byte[] currBuffer = new byte[currBufferSize];

                if ((returnCode = afc_file_read(afcClient, fileHandle, currBuffer, Convert.ToUInt32(currBufferSize), out bytesRead))
                    != AFCError.AFC_E_SUCCESS)
                {
                    afc_file_close(afcClient, fileHandle);
                    return returnCode;
                }

                fileStream.Write(currBuffer, 0, currBufferSize);
            }

            fileStream.Close();
            returnCode = afc_file_close(afcClient, fileHandle);

            return returnCode;
        }

        internal static AFCError copyToDevice(IntPtr afcClient, string filePath, string savePath)
        {
            byte[] fileContent = File.ReadAllBytes(filePath);

            ulong fileHandle;
            AFCError returnCode = afc_file_open(afcClient, savePath, FileOpenMode.AFC_FOPEN_WRONLY, out fileHandle);
            if (returnCode != AFCError.AFC_E_SUCCESS)
            {
                return returnCode;
            }

            uint bytesWritten;
            returnCode = afc_file_write(afcClient, fileHandle, fileContent, Convert.ToUInt32(fileContent.Length), out bytesWritten);
            afc_file_close(afcClient, fileHandle);
            return returnCode;
        }
        
        #region Helper functions
        static AFCError saveFile(IntPtr afcClient, string filePath, string savePath)
        {
            AFCError returnCode;
            IntPtr fileInfoPtr;
            if ((returnCode = afc_get_file_info(afcClient, filePath, out fileInfoPtr)) != AFCError.AFC_E_SUCCESS)
            {
                return returnCode;
            }

            List<string> infoList = LibiMobileDevice.PtrToStringList(fileInfoPtr, 0);
            long fileSize = Convert.ToInt64(infoList[infoList.FindIndex(x => x == "st_size") + 1]);
            afc_dictionary_free(fileInfoPtr);

            ulong fileHandle;
            if ((returnCode = afc_file_open(afcClient, filePath, FileOpenMode.AFC_FOPEN_RDONLY, out fileHandle))
                != AFCError.AFC_E_SUCCESS)
            {
                return returnCode;
            }

            FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write);
            const int bufferSize = 4194304;
            for (int i = 0; i < fileSize / bufferSize + 1; i++)
            {
                uint bytesRead;

                long remainder = fileSize - i * bufferSize;
                int currBufferSize = remainder >= bufferSize ? bufferSize : (int)remainder;
                byte[] currBuffer = new byte[currBufferSize];
                if ((returnCode = afc_file_read(afcClient, fileHandle, currBuffer, Convert.ToUInt32(currBufferSize), out bytesRead))
                    == AFCError.AFC_E_SUCCESS)
                {
                    fileStream.Write(currBuffer, 0, currBufferSize);
                }
            }

            fileStream.Close();
            afc_file_close(afcClient, fileHandle);

            return returnCode;
        }
        #endregion
    }
}
