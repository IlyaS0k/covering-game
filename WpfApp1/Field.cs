using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Field : IEnumerable<Cell>
    {

        private Cell[,] _area;

        public Field(int r, int c)
        {
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
    }
}

