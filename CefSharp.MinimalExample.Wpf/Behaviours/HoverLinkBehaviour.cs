﻿namespace CefSharp.MinimalExample.Wpf.Behaviours
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Interactivity;

    using CefSharp.Wpf;

    public class HoverLinkBehaviour : Behavior<ChromiumWebBrowser>
    {
        #region constants

        // Using a DependencyProperty as the backing store for HoverLink. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverLinkProperty = DependencyProperty.Register("HoverLink", typeof(string), typeof(HoverLinkBehaviour), new PropertyMetadata(string.Empty));

        #endregion

        #region methods

        protected override void OnAttached()
        {
            AssociatedObject.StatusMessage += OnStatusMessageChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.StatusMessage -= OnStatusMessageChanged;
        }

        private void OnStatusMessageChanged(object sender, StatusMessageEventArgs e)
        {
            var chromiumWebBrowser = sender as ChromiumWebBrowser;
            chromiumWebBrowser?.Dispatcher?.BeginInvoke((Action)(() => HoverLink = e.Value));
        }

        #endregion

        #region properties

        public string HoverLink
        {
            get => (string)GetValue(HoverLinkProperty);
            set => SetValue(HoverLinkProperty, value);
        }

        #endregion
    }
}