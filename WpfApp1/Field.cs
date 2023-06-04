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

        public bool canInsert(int row, int col, Figure figure)
        {

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    int x = row + i - 1;
                    int y = col + j - 1;
                    Cell cell = figure.FigureArea[i, j];
                    State state = cell.State;
                    if (state == State.Empty) continue;
                    if (x < 0 || y < 0) return false;
                    if (x >= Rows || y >= Columns) return false;
                    if (StateUtils.canConnect(state, _area[x, y].State) == false &&
                        StateUtils.canConnect(_area[x, y].State, state) == false)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public void clear(bool withHoles = false)
        {
            for (int i = 0; i < _area.GetLength(0); i++)
            {
                for (int j = 0; j < _area.GetLength(1); j++)
                {
                    if (!withHoles && _area[i, j].State != State.Empty 
                        && _area[i, j].State != State.Hole)
                    {
                        _area[i, j].State = State.Empty;
                    }
                    else if (_area[i, j].State != State.Empty)
                    {
                        _area[i, j].State = State.Empty;
                    }
                }
            }
        }

        public bool isCovered()
        {
            for (int i = 0; i < _area.GetLength(0); i++)
            {
                for (int j = 0; j < _area.GetLength(1); j++)
                {
                    if (_area[i, j].State == State.Empty) return false;
                }
            }
            return true;
        }
    }
}

