using MaterialDesignExtensions.Controls;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AJWPFAdmin.Modules.Common.Views
{
    /// <summary>
    /// DialogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DialogWindow : MaterialWindow, IDialogWindow
    {
        public DialogWindow()
        {
            InitializeComponent();

            UseLayoutRounding = true;

            ContentRendered += (_, _) =>
            {
                if (SizeToContent != SizeToContent.Manual)
                {
                    // force a SizeToContent change so that WPF/MDE can correctly layout the window to fit the content
                    var sizeToContent = SizeToContent;
                    SizeToContent = SizeToContent.Manual;
                    SizeToContent = sizeToContent;
                }
            };
        }

        public IDialogResult Result { get; set; }
    }
}
