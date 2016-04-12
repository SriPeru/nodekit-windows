﻿/*
* nodekit.io
*
* Copyright (c) 2016 OffGrid Networks. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#if WINDOWS_WIN32

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using io.nodekit.NKScripting;
using System.Linq;
using System.IO;

namespace io.nodekit.NKCore
{
    public sealed class NKC_FileSystem
    {

        internal static NKC_FileSystem current = new NKC_FileSystem();
        //  private NKEventEmitter events = NKEventEmitter.global;

        #region NKScriptExport

        internal static Task attachToContext(NKScriptContext context, Dictionary<string, object> options)
        {
            return context.NKloadPlugin(current, null, options);
        }

        private static string defaultNamespace { get { return "io.nodekit.platform.filesystem"; } }

        private static string rewriteGeneratedStub(string stub, string forKey)
        {
            switch (forKey)
            {
                case ".global":
                    var appjs = NKStorage.getResource(typeof(NKC_FileSystem), "fs.js", "lib_core/platform");
                    return "function loadplugin(){\n" + appjs + "\n}\n" + stub + "\n" + "loadplugin();" + "\n";
                default:
                    return stub;
            }
        }
        #endregion
 
        public Task<Dictionary<string, object>> stat(string path)
        {
            Dictionary<string, object> storageItem = new Dictionary<string, object>();

            FileSystemInfo fsi;

            if (Directory.Exists(path))
            {
                fsi = new DirectoryInfo(path);
                storageItem["size"] = 0;
                storageItem["filetype"] = "Directory";
            }
            else if (File.Exists(path))
            {
                var fi = new FileInfo(path);
                fsi = fi;
                storageItem["size"] = fi.Length;
                storageItem["filetype"] = "File";
            }
            else
            {
                return Task.FromResult(storageItem);
            }

            storageItem["birthtime"] = fsi.CreationTime;
            storageItem["path"] = fsi.FullName;
            storageItem["mtime"] = fsi.LastWriteTime;

            return Task.FromResult(storageItem);
        }

        public Task<bool> exists(string path)
        {
            return Task.FromResult(File.Exists(path));
        }

        public Task<string[]> getDirectory(string path)
        {
            var dsi = new DirectoryInfo(path);
            var items = dsi.EnumerateFileSystemInfos();
            return Task.FromResult(items.Select(t => t.Name).ToArray());
        }

        public string getTempDirectorySync()
        {
            return Path.GetTempPath(); 
        }

        public async Task<string> getContent(Dictionary<string, object> storageItem)
        {
            var path = storageItem["path"] as string;
            string source;

            using (StreamReader streamReader = new StreamReader(path))
            {
                source = await streamReader.ReadToEndAsync();
            }

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(source);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public async Task<bool> writeContent(Dictionary<string, object> storageItem, string contents)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(contents);
                var plainText = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                var path = storageItem["path"] as string;

                using (StreamWriter streamWriter = new StreamWriter(path))
                {
                    await streamWriter.WriteAsync(plainText);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<string> getSource(string module)
        {
            var folder = System.IO.Path.GetDirectoryName(module);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(module);
            var fileExtension = System.IO.Path.GetExtension(module);
            if (fileExtension == String.Empty)
                fileExtension = ".js";

            var source = await NKStorage.getResourceAsync(Main.entryType, typeof(Main), fileName + fileExtension, folder);

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(source);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public Task<bool> mkdir(string path)
        {
            try
            {
                var folder = new DirectoryInfo(System.IO.Path.GetDirectoryName(path));
                folder.CreateSubdirectory(System.IO.Path.GetFileName(path));
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> rmdir(string path)
        {
            try
            {
                var folder = new DirectoryInfo(path);
                folder.Delete();
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> move(string path, string path2)
        {
            try
            {
                File.Move(path, path2);
                return Task.FromResult(true);

            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> unlink(string path)
        {
            try
            {
                var file = new FileInfo(path);
                file.Delete();
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public string getFullPathSync(string parentModule, string module)
        {
            var folder = System.IO.Path.GetDirectoryName(parentModule);
            return System.IO.Path.Combine(folder, module);
        }
    }
}
#endif