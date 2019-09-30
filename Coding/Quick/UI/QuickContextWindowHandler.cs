using Froser.Quick.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Froser.Quick.UI
{
    public class QuickContextWindowHandler : IQuickContextWindowHandler
    {
        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        /// <param name="lpPoint"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        public static extern bool GetCursorPos(ref Point lpPoint);

        public void SetHost(QuickContextWindow host)
        {
            m_host = host;
        }

        public void Init()
        {
            var list = m_host.GetList();
            foreach (var i in QuickConfig.ThisConfig.ContextMenuList)
            {
                QuickListItem item = new QuickListItem(i.Name, null, null);
                item.Tag = i;
                item.CreateListBoxItemTo(list);
            }
            list.ListItemClicked += itemClicked;


            m_host.GetList().KeyUp+= ListOnKeyUp;
        }

        /// <summary>
        /// 释放按键时触发配置事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListOnKeyUp(object sender, KeyEventArgs e)
        {
            Debug.WriteLine($"KeyUp {e.Key} {e.SystemKey} {e.KeyboardDevice.Modifiers}");

            if (e.Key == Key.Oem3)
            {
                m_host.SelectNext();
            }
            else if (e.Key == Key.LeftCtrl)
            {
                ItemFire(m_host.GetList().SelectedItem);
            }
        }

        /// <summary>
        /// 选项点击触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemClicked(object sender, EventArgs e)
        {
            ItemFire(sender);
        }

        /// <summary>
        /// 选项触发事件
        /// </summary>
        /// <param name="item"></param>
        public void ItemFire(object item)
        {
            ListBoxItem listBoxItem = item as ListBoxItem;
            QuickListItem rawItem = listBoxItem.Tag as QuickListItem;
            var menuItem = rawItem.Tag as QuickConfig.ContextMenuItem;
            string concreteCmd = menuItem.Exec.Replace(QuickConfig.ContextMenuItem.Replacement, m_context);
            Process.Start(concreteCmd, menuItem.Argument);
        }

        public void BeforeShow(string context)
        {
            m_context = context;

            Point mousePos = new Point();
            GetCursorPos(ref mousePos);
            m_host.Left = mousePos.X;
            m_host.Top = mousePos.Y;
            m_host.Activate();
        }

        public void AfterShow()
        {
            if (!m_adjustedHeight)
            {
                var listItems = m_host.GetList().Items;
                if (listItems.Count > 0)
                {
                    ListBoxItem item = (ListBoxItem)m_host.GetList().Items[0];
                    double eachHeight = item.ActualHeight;
                    const int OFFSET = 4;
                    m_host.Height = (eachHeight + OFFSET) * listItems.Count ;
                }
                m_adjustedHeight = true;
            }
        }

        public void OnDeactivate(object sender, EventArgs e)
        {
            m_host.Hide();
        }

        private QuickContextWindow m_host;
        private bool m_adjustedHeight = false;
        private string m_context;
    }
}
