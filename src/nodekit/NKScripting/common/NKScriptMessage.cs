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

namespace io.nodekit.NKScripting
{
    public class NKScriptMessage
    {
        public object body;
        public string name;

        public NKScriptMessage(string name, object body)
        {
            this.body = body;
            this.name = name;
        }
    }

    public interface NKScriptMessageHandler
    {
        void didReceiveScriptMessage(NKScriptMessage message);
        object didReceiveScriptMessageSync(NKScriptMessage message);
    }


}