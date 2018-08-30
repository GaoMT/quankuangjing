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
    /// FormPoint.xaml 的交互逻辑
    /// </summary>
    public partial class FormPoint : Window
    {
        public String PointName { set; get; }

        public String Id { set; get; }

        public String Place { set; get; }

        public int EquipID { set; get; }

        public FormPoint()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbx_EquipID.SelectedIndex == -1)
            {
                cbx_EquipID.SelectedIndex = 0;
            }

            PointName = tbx_Name.Text;
            Id = tbx_Id.Text;
            Place = tbx_Place.Text;
            EquipID = cbx_EquipID.SelectedIndex;

            this.DialogResult = true;
        }
    }
}
