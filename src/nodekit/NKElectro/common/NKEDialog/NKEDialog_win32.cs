﻿#if WINDOWS_WIN32_WPF
/*
* nodekit.io
*
* Copyright (c) 2016 OffGrid Networks. All Rights Reserved.
* Portions Copyright (c) 2013 GitHub, Inc. under MIT License
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

using System;
using System.Collections.Generic;
using io.nodekit.NKScripting;
using System.Windows;
using System.Threading.Tasks;

namespace io.nodekit.NKElectro
{
    public sealed class NKE_Dialog
    {
        private NKEventEmitter events = NKEventEmitter.global;

#region 
        internal static Task attachToContext(NKScriptContext context, Dictionary<string, object> options)
        {
            return context.NKloadPlugin(new NKE_Dialog(), null, options);
        }

        private static string defaultNamespace { get { return "io.nodekit.electro.dialog"; } }
#endregion

       public void showOpenDialog(NKE_BrowserWindow browserWindow, Dictionary<string, object> options, NKScriptValue callback)
        {
            throw new NotImplementedException();
        }

        public void showSaveDialog(NKE_BrowserWindow browserWindow, Dictionary<string, object> options, NKScriptValue callback)
        {
            throw new NotImplementedException();
        }

        public void showMessageBox(NKE_BrowserWindow browserWindow, Dictionary<string, object> options, NKScriptValue callback)
        {
            string caption = NKOptions.itemOrDefault(options, "title", "");
            string message = NKOptions.itemOrDefault(options, "message", "");
            string [] buttonArray = NKOptions.itemOrDefault(options, "buttons", new string[] { "OK" });
            string detail = NKOptions.itemOrDefault(options, "detail", "");

            MessageBoxImage icon;
            switch (detail)
            {
                case "info":
                    icon = MessageBoxImage.Information;
                    break;
                case "warning":
                    icon = MessageBoxImage.Warning;
                    break;
                case "error":
                    icon = MessageBoxImage.Error;
                    break;
                default:
                    icon = MessageBoxImage.None;
                    break;
            }
            
            MessageBoxButton buttons = buttons = MessageBoxButton.OK;

            if ((Array.IndexOf(buttonArray, "OK") > -1) && (Array.IndexOf(buttonArray, "Cancel") > -1))
                buttons = MessageBoxButton.OKCancel;
            else if (Array.IndexOf(buttonArray, "OK") > -1)
                   buttons = MessageBoxButton.OK;
              else if ((Array.IndexOf(buttonArray, "Yes") > -1) && (Array.IndexOf(buttonArray, "No") > -1) && (Array.IndexOf(buttonArray, "Cancel") > -1))
                buttons = MessageBoxButton.YesNoCancel;
            else if ((Array.IndexOf(buttonArray, "Yes") > -1) && (Array.IndexOf(buttonArray, "No") > -1))
                buttons = MessageBoxButton.YesNo;
      
            MessageBoxResult result = MessageBox.Show(message, caption, buttons, icon);
            if (callback != null)
                 callback.callWithArguments(new object[] { result.ToString() });
        }

        public void showErrorBox(string title, string content)
        {
            showMessageBox(null, new Dictionary<string, object> { ["title"] = title, ["message"] = content, ["detail"] = "error" }, null);
        }
    }
}



#endif