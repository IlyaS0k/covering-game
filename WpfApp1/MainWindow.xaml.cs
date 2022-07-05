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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly MainView mainView;
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i <= 100; ++i)
            {
                if (i >= 2 && i <= 15)
                {
                    RowsCount.Items.Add(i.ToString());
                    ColumnsCount.Items.Add(i.ToString());
                }
                HolesCount.Items.Add(i.ToString());
            }
            mainView = new MainView();
            this.DataContext = mainView;
            
        }
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            
            int rows = int.Parse(RowsCount.SelectedItem.ToString());
            int cols = int.Parse(ColumnsCount.SelectedItem.ToString());
            int holes = int.Parse(HolesCount.SelectedItem.ToString());
            mainView.PrevField = null;
            if (rows * cols > 2 * holes)
            {
                mainView.Rows = rows;
                mainView.Columns = cols;
                mainView.Field = new Field(mainView.Rows, mainView.Columns);
                mainView.IsCovered = false;
                mainView.GenerateHoles(holes);
                mainView.SetFigures();
            }
            else 
                MessageBox.Show("Дырок не должно быть так много!");
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            mainView.Field = mainView.PrevField;
            mainView.PrevField = null;
        }
    }
}
