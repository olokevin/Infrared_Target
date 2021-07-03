using MahApps.Metro.Controls;
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

namespace InfraredRayTarget
{
    public partial class ConfirmWindow : MetroWindow
    {
        private string m_title;
        private string m_text;
        private Delegate m_callback;
        private bool m_isEdit;

        public ConfirmWindow(string title, string text, Delegate callback, bool isEdit = true)
        {
            InitializeComponent();
            m_title = title;
            m_text = text;
            m_callback = callback;
            m_isEdit = isEdit;
            ShowDialog();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Title = m_title;
            valueBox.Text = m_text;
            if (m_isEdit)
            {
                valueBox.Focus();
                valueBox.SelectAll();
            }
            else
            {
                valueBox.IsReadOnly = true;
            }
        }

        private void OnConfirm(object sender, RoutedEventArgs e)
        {
            if (m_callback != null)
            {
                m_callback.DynamicInvoke(valueBox.Text);
                Close();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    m_callback.DynamicInvoke(valueBox.Text);
                    Close();
                    break;
                case Key.Escape:
                    Close();
                    break;
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
