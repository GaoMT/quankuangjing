using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SHH.TF.UI
{
    /// <summary>
    /// FormGroup.xaml 的交互逻辑
    /// </summary>
    public partial class FormGroup : Window
    {
        public String Name { set; get; }
        public int UpdateRate { set; get; }
        public float DeadBend { set; get; }
        public int TimeBias { set; get; }
        public bool IsActive { set; get; }
        public bool IsSubscribed { set; get; }


        public FormGroup()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Name = tbx_Name.Text;
            UpdateRate = Int32.Parse(tbx_UpdateRate.Text);
            DeadBend = float.Parse(tbx_DeadBend.Text);
            TimeBias = Int32.Parse(tbx_TimeBias.Text);
            IsActive = cbx_IsActive.IsChecked.Value;
            IsSubscribed = cbx_IsSubscribed.IsChecked.Value;


            this.DialogResult = true;
        }
    }
}
