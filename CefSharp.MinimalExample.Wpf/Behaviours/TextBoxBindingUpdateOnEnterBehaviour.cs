namespace CefSharp.MinimalExample.Wpf.Behaviours
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    public class TextBoxBindingUpdateOnEnterBehaviour : Behavior<TextBox>
    {
        #region methods

        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += OnTextBoxKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= OnTextBoxKeyDown;
        }

        private static void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }
            var txtBox = sender as TextBox;
            txtBox?.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
        }

        #endregion
    }
}