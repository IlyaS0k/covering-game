using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

namespace WpfApp1
{
    class Field : INotifyPropertyChanged, IEnumerable<Cell>
    {

        private Cell[,] _area;
        private int _rows;
        private int _columns;

         public int Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                OnPropertyChanged();
            }
        }
        public int Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                OnPropertyChanged();
            }
        }

        public Field(int r, int c)
        {
            Rows = r;
            Columns = c;
            _area = new Cell[r, c];
            for (int i = 0; i < _area.GetLength(0); i++)
                for (int j = 0; j < _area.GetLength(1); j++)
                    _area[i, j] = new Cell(i, j);
            
        }

        public Field()
        { 

        }
        public Cell this[int row, int column]
        {
            get => _area[row, column];
            set => _area[row, column] = value;
        }

        public IEnumerator<Cell> GetEnumerator()
            => _area.Cast<Cell>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _area.GetEnumerator();
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void clearField()
        {
            for (int i = 0; i < _area.GetLength(0); i++)
                for (int j = 0; j < _area.GetLength(1); j++)
                    if (_area[i,j].State != State.Empty && _area[i,j].State != State.Hole)
                    {
                        _area[i, j].State = State.Empty;  
                    }
        }
    }
}

