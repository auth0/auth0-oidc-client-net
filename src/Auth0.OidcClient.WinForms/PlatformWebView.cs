// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.OidcClient.Browser;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auth0.OidcClient
{
    public class PlatformWebView : IBrowser
    {
        private readonly Func<Form> _formFactory;

        public PlatformWebView(Func<Form> formFactory)
        {
            _formFactory = formFactory;
        }

        public PlatformWebView(string title = "Authenticating ...", int width = 1024, int height = 768)
            : this(() => new Form
            {
                Name = "WebAuthentication",
                Text = title,
                Width = width,
                Height = height
            })
        { }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options)
        {
            using (var form = _formFactory.Invoke())
            using (var browser = new ExtendedWebBrowser()
            {
                Dock = DockStyle.Fill
            })
            {
                var signal = new SemaphoreSlim(0, 1);

                var result = new BrowserResult
                {
                    ResultType = BrowserResultType.UserCancel
                };

                form.FormClosed += (o, e) =>
                {
                    signal.Release();
                };

                browser.NavigateError += (o, e) =>
                {
                    e.Cancel = true;
                    result.ResultType = BrowserResultType.HttpError;
                    result.Error = e.StatusCode.ToString();
                    signal.Release();
                };

                browser.DocumentCompleted += (o, e) =>
                {
                    if (e.Url.AbsoluteUri.StartsWith(options.EndUrl))
                    {
                        result.ResultType = BrowserResultType.Success;
                        result.Response = e.Url.ToString();
                        signal.Release();
                    }
                };

                form.Controls.Add(browser);
                browser.Show();

                form.Show();
                browser.Navigate(options.StartUrl);

                await signal.WaitAsync();

                form.Hide();
                browser.Hide();

                return result;
            }
        }
    }
}