﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft;

namespace InlineLocals
{
    // If we want more space between the end of the line and the watch button,
    // we can use a StackPanel with vertical orientation for our content and add
    // a first child that is invisible, which will function as padding.
    //
    // Maybe it'd be a good idea to give each variable its own control element, so for example
    // we wouldn't have "x: 1   y: 2" as one button, but as two separate ones. That way
    // we could add different behavior based on the represented data type, for example
    // when hovering. The default view for a vector could be "vi: {size=5}", but when you
    // hover the button, it changes to "vi: {1, 2, 3, 4, 5}".
    class WatchAdornment : ContentControl
    {
        internal WatchAdornment(WatchTag watchTag) {
            //this.Padding = new Thickness(-0.15);

            double outFontSize = 0;
            if (TryGetFontSize(ref outFontSize)) {
                this.FontSize = outFontSize + 2;
            }

            Update(watchTag);
        }

        protected override void OnMouseEnter(MouseEventArgs e) {
            base.OnMouseEnter(e);
            // TODO: Use Command (find in view->other windows->command explorer) to execute vsix pinned inline local window
        }

        private Brush MakeBrush(Color color) {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        internal void Update(WatchTag watchTag) {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            TranslateTransform tt = new TranslateTransform(20.0, 0.0);
            stackPanel.RenderTransform = tt;
            foreach (var s in watchTag.Locals) {
                TextBox textBox = CreateTextBox(s);
                stackPanel.Children.Add(textBox);
            }
            this.Content = stackPanel;
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnPreviewMouseLeftButtonUp(e);
            e.Handled = false;
        }

        private TextBox CreateTextBox(KeyValuePair<string,string> local) {
            TextBox textBox = new TextBox();
            Color backgroundColor = Colors.DarkGray;
            backgroundColor.ScA = 0.0F;
            textBox.Background = MakeBrush(backgroundColor);

            textBox.Foreground = MakeBrush(Colors.LightGray);

            Color borderColor = Colors.DarkGray;
            borderColor.ScA = 0.2F;
            textBox.BorderBrush = MakeBrush(borderColor);
            textBox.FontStyle = FontStyles.Italic;
            textBox.IsReadOnly = true;
            textBox.IsReadOnlyCaretVisible = false;

            textBox.Tag = local;
            textBox.Cursor = Cursors.Hand;
            textBox.Text = " " + local.Key + ": " + local.Value + " ";

            textBox.PreviewMouseLeftButtonUp += HandleTextBoxMouseLeftButtonUp;

            return textBox;
        }
        private void HandleTextBoxMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            TextBox textBox = sender as TextBox;
            if(textBox is null) {
                return;
            }

            EnvDTE.DTE dte = (EnvDTE.DTE)Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
            if(dte is null) {
                return;
            }

            KeyValuePair<string, string> local = (KeyValuePair<string, string>) textBox.Tag;
            dte.ExecuteCommand("Debug.AddWatch " + local.Key);
        }

        bool TryGetFontSize(ref double size) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            EnvDTE.DTE DTE = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;
            if (DTE is null)
                return false;
            EnvDTE.Properties propertiesList = DTE.get_Properties("FontsAndColors", "TextEditor");
            if (propertiesList is null)
                return false;
            Property prop = propertiesList.Item("FontSize");
            if (prop is null)
                return false;
            int fontSize = (System.Int16)prop.Value;
            size = (double)fontSize;
            return true;
        }
    }
}
