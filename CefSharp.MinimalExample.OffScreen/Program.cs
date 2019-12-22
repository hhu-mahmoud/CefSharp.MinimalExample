// Copyright © 2010-2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

namespace CefSharp.MinimalExample.OffScreen
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CefSharp.OffScreen;

    public static class Program
    {
        #region constants

        private static ChromiumWebBrowser _browser;

        #endregion

        #region methods

        public static void Main(string[] args)
        {
            const string TestUrl = "https://www.google.com/";
            Console.WriteLine("This example application will load {0}, take a screenshot, and save it to your desktop.", TestUrl);
            Console.WriteLine("You may see Chromium debugging output, please wait...");
            Console.WriteLine();
            var settings = new CefSettings
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, true, browserProcessHandler: null);

            // Create the offscreen Chromium browser.
            _browser = new ChromiumWebBrowser(TestUrl);

            // An event that is fired when the first page is finished loading.
            // This returns to us from another thread.
            _browser.LoadingStateChanged += BrowserLoadingStateChanged;

            // We have to wait for something, otherwise the process will exit too soon.
            Console.ReadKey();

            // Clean up Chromium objects.  You need to call this in your application otherwise
            // you will get a crash when closing.
            Cef.Shutdown();
        }

        private static void BrowserLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            // Check to see if loading is complete - this event is called twice, one when loading starts
            // second time when it's finished
            // (rather than an iframe within the main frame).
            if (e.IsLoading)
            {
                return;
            }
            // Remove the load event handler, because we only want one snapshot of the initial page.
            _browser.LoadingStateChanged -= BrowserLoadingStateChanged;
            var scriptTask = _browser.EvaluateScriptAsync("document.getElementById('lst-ib').value = 'CefSharp Was Here!'");
            scriptTask.ContinueWith(
                t =>
                {
                    //Give the browser a little time to render
                    Thread.Sleep(500);
                    // Wait for the screenshot to be taken.
                    var task = _browser.ScreenshotAsync();
                    task.ContinueWith(
                        x =>
                        {
                            // Make a file to save it to (e.g. C:\Users\jan\Desktop\CefSharp screenshot.png)
                            var screenshotsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CefSharp screenshot.png");
                            Console.WriteLine();
                            Console.WriteLine("Screenshot ready. Saving to {0}", screenshotsPath);

                            // Save the Bitmap to the path.
                            // The image type is auto-detected via the ".png" extension.
                            task.Result.Save(screenshotsPath);

                            // We no longer need the Bitmap.
                            // Dispose it to avoid keeping the memory alive.  Especially important in 32-bit applications.
                            task.Result.Dispose();
                            Console.WriteLine("Screenshot saved.  Launching your default image viewer...");

                            // Tell Windows to launch the saved image.
                            Process.Start(
                                new ProcessStartInfo(screenshotsPath)
                                {
                                    // UseShellExecute is false by default on .NET Core.
                                    UseShellExecute = true
                                });
                            Console.WriteLine("Image viewer launched.  Press any key to exit.");
                        },
                        TaskScheduler.Default);
                });
        }

        #endregion
    }
}