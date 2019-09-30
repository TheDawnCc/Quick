using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Froser.Quick.UI
{
    /// <summary>
    /// QuickContextWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QuickContextWindow : Window
    {
        public QuickContextWindow(IQuickContextWindowHandler handler)
        {
            InitializeComponent();

            handler.SetHost(this);
            m_handler = handler;
            this.Deactivated += handler.OnDeactivate;
            handler.Init();
            this.DataContext = handler;
        }

        public void Show(string context)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            m_handler.BeforeShow(context);
            Show();
            m_handler.AfterShow();
        }

        public void SelectNext()
        {
            var list = GetList();
            int nextIndex = list.SelectedIndex + 1;
            if (nextIndex >= list.Items.Count)
            {
                list.SelectedIndex = 0;
            }
            else
            {
                list.SelectedIndex = nextIndex;
            }
        }

        public void SelectPrevious()
        {
            var list = GetList();
            int nextIndex = list.SelectedIndex - 1;
            if (nextIndex < 0)
            {
                list.SelectedIndex = list.Items.Count - 1;
            }
            else
            {
                list.SelectedIndex = nextIndex;
            }
        }

        public void Select(int index)
        {
            var list = GetList();
            list.SelectedIndex = index;
        }

        public QuickListBox GetList()
        {
            return quickContextList;
        }

        private IQuickContextWindowHandler m_handler;
    }
}
