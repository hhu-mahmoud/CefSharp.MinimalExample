// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

namespace CefSharp.MinimalExample.WinForms
{
    using System;
    using System.Linq;
    using System.Windows.Forms;

    using CefSharp.WinForms;

    using Controls;

    using Properties;

    public partial class BrowserForm : Form
    {
        #region member vars

        private readonly ChromiumWebBrowser _browser;

        #endregion

        #region constructors and destructors

        public BrowserForm()
        {
            InitializeComponent();
            Text = "CefSharp";
            WindowState = FormWindowState.Maximized;
            _browser = new ChromiumWebBrowser("www.google.com");
            toolStripContainer.ContentPanel.Controls.Add(_browser);
            _browser.IsBrowserInitializedChanged += OnIsBrowserInitializedChanged;
            _browser.LoadingStateChanged += OnLoadingStateChanged;
            _browser.ConsoleMessage += OnBrowserConsoleMessage;
            _browser.StatusMessage += OnBrowserStatusMessage;
            _browser.TitleChanged += OnBrowserTitleChanged;
            _browser.AddressChanged += OnBrowserAddressChanged;
            var version = $"Chromium: {Cef.ChromiumVersion}, CEF: {Cef.CefVersion}, CefSharp: {Cef.CefSharpVersion}";
#if NETCOREAPP
            // .NET Core
            var environment = string.Format("Environment: {0}, Runtime: {1}",
                System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(),
                System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
#else
            // .NET Framework
            var bitness = Environment.Is64BitProcess ? "x64" : "x86";
            var environment = $"Environment: {bitness}";
#endif
            DisplayOutput($"{version}, {environment}");
        }

        #endregion

        #region methods

        private void BackButtonClick(object sender, EventArgs e)
        {
            _browser.Back();
        }

        private void DisplayOutput(string output)
        {
            this.InvokeOnUiThreadIfRequired(() => outputLabel.Text = output);
        }

        private void ExitMenuItemClick(object sender, EventArgs e)
        {
            _browser.Dispose();
            Cef.Shutdown();
            Close();
        }

        private void ForwardButtonClick(object sender, EventArgs e)
        {
            _browser.Forward();
        }

        private void GoButtonClick(object sender, EventArgs e)
        {
            LoadUrl(urlTextBox.Text);
        }

        private void HandleToolStripLayout(object sender, LayoutEventArgs e)
        {
            HandleToolStripLayout();
        }

        private void HandleToolStripLayout()
        {
            var width = toolStrip1.Items.Cast<ToolStripItem>().Where(item => item != urlTextBox).Aggregate(toolStrip1.Width, (current, item) => current - (item.Width - item.Margin.Horizontal));
            urlTextBox.Width = Math.Max(0, width - urlTextBox.Margin.Horizontal - 18);
        }

        private void LoadUrl(string url)
        {
            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                _browser.Load(url);
            }
        }

        private void OnBrowserAddressChanged(object sender, AddressChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => urlTextBox.Text = args.Address);
        }

        private void OnBrowserConsoleMessage(object sender, ConsoleMessageEventArgs args)
        {
            DisplayOutput($"Line: {args.Line}, Source: {args.Source}, Message: {args.Message}");
        }

        private void OnBrowserStatusMessage(object sender, StatusMessageEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => statusLabel.Text = args.Value);
        }

        private void OnBrowserTitleChanged(object sender, TitleChangedEventArgs args)
        {
            this.InvokeOnUiThreadIfRequired(() => Text = args.Title);
        }

        private void OnIsBrowserInitializedChanged(object sender, EventArgs e)
        {
            var b = (ChromiumWebBrowser)sender;
            this.InvokeOnUiThreadIfRequired(() => b.Focus());
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            SetCanGoBack(args.CanGoBack);
            SetCanGoForward(args.CanGoForward);
            this.InvokeOnUiThreadIfRequired(() => SetIsLoading(!args.CanReload));
        }

        private void SetCanGoBack(bool canGoBack)
        {
            this.InvokeOnUiThreadIfRequired(() => backButton.Enabled = canGoBack);
        }

        private void SetCanGoForward(bool canGoForward)
        {
            this.InvokeOnUiThreadIfRequired(() => forwardButton.Enabled = canGoForward);
        }

        private void SetIsLoading(bool isLoading)
        {
            goButton.Text = isLoading ? "Stop" : "Go";
            goButton.Image = isLoading ? Resources.nav_plain_red : Resources.nav_plain_green;
            HandleToolStripLayout();
        }

        private void ShowDevToolsMenuItemClick(object sender, EventArgs e)
        {
            _browser.ShowDevTools();
        }

        private void UrlTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            LoadUrl(urlTextBox.Text);
        }

        #endregion

        #region properties

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        #endregion
    }
}