// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Auth0.OidcClient
{
    internal class ExtendedWebBrowser : WebBrowser
    {
        private AxHost.ConnectionPointCookie _cookie;
        private ExtendedWebBrowserEventHelper _helper;

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void CreateSink()
        {
            base.CreateSink();

            _helper = new ExtendedWebBrowserEventHelper(this);
            _cookie = new AxHost.ConnectionPointCookie(ActiveXInstance, _helper, typeof(DWebBrowserEvents2));
        }

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void DetachSink()
        {
            if (_cookie != null)
            {
                _cookie.Disconnect();
                _cookie = null;
            }
            base.DetachSink();
        }

        internal event EventHandler<NavigateErrorEventArgs> NavigateError;

        private void OnNavigateError(object pDisp, ref object url, ref object frame, ref object statusCode, ref bool cancel)
        {
            var handler = NavigateError;
            if (handler != null)
            {
                var args = new NavigateErrorEventArgs((string)url, (string)frame, (int)statusCode);
                handler(this, args);
                cancel = args.Cancel;
            }
        }

        internal class NavigateErrorEventArgs : EventArgs
        {
            public NavigateErrorEventArgs(string url, string frame, int statusCode)
            {
                Url = url;
                Frame = frame;
                StatusCode = statusCode;
                Cancel = false;
            }

            public string Url { get; }
            public string Frame { get; }
            public int StatusCode { get; }
            public bool Cancel { get; set; }
        }

        [ComImport]
        [Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        [TypeLibType(TypeLibTypeFlags.FHidden)]
        private interface DWebBrowserEvents2
        {
            [PreserveSig]
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            [DispId(271)]
            void NavigateError([In] [MarshalAs(UnmanagedType.IDispatch)] object pDisp,
                            [In] [MarshalAs(UnmanagedType.Struct)] ref object URL,
                            [In] [MarshalAs(UnmanagedType.Struct)] ref object Frame,
                            [In] [MarshalAs(UnmanagedType.Struct)] ref object StatusCode,
                            [In] [Out] ref bool Cancel);
        }


        private class ExtendedWebBrowserEventHelper : StandardOleMarshalObject, DWebBrowserEvents2
        {
            readonly ExtendedWebBrowser parent;

            public ExtendedWebBrowserEventHelper(ExtendedWebBrowser parent)
            {
                this.parent = parent;
            }

            public void NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
            {
                parent.OnNavigateError(pDisp, ref URL, ref Frame, ref StatusCode, ref Cancel);
            }
        }
    }
}
