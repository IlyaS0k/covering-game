using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WpfApp1
{
    
    class GreedyAlgorithmStrategy : IStrategy
    {
        private List<List<int>> _visited;
        private List<List<int>> _bounds;
        private List<List<int>> _vertexDegree;
        private const int _chainLengthMaximumWeight = 100;
        private const int _chainOfLength4Weight = 40;
        private const int _chainOfLength5Weight = 65;
        private const int _chainOfLength7Weight = 80;
        private Random random = new Random();
        public void executeStrategy(Field area)
        {
            setVisited(area);
            setVertexDegree(area);
            Tuple<int, int> currentVertex = getVertexWithMinimumDegree(area);
            int tries = (area.Columns + area.Rows) * (area.Columns + area.Rows) * 10000;
            List<ComparableList<Tuple<int, int>>> chains = new List<ComparableList<Tuple<int, int>>>();
            while (tries > 0)
            {
                --tries;
                int chainLength = generateRandomChainLength();
                ComparableList<Tuple<int, int>> nextChain = new ComparableList<Tuple<int, int>>() { currentVertex };
                nextChain = findRandomChain(area, currentVertex, 1, chainLength, nextChain);
                if (nextChain != null)
                {
                    chains.Add(nextChain);
                    foreach (Tuple<int, int> t in nextChain)
                    {
                        _visited[t.Item1][t.Item2] = chainLength;
                    }
                    calculateVertexDegree(area);
                    currentVertex = getVertexWithMinimumDegree(area);
                }

                if (nextChain == null && getVertexWithMinimumDegree(area) != null)
                {
                    chains.Clear();
                    setVisited(area);
                    setVertexDegree(area);
                }
                if (getVertexWithMinimumDegree(area) == null)
                {
                    foreach (var c in chains)
                    {
                        fillField(area, c);
                    }
                    break;
                }
            }
        }
        private void tryToIncreaseChain(Field area, List<ComparableList<Tuple<int, int>>> chains)
        {
            for (int r = 1; r < area.Rows - 1; ++r)
            {
                for (int c = 1; c < area.Columns; ++c)
                {
                    if (_visited[r][c] != 0) continue;
                    int[,] steps = new int[4, 2] {
                        {1, 0},
                        {0, 1},
                        {-1,0},
                        {0,-1}
                    };
                    for (int d = 0; d < 4; ++d)
                    {
                        int nr = r + steps[d, 0];
                        int nc = c + steps[d, 1];
                        if (inBounds(area, nr, nc) && _visited[nr][nc] == 0)
                        {
                            for (int i = 0; i < 4; ++i)
                            {
                                int r1 = r + steps[i, 0];
                                int c1 = c + steps[i, 1];
                                if (r1 == nr && c1 == nc) continue;
                                if (inBounds(area, r1, c1) && _bounds[r1][c1] !=0)
                                {
                                    int chainIndex = 0;
                                    if (_bounds[r1][c1] > 0)
                                    {
                                        chainIndex = _bounds[r1][c1] - 1;
                                        chains[chainIndex].Insert(0, Tuple.Create(r, c));
                                        chains[chainIndex].Insert(0, Tuple.Create(nr, nc));
                                        _bounds[r1][c1] = 0;
                                        _bounds[nr][nc] = chainIndex;
                                    }
                                    else
                                    {
                                        chainIndex = -_bounds[r1][c1] - 1;
                                        chains[chainIndex].Add(Tuple.Create(r, c));
                                        chains[chainIndex].Add(Tuple.Create(nr, nc));
                                        _bounds[r1][c1] = 0;
                                        _bounds[nr][nc] = -chainIndex;
                                    }
                                    _visited[nr][nc] = 1;
                                    _visited[r][c] = 1;
                                    d = 5;
                                    break;
                                    
                                }
                            }
                            for (int i = 0; i < 4; ++i)
                            {
                                int r1 = nr + steps[i, 0];
                                int c1 = nc + steps[i, 1];
                                if (r1 == r && c1 == c) continue;
                                if (inBounds(area, r1, c1) && _bounds[r1][c1] != 0)
                                {
                                    int chainIndex = 0;
                                    if (_bounds[r1][c1] > 0)
                                    {
                                        chainIndex = _bounds[r1][c1] - 1;
                                        chains[chainIndex].Insert(0, Tuple.Create(nr,nc));
                                        chains[chainIndex].Insert(0, Tuple.Create(r, c));
                                        _bounds[r][c] = chainIndex;
                                        _bounds[r1][c1] = 0;
                                    }
                                    else
                                    {
                                        chainIndex = -_bounds[r1][c1] - 1;
                                        chains[chainIndex].Add(Tuple.Create(nr, nc));
                                        chains[chainIndex].Add(Tuple.Create(r, c));
                                        _bounds[r][c] = -chainIndex;
                                        _bounds[r1][c1] = 0;
                                    }
                                    _visited[nr][nc] = 1;
                                    _visited[r][c] = 1;
                                    d = 5;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private int generateRandomChainLength()
        {
            int randomWeight = random.Next(_chainLengthMaximumWeight);
            if (randomWeight > _chainOfLength7Weight)
            {
                return 7;
            }
            else
            if (randomWeight > _chainOfLength5Weight)
            {
                return 5;
            }
            else
            if (randomWeight > _chainOfLength4Weight)
            {
                return 4;
            }
            else return 3;
        }
        private void setVisited(Field area)
        {
            _visited = new List<List<int>>();
            _bounds = new List<List<int>>();
            for (int r = 0; r < area.Rows; ++r)
            {
                _visited.Add(new List<int>());
                _bounds.Add(new List<int>());
                for (int c = 0; c < area.Columns; ++c)
                {
                    _visited[r].Add(0);
                    _bounds[r].Add(0);
                }
            }
            for (int r = 0; r < area.Rows; ++r)
            {
                for (int c = 0; c < area.Columns; ++c)
                {
                    _visited[r][c] = 0;
                    _bounds[r][c] = 0;
                }
            }
        }
        private void setVertexDegree(Field area)
        {
            _vertexDegree = new List<List<int>>();
            for (int r = 0; r < area.Rows; ++r)
            {
                _vertexDegree.Add(new List<int>());
                for (int c = 0; c < area.Columns; ++c)
                {
                    _vertexDegree[r].Add(0);
                    int[,] steps = new int[4, 2] { 
                        {1,0}, 
                        {0,1}, 
                        {-1,0}, 
                        {0,-1} 
                    };
                    for (int delta = 0; delta < 4; ++delta)
                    {
                        if (inBounds(area, r + steps[delta, 0], c + steps[delta,1]) == true)
                        {
                            ++_vertexDegree[r][c];
                        }
                    }

                }
            }
        }

        private void calculateVertexDegree(Field area)
        {
            for (int c = 0; c < area.Columns; ++c)
            {
                _vertexDegree.Add(new List<int>());
                for (int r = 0; r < area.Rows; ++r)
                {
                    _vertexDegree[r][c] = 0;
                    int[,] steps = new int[4, 2] {
                        {1,0},
                        {0,1},
                        {-1,0},
                        {0,-1}
                    };
                    for (int delta = 0; delta < 4; ++delta)
                    {
                        int newR = r + steps[delta, 0];
                        int newC = c + steps[delta, 1];
                        if (inBounds(area, newR, newC) 
                            && _visited[newR][newC] == 0 && inBounds(area, newR, newC) == true)
                        {
                            ++_vertexDegree[r][c];
                        }
                    }

                }
            }
        }

        private Tuple<int, int> getVertexWithMinimumDegree(Field area)
        {
            Tuple<int, int> result = null;
            int currentMinimumDegree = 5;
            for (int r = 0; r < _vertexDegree.Count; ++r)
            {
                for (int c = 0; c < _vertexDegree[r].Count; ++c)
                {
                    if (_vertexDegree[r][c] < currentMinimumDegree && _visited[r][c] == 0 && inBounds(area,r,c))
                    {
                        result = Tuple.Create(r, c);
                        currentMinimumDegree = _vertexDegree[r][c];
                    }
                }
            }
            return result;
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

        private ComparableList<Tuple<int, int>> findRandomChain(Field area, Tuple<int,int> cords, 
            int curLen, int needLen, ComparableList<Tuple<int, int>> chain)
        {
            if (needLen == 4)
            {
                int x = cords.Item1;
                int y = cords.Item2;
                ComparableList<Tuple<int, int>> upRight = new ComparableList<Tuple<int, int>>() {cords,
                Tuple.Create(x-1,y), Tuple.Create(x-1,y+1), Tuple.Create(x,y+1) };
                ComparableList<Tuple<int, int>> upLeft = new ComparableList<Tuple<int, int>>() {cords,
                Tuple.Create(x-1,y), Tuple.Create(x-1,y-1), Tuple.Create(x,y-1)};
                ComparableList<Tuple<int, int>> downLeft = new ComparableList<Tuple<int, int>>() {cords,
                Tuple.Create(x+1,y), Tuple.Create(x+1,y-1), Tuple.Create(x,y-1)};
                ComparableList<Tuple<int, int>> downRight = new ComparableList<Tuple<int, int>>() {cords,
                Tuple.Create(x+1,y), Tuple.Create(x+1,y+1), Tuple.Create(x,y+1)
                };
                List<ComparableList<Tuple<int, int>>> allLists = new List<ComparableList<Tuple<int, int>>>() 
                {
                    upRight, upLeft, downLeft, downRight
                };
                List<ComparableList<Tuple<int, int>>> chains = new List<ComparableList<Tuple<int, int>>>();
                foreach (var list in allLists)
                {
                    bool isGoodChain = true;
                    foreach (Tuple<int, int> tuple in list)
                    {
                        if (!inBounds(area, tuple.Item1, tuple.Item2) || _visited[tuple.Item1][tuple.Item2] != 0)
                        {
                            isGoodChain = false;
                        }
                    }
                    if (isGoodChain)
                    {
                        chains.Add(list);
                    }
                }
                if (chains.Count == 0) return null;
                int resultIndex = random.Next(chains.Count);
                chain = chains[resultIndex];
                return chain;
            }
            if (curLen == needLen)
            {
                return chain;
            }
            List<int> deltas = new List<int> { 0, 1, 2, 3 };
            var shuffled = deltas.OrderBy(_ => random.Next()).ToList();
            for (int d = 0; d < 4; ++d)
            {
                Tuple<int, int> newCords = changeCord(cords.Item1, cords.Item2, shuffled[d]);
                
                if (inBounds(area, newCords.Item1, newCords.Item2) && _visited[newCords.Item1][newCords.Item2] == 0)
                {
                    bool isUnique = true;
                    foreach (Tuple<int,int> tuple in chain)
                    {
                        if (tuple.Item1 == newCords.Item1 && tuple.Item2 == newCords.Item2)
                        {
                            isUnique = false;
                        }
                    }
                    if (isUnique)
                    {
                        chain.Add(newCords);
                        if (findRandomChain(area, newCords, curLen + 1, needLen, chain) != null)
                        {
                            return chain;
                        }
                        chain.Remove(newCords);
                    }
                }

            }
            return null;
        }

        private Tuple<int, int> changeCord(int x, int y, int d)
        {
            if (d == 0) return Tuple.Create(x + 1, y);
            if (d == 2) return Tuple.Create(x - 1, y);
            if (d == 1) return Tuple.Create(x, y + 1);
            return Tuple.Create(x, y - 1);
        }

        private bool isFilled()
        {
            foreach (var l in _visited)
            {
                if (l.Min() == 0) return false;
            }
            return true;
        }
        private void fillField(Field area, ComparableList<Tuple<int, int>> chain)
        {
            if (chain.Count == 4)
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
                Tuple<int, int> t0 = chain[0];
                Tuple<int, int> t1 = chain[1];
                Tuple<int, int> t3 = chain[2];
                Tuple<int, int> t2 = chain[3];
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
                                i % 2 == 0 ? State.RightDownCompound : State.RightDownRing;
                        }
                        else if (chain[i + 1].Item2 < chain[i].Item2)
                        {
                            area[chain[i].Item1, chain[i].Item2].State =
                                i % 2 == 0 ? State.LeftDownCompound : State.LeftDownRing;
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
                    else if (chain[i - 1].Item2 > chain[i].Item2)
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
    }
}
