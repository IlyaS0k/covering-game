using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Figure 
    {
        private Field _figureArea;
        private int _index;
        public Figure()
        {
            _figureArea = new Field(4, 3);
        }
        
        public Field FigureArea
        {
            get => _figureArea;
            set
            {
                _figureArea = value;
                OnPropertyChanged();
            }
        }

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
