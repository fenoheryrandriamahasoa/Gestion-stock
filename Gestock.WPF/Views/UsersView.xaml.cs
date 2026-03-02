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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuperMarcheApp.Views
{
    /// <summary>
    /// Logique d'interaction pour UsersView.xaml
    /// </summary>
    public partial class UsersView : UserControl
    {
        public UsersView()
        {
            InitializeComponent();
        }

        private void MyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Code to execute when the CheckBox is checked
            //MessageBox.Show("CheckBox is checked.");
        }

        private void MyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Code to execute when the CheckBox is unchecked
            //MessageBox.Show("CheckBox is unchecked.");
        }
    }
}
