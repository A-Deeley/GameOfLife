using JeuDeLaVie.ViewModel;
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

namespace JeuDeLaVie.View
{
    /// <summary>
    /// Interaction logic for CanvasPicker.xaml
    /// </summary>
    public partial class CanvasPicker : Window
    {
        public CanvasPicker()
        {
            InitializeComponent();
            DataContext = new VM_CreateCanvas(this);
            Closed += (s, e) => DataContext = null;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textbox = e.OriginalSource as TextBox;
            if (textbox is not null)
                textbox.SelectAll();
        }
    }
}
