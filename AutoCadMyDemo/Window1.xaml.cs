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

namespace AutoCadMyDemo
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {

        private int flag;

        public int Flag{
            get { return flag; }
            set { flag = value; }         
        }
    
        public Window1()
        {
            InitializeComponent();
        }

        private void B1_Click(object sender, RoutedEventArgs e)
        {
            flag = 1;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            flag = 2;
            this.Close();
        }
    }
}
