namespace CefSharp.MinimalExample.Wpf
{
    using System;
    using System.IO;
    using System.Linq;

    using CefSharp.Wpf;

    public partial class App
    {
        #region constructors and destructors

        public App()
        {
            var settings = new CefSettings
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            //Example of setting a command line argument
            //Enables WebRTC
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            //Perform dependency check to make sure all relevant resources are in our output directory.
            Cef.Initialize(settings, true, browserProcessHandler: null);
        }

        #endregion
    }
}