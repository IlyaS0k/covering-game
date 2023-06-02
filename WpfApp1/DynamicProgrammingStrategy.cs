using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WpfApp1
{
    class DynamicProgrammingStrategy : IStrategy
    {
        private Dictionary<Tuple<int, int>, Tuple<int, int, int>> _cordMap = new Dictionary<Tuple<int, int>, Tuple<int, int, int>>();
        private List<List<int>> _holeCounter;
        private List<List<int>> _visited;

        public bool executeStrategy(Field area)
        {
            countHoles(area);
            setCords(area);
            setVisited(area);
            for (int r = 0; r < area.Rows; ++r)
            {
                for (int c = 0; c < area.Columns; ++c)
                {
                    if (area[r,c].State != State.Hole && _visited[r][c] == 0)
                    {
                        SortedSet<ComparableList<Tuple<int, int, int, int>>> chains = new SortedSet<ComparableList<Tuple<int, int, int, int>>>();
                        ComparableList<Tuple<int, int, int>> cells = new ComparableList<Tuple<int, int, int>>();
                        coverComponent(area, cells, chains, r, c);
                        setСover(area, cells, chains);
                     

                    }
                }
            }
            return true;
        }

        private void coverComponent(Field area, ComparableList<Tuple<int, int, int>> cells, SortedSet<ComparableList<Tuple<int,int,int, int>>> chains, int row, int col)
        {
            
            if (!inBounds(area, row, col))
            {
                return;
            }
            _visited[row][col] = 1;
            Tuple<int,int,int> currentCord;
            if (_cordMap.TryGetValue(Tuple.Create(row, col), out currentCord))
            {
                cells.Add(currentCord);
            }
            if (inBounds(area, row, col + 1) && inBounds(area, row + 1, col + 1) && inBounds(area, row + 1, col))
            {
                Tuple<int, int, int> mappedCords;
                ComparableList<Tuple<int,int,int,int>> listTuple = new ComparableList<Tuple<int,int,int,int>>();
                for (int r = row; r <= row + 1; ++r)
                {
                    for (int c = col; c <= col + 1; ++c)
                    {
                        if (_cordMap.TryGetValue(Tuple.Create(r, c), out mappedCords))
                        {
                            listTuple.Add(Tuple.Create(mappedCords.Item1,mappedCords.Item2,mappedCords.Item3, listTuple.Count));
                        }
                    }
                }
                chains.Add(listTuple);
            }

            int x0 = row;
            int y0 = col;
            Tuple<int, int, int> initialCords = Tuple.Create(x0, y0, 0);
            SortedSet<Tuple<int, int, int>> current = new SortedSet<Tuple<int, int, int>>
            {
                initialCords
            };
            findAllChainsInCell(area, initialCords, 1, 3, current, chains);
            findAllChainsInCell(area, initialCords, 1, 5, current, chains);
            findAllChainsInCell(area, initialCords, 1, 7, current, chains);
            for (int deltaRow = -1; deltaRow <=1; ++deltaRow)
            {
                for (int deltaCol = -1; deltaCol <=1; ++deltaCol)
                {
                    int nextRow = row + deltaRow;
                    int nextCol = col + deltaCol;
                    if (Math.Abs(deltaCol) + Math.Abs(deltaRow) == 1)  
                    if (inBounds(area, nextRow, nextCol) && _visited[nextRow][nextCol] == 0)
                    {
                            coverComponent(area, cells, chains, nextRow, nextCol);
                    }
                }
            }
        }

        
        private ComparableList<Tuple<int,int,int,int>> sortedSetIntoList (SortedSet<Tuple<int,int,int>> ss)
        {
            ComparableList<Tuple<int, int, int, int>> list = new ComparableList<Tuple<int, int,int, int>>();
            foreach(Tuple<int,int,int> tuple in ss)
            {
                Tuple<int, int, int> mappedCords;
                if (_cordMap.TryGetValue(Tuple.Create(tuple.Item1, tuple.Item2), out mappedCords))
                {
                    Tuple<int, int, int, int> result = Tuple.Create(mappedCords.Item1, mappedCords.Item2, mappedCords.Item3, tuple.Item3);
                    list.Add(result);
                }
            }
            return list;
        }
        private bool inBounds(Field area, int row, int col)
        {
            if (row >= area.Rows || row < 0 || col < 0 ||
                col >= area.Columns || area[row, col].State == State.Hole)
            {
                return false;
            }
            return true;
        }

        private bool setInBounds(Field area, SortedSet<Tuple<int,int>> ss)
        {
            foreach (Tuple<int,int> tuple in ss)
            {
                if (!inBounds(area, tuple.Item1, tuple.Item2))
                {
                    return false;
                }
            }
            return true;
        }
        private void countHoles(Field area)
        {
            _holeCounter = new List<List<int>>();
            for (int r = 0; r < area.Rows; ++r)
            {
                _holeCounter.Add(new List<int>());
                for (int c = 0; c < area.Columns; ++c)
                {
                    _holeCounter[r].Add(0);
                }

            }
            int currentCount = 0;
            for (int r = 0; r < area.Rows; ++r)
            {
                for (int c = 0; c < area.Columns; ++c)
                {
                    if (area[r, c].State == State.Hole)
                    {
                        currentCount++;
                    }
                    _holeCounter[r][c] = currentCount;
                }
            }
        }

        private void setCords(Field area)
        {
            for (int r = 0; r < area.Rows; ++r)
            {
                for (int c = 0; c < area.Columns; ++c)
                {
                    Tuple<int, int> cords2D = new Tuple<int, int>(r, c);
                    int pos1D = area.Columns*r + c - _holeCounter[r][c];
                    Tuple<int, int, int> cords1D = new Tuple<int, int, int>(r, c, pos1D);
                    _cordMap.Add(cords2D, cords1D);
                }
            }
        }   

        private void setVisited(Field area)
        {
            _visited = new List<List<int>>();
            for (int r = 0; r < area.Rows; ++r)
            {
               _visited.Add(new List<int>());
                for (int c = 0; c < area.Columns; ++c)
                {
                    _visited[r].Add(0);
                }
            }
            for (int r = 0; r < area.Rows; ++r)
            {
                for (int c = 0; c < area.Columns; ++c)
                {
                    _visited[r][c] = 0;
                }
            }
        }
        private Tuple<int,int> changeCord(int x, int y, int d)
        {
            if (d == 0) return Tuple.Create(x + 1, y);
            if (d == 1) return Tuple.Create(x - 1, y);
            if (d == 2) return Tuple.Create(x, y + 1);
            return Tuple.Create(x, y - 1);
        }

        private void findAllChainsInCell(Field area, Tuple<int,int,int> cords, int curLen, 
            int needLen, SortedSet<Tuple<int, int, int>> current, 
            SortedSet<ComparableList<Tuple<int, int, int, int>>> chains)
        {
            if (curLen == needLen)
            {
                chains.Add(sortedSetIntoList(current));
                return;
            }
            for (int d = 0; d < 4; ++d)
            {
                Tuple<int,int> newCords = changeCord(cords.Item1, cords.Item2, d);
                if (inBounds(area, newCords.Item1, newCords.Item2))
                {
                    Tuple<int, int, int> nextStep = Tuple.Create(newCords.Item1, newCords.Item2, current.Count);
                    bool isUnique = true;
                    foreach (Tuple<int, int, int> el in current)
                    {
                        if (nextStep.Item1 == el.Item1 && nextStep.Item2 == el.Item2)
                        {
                            isUnique = false;
                        }
                    }
                    if (isUnique)
                    {
                        current.Add(nextStep);
                        findAllChainsInCell(area, Tuple.Create(newCords.Item1, newCords.Item2, current.Count), curLen + 1, needLen, current, chains);
                        current.Remove(nextStep);
                        
                    }
                    
                }
            }
        }

        private void sortChain(ComparableList<Tuple<int, int, int, int>> chain)
        {
            if (chain.Count == 4)
            {
                for (int j = 4; j >= 0; --j)
                    for (int i = 0; i < chain.Count - 1; ++i)
                    {
                        if (chain[i].Item1 > chain[i + 1].Item1 ||
                           (chain[i].Item1 == chain[i + 1].Item1 && chain[i].Item2 > chain[i + 1].Item2))
                        {
                            var tmp = chain[i];
                            chain[i] = chain[i + 1];
                            chain[i + 1] = tmp;
                        }
                    }
            }
            else
            {
                for (int j = 7; j >= 0; j--)
                {
                    for (int i = 0; i < chain.Count - 1; ++i)
                    {
                        if (chain[i].Item4 > chain[i + 1].Item4)
                        {
                            var tmp = chain[i];
                            chain[i] = chain[i + 1];
                            chain[i + 1] = tmp;
                        }
                    }
                }

            }
        }

        private void fillField(Field area, ComparableList<Tuple<int, int, int, int>> chain)
        {
            if (chain.Count == 4)
            {
                Tuple<int, int, int, int> t0 = chain[0];
                Tuple<int, int, int, int> t1 = chain[1];
                Tuple<int, int, int, int> t3 = chain[2];
                Tuple<int, int, int, int> t2 = chain[3];
                area[t0.Item1, t0.Item2].State = State.RightDownCompound;
                area[t1.Item1, t1.Item2].State = State.LeftDownCompound;
                area[t2.Item1, t2.Item2].State = State.LeftUpCompound;
                area[t3.Item1, t3.Item2].State = State.RightUpCompound;
            }
            else
            {
                if (chain[1].Item1 > chain[0].Item1)
                {
                    area[chain[0].Item1, chain[0].Item2].State = State.DownBall;
                }
                else if (chain[1].Item1 < chain[0].Item1)
                {
                    area[chain[0].Item1, chain[0].Item2].State = State.UpBall;
                }
                else if (chain[1].Item2 > chain[0].Item2)
                {
                    area[chain[0].Item1, chain[0].Item2].State = State.RightBall;
                }
                else if (chain[1].Item2 < chain[0].Item2)
                {
                    area[chain[0].Item1, chain[0].Item2].State = State.LeftBall;
                }
                for (int i = 1; i < chain.Count - 1; ++i)
                {
                    if (chain[i - 1].Item1 < chain[i].Item1)
                    {
                        if (chain[i + 1].Item1 > chain[i].Item1)
                        {
                            area[chain[i].Item1, chain[i].Item2].State = 
                                i % 2 == 0 ? State.UpDownCompound : State.UpDownRing;
                        }
                        else if (chain[i + 1].Item2 > chain[i].Item2)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.RightUpCompound : State.RightUpRing;
                        }
                        else if (chain[i + 1].Item2 < chain[i].Item2)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.LeftUpCompound : State.LeftUpRing;
                        }
                    }
                    else if (chain[i - 1].Item1 > chain[i].Item1)
                    {
                        if (chain[i + 1].Item1 < chain[i].Item1)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.UpDownCompound : State.UpDownRing;
                        }
                        else if (chain[i + 1].Item2 > chain[i].Item2)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.RightUpCompound : State.RightUpRing;
                        }
                        else if (chain[i + 1].Item2 < chain[i].Item2)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.LeftUpCompound : State.LeftUpRing;
                        }
                    }
                    else if (chain[i - 1].Item2 < chain[i].Item2)
                    {
                        if (chain[i + 1].Item1 > chain[i].Item1)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.LeftDownCompound : State.LeftDownRing;
                        }
                        else if (chain[i + 1].Item2 > chain[i].Item2)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.LeftRightCompound : State.LeftRightRing;
                        }
                        else if (chain[i + 1].Item1 < chain[i].Item1)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.LeftUpCompound : State.LeftUpRing;
                        }
                    }
                    else if(chain[i - 1].Item2 > chain[i].Item2)
                    {
                        if (chain[i + 1].Item1 > chain[i].Item1)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.RightDownCompound : State.RightDownRing;
                        }
                        else if (chain[i + 1].Item2 < chain[i].Item2)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.LeftRightCompound : State.LeftRightRing;
                        }
                        else if (chain[i + 1].Item1 < chain[i].Item1)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.RightUpCompound : State.RightUpRing;
                        }
                    }
                }
                if (chain[chain.Count - 2].Item1 > chain[chain.Count - 1].Item1)
                {
                    area[chain[chain.Count - 1].Item1, chain[chain.Count - 1].Item2].State = State.DownBall;
                }
                else if (chain[chain.Count - 2].Item1 < chain[chain.Count - 1].Item1)
                {
                    area[chain[chain.Count - 1].Item1, chain[chain.Count - 1].Item2].State = State.UpBall;
                }
                else if (chain[chain.Count - 2].Item2 > chain[chain.Count - 1].Item2)
                {
                    area[chain[chain.Count - 1].Item1, chain[chain.Count - 1].Item2].State = State.RightBall;
                }
                else if (chain[chain.Count - 2].Item2 < chain[chain.Count - 1].Item2)
                {
                    area[chain[chain.Count - 1].Item1, chain[chain.Count - 1].Item2].State = State.LeftBall;
                }
            }
        }
        private void setСover(Field area, ComparableList<Tuple<int, int, int>> elements, SortedSet<ComparableList<Tuple<int, int, int, int>>> subsets)
        {
            int subsetsCount = subsets.Count;
            int cardinalityOfSet = elements.Count;
            if (cardinalityOfSet > 30) throw new Exception("Недостаточно оперативной памяти!");
            int bits = (1 << cardinalityOfSet);
            BitArray[] dp;
            Dictionary<int, Tuple<int, int, int>> mapFrom1DTo3D = new Dictionary<int, Tuple<int, int, int>>();
            Dictionary<Tuple<int, int, int>, int> mapFrom3DTo1D = new Dictionary<Tuple<int, int, int>, int>();
            try
            {
                dp = new BitArray[subsetsCount + 1];
                for (int i = 0; i < dp.Length; ++i)
                {
                    dp[i] = new BitArray(bits);
                }
            }
            catch
            {
                throw new Exception("Недостаточно оперативной памяти!");
            }
            for (int i = 0; i < elements.Count; i++)
            {
                mapFrom1DTo3D.Add(i, elements[i]);
                mapFrom3DTo1D.Add(elements[i], i);
            }
            int curNum = 0;
            dp[0][0] = true;
            foreach (ComparableList<Tuple<int, int, int, int>> currentChain in subsets)
            {
                curNum++;
                for (int mask = 0; mask < bits; ++mask)
                {
                    bool allOnes = true;
                    int currentMask = 0;
                    foreach (Tuple<int,int,int,int> el in currentChain)
                    {
                          int curBit;
                         Tuple<int, int, int> tuple = Tuple.Create(el.Item1, el.Item2, el.Item3);
                          mapFrom3DTo1D.TryGetValue(tuple, out curBit);
                          if (((1 << curBit) & mask) == 0)
                          {
                            allOnes = false;
                            break;
                          }
                          currentMask += (1 << curBit);
                    }

                    if (mask == 0)
                    {
                        dp[curNum][mask] = true;
                    }
                    else
                    {
                        dp[curNum][mask] = dp[curNum - 1][mask];
                        if (allOnes && dp[curNum - 1][mask ^ currentMask] == true)
                            dp[curNum][mask] = dp[curNum - 1][mask ^ currentMask];
                    }
                    
                }
            }
            int restoreMask = bits - 1;
            int restoreSetNumber = curNum;
            var subsetsAsList = subsets.ToList();
            if (dp[restoreSetNumber][restoreMask] == false) return;
            while (true)
            {
                if (restoreMask == 0 || restoreSetNumber == 0)
                {
                    break;
                }
                if (dp[restoreSetNumber - 1][restoreMask] == true)
                {
                    restoreSetNumber--;
                }
                else
                {
                    int currentMask = 0;
                    sortChain(subsetsAsList[restoreSetNumber - 1]);
                    foreach (Tuple<int, int, int, int> el in subsetsAsList[restoreSetNumber-1])
                    {
                        int curBit;
                        Tuple<int, int, int> tuple = Tuple.Create(el.Item1, el.Item2, el.Item3);
                        mapFrom3DTo1D.TryGetValue(tuple, out curBit);
                        currentMask += (1 << curBit);
                        fillField(area, subsetsAsList[restoreSetNumber - 1]);
                    }
                    restoreMask ^= currentMask;
                }
            }
        }
    }
}
