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
            mainView = new MainView();
            this.DataContext = mainView;
            
        }
       
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            string rules = "Правила игры: \n" +
                           "Имеется прямоугольное поле размерности 𝑚×𝑛, разделённое на равные клетки(прямоугольная решётка). Какие - то\n" +
                           "из этих клеток могут быть «выколоты» («дырки»). Также имеется 𝑘 неразъёмных трёхсекционных фигур прямоугольной либо угловой конфигурации.\n" +
                           "Средняя секция каждой такой фигуры представляет собой кольцо, любая из крайних секций – либо шарик, либо\n" +
                           "полукольцо(кольцо с прорезью по одному из трёх возможных направлений).\n" +
                           "Фигуры раскладываются по полю.При этом шарик и полукольцо могут занять одну клетку, если совпадут по\n" +
                           "направлению(если шарик можно вставить в полукольцо таким образом, чтобы крепление шарика к средней секции\n" +
                           "попало в прорезь полукольца).\n" +
                           "Требуется замостить поле фигурами из данного набора так, чтобы в каждой клетке поля присутствовал хотя бы\n" +
                           "один элемент фигуры, чтобы фигуры лежали в один слой и чтобы каждая фигура целиком находилась на поле.\n\n" +
                           "Для того чтобы начать игру:\n" +
                           "Выберите размеры поля и количество дырок в соответствующих полях и нажмите кнопку 'Создать'.\n" +
                           "Справа появится список всех возможных фигур.У под каждой фигуры есть 2 кнопки:\n" +
                           "Розовая осуществляет поворот соответствующей ей фигуры на 90 градусов по часовой стрелке,\n" +
                           "Зеленая вставляет(если это возможно) фигуру в выбранную клетку на доске.\n" +
                           "Всегда можно начать новую игру выбрав новые параметры поля и нажав кнопку 'Создать'.\n";
          MessageBox.Show(rules);
        }
    }
}
