using AJWPFAdmin.Core.Components.AJTable.ViewModels;
using AJWPFAdmin.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AJWPFAdmin.Core.AssistProperties
{
    public class TextFieldAssist
    {
        public static readonly DependencyProperty OnlyNumberProperty =
            DependencyProperty.RegisterAttached("OnlyNumber", typeof(bool), typeof(TextBox),
                new PropertyMetadata(false, OnOnlyNumberPropertyChanged));

        public static bool GetOnlyNumber(DependencyObject obj)
        {
            return (bool)obj.GetValue(OnlyNumberProperty);
        }

        public static void SetOnlyNumber(DependencyObject obj, bool value)
        {
            obj.SetValue(OnlyNumberProperty, value);
        }

        public static void OnOnlyNumberPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var hit = !(bool)e.NewValue;
                textBox.SetValue(InputMethod.IsInputMethodEnabledProperty, hit);
                textBox.PreviewTextInput -= TextInput;
                if (!hit)
                {
                    textBox.PreviewTextInput += TextInput;
                }
            }
        }

        private static void TextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !CommonRegex.INTEGERSTRING.IsMatch(e.Text);

        }
    }
}
