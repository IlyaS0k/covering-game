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
        Empty,
        Hole,
        LeftUpRing,
        LeftRightRing,
        LeftDownRing,
        RightUpRing,
        RightDownRing,
        LeftBall,
        RightBall,
        UpBall,
        DownBall,
        LeftHalfRing,
        RightHalfRing,
        UpHalfRing,
        DownHalfRing,
        LeftRightCompound,
        UpDownCompound,
        UpDownRing,
        LeftDownCompound,
        LeftUpCompound, 
        RightDownCompound, 
        RightUpCompound, 
        DownHalfRingL,
        DownHalfRingR,
        UpHalfRingL,
        UpHalfRingR,
        LeftHalfRingL,
        LeftHalfRingR,
        RightHalfRingL,
        RightHalfRingR
    }
    public static class StateUtils
    {
        public static State rotate(this State state)
        {
            switch (state)
            {
                case State.UpBall: return State.RightBall;

                case State.LeftBall: return State.UpBall;

                case State.DownBall: return State.LeftBall;

                case State.RightBall: return State.DownBall;

                case State.UpHalfRing: return State.RightHalfRing;

                case State.LeftHalfRing: return State.UpHalfRing;

                case State.DownHalfRing: return State.LeftHalfRing;

                case State.RightHalfRing: return State.DownHalfRing;

                case State.LeftRightRing: return State.UpDownRing;

                case State.UpDownRing: return State.LeftRightRing;

                case State.LeftUpRing: return State.RightUpRing;

                case State.LeftDownRing: return State.LeftUpRing;

                case State.RightDownRing: return State.LeftDownRing;

                case State.RightUpRing: return State.RightDownRing;

                case State.DownHalfRingL: return State.LeftHalfRingL;

                case State.DownHalfRingR: return State.LeftHalfRingR;

                case State.LeftHalfRingL: return State.UpHalfRingL;

                case State.LeftHalfRingR: return State.UpHalfRingR;

                case State.RightHalfRingL: return State.DownHalfRingL;

                case State.RightHalfRingR: return State.DownHalfRingR;

                case State.UpHalfRingL: return State.RightHalfRingL;

                case State.UpHalfRingR: return State.RightHalfRingR;

                default: return state;
            }

        }
        public static bool canConnect(State state, State insertedState)
        {
            switch (insertedState)
            {
                case State.Empty:
                    {
                        return true;
                    }

                case State.DownHalfRing:
                    {
                        if (state != State.UpBall)
                            return false;
                        else
                            return true;
                    }
                case State.DownHalfRingL:
                    {
                        if (state != State.LeftBall)
                            return false;
                        else
                            return true;
                    }
                case State.DownHalfRingR:
                    {
                        if (state != State.RightBall)
                            return false;
                        else
                            return true;
                    }

                case State.LeftHalfRing:
                    {
                        if (state != State.RightBall)
                            return false;
                        else
                            return true;
                    }

                case State.LeftHalfRingL:
                    {
                        if (state != State.UpBall)
                            return false;
                        else
                            return true;
                    }
                case State.LeftHalfRingR:
                    {
                        if (state != State.DownBall)
                            return false;
                        else
                            return true;
                    }

                case State.RightHalfRing:
                    {
                        if (state != State.LeftBall)
                            return false;
                        else
                            return true;

                    }
                case State.RightHalfRingL:
                    {
                        if (state != State.DownBall)
                            return false;
                        else
                            return true;

                    }
                case State.RightHalfRingR:
                    {
                        if (state != State.UpBall)
                            return false;
                        else
                            return true;

                    }
                case State.UpHalfRing:
                    {
                        if (state != State.DownBall)
                            return false;
                        else
                            return true;
                    }
                case State.UpHalfRingL:
                    {
                        if (state != State.RightBall)
                            return false;
                        else
                            return true;
                    }
                case State.UpHalfRingR:
                    {
                        if (state != State.LeftBall)
                            return false;
                        else
                            return true;
                    }


                default: { return false; }

            }
        }
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
