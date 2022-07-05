using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public enum State
    {
        Empty,//
        Hole,//
        LeftUpRing,//
        LeftRightRing,//
        LeftDownRing,//
        RightUpRing,//
        RightDownRing,//
        LeftBall,//
        RightBall,//
        UpBall,//
        DownBall,//
        LeftHalfRing,//
        RightHalfRing,//
        UpHalfRing,//
        DownHalfRing,//
        LeftRightCompound,//
        UpDownCompound,//
        UpDownRing,//
        LeftDownCompound,//
        LeftUpCompound,//
        RightDownCompound,//
        RightUpCompound,//
        DownHalfRingL,
        DownHalfRingR,
        UpHalfRingL,
        UpHalfRingR,
        LeftHalfRingL,
        LeftHalfRingR,
        RightHalfRingL,
        RightHalfRingR,
    }
    class Cell : INotifyPropertyChanged
    {

        private State _state;
        private int _row;
        private int _column;
        private bool _active; 
        public Cell(int i, int j)
        {
            this.State = State.Empty;
            this.Active = false;
            this.Row = i;
            this.Column = j;
        }
        public State State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                OnPropertyChanged();
            }
        }

        public int Row
        {
            get { return _row; }
            set
            {
                _row = value;
                OnPropertyChanged("Row");
            }
        }
        public int Column
        {
            get { return _column; }
            set
            {
                _column = value;
                OnPropertyChanged("Column");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
