// <copyright file="IosPlatformConfiguration.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))

namespace GooglePlayGames.Native.PInvoke
{
    using System;
    using System.Runtime.InteropServices;
    using GooglePlayGames.OurUtils;
    using C = GooglePlayGames.Native.Cwrapper.IosPlatformConfiguration;

    sealed class IosPlatformConfiguration : PlatformConfiguration
    {

        private IosPlatformConfiguration(IntPtr selfPointer)
            : base(selfPointer)
        {
        }

        internal void SetClientId(string clientId)
        {
            bubble.CheckNotNull(clientId);

            C.IosPlatformConfiguration_SetClientID(SelfPtr(), clientId);
        }

        protected override void CallDispose(HandleRef selfPointer)
        {
            C.IosPlatformConfiguration_Dispose(selfPointer);
        }

        internal static IosPlatformConfiguration Create()
        {
            return new IosPlatformConfiguration(C.IosPlatformConfiguration_Construct());
        }
    }
}


#endif
