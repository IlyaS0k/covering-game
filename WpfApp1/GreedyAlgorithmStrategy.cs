using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace WpfApp1
{
    
    class GreedyAlgorithmStrategy : IStrategy
    {
        private List<List<int>> _visited;
        private List<List<int>> _bounds;
        private List<List<Tuple<int,int>>> _extremeVertex;
        private List<List<int>> _vertexDegree;
        int[,] _used;
        private const int _chainLengthMaximumWeight = 100;
        private const int _chainOfLength4Weight = 40;
        private const int _chainOfLength5Weight = 65;
        private const int _chainOfLength7Weight = 75;
        private Random random = new Random();
        int[,] steps = new int[4, 2] {
                        {1, 0},
                        {0, 1},
                        {-1,0},
                        {0,-1}
                    };
        public bool executeStrategy(Field area)
        {
            _used = new int[area.Rows, area.Columns];
            setVisited(area);
            setVertexDegree(area);
            Tuple<int, int> currentVertex = getVertexWithMinimumDegree(area);
            int tries = (area.Columns + area.Rows) * (area.Columns + area.Rows) * 500;
            List<ComparableList<Tuple<int, int>>> chains = new List<ComparableList<Tuple<int, int>>>();
            while (tries > 0)
            {
                  --tries;
                    currentVertex = getVertexWithMinimumDegree(area);
                    int chainLength = -1;
                    int triesToGenerateLength = 7;
                    ComparableList<Tuple<int, int>> nextChain = null;
                    while (triesToGenerateLength-- > 0)
                    {
                        chainLength = generateRandomChainLength();
                        nextChain = new ComparableList<Tuple<int, int>>() { currentVertex };
                        nextChain = findRandomChain(area, currentVertex, 1, chainLength, nextChain);//пытаемся такую найти
                        if (nextChain != null) break;
                        triesToGenerateLength--;
                    }
                    if (nextChain != null)//нашли
                    {
                        chains.Add(nextChain);//добавляем в пул цепочек
                        for (int i = 0; i < nextChain.Count; ++i)
                        {
                        int x = nextChain[i].Item1;
                        int y = nextChain[i].Item2;
                        _visited[x][y] = chainLength;//отмечаем вершины помечеными

                        if (i == 0 && chainLength != 4)
                        {
                            _bounds[x][y] = chains.Count;
                            _extremeVertex[x][y] = 
                                Tuple.Create(nextChain[nextChain.Count - 1].Item1, nextChain[nextChain.Count - 1].Item2);
                        }
                        if (i == nextChain.Count - 1 && chainLength != 4)
                        {
                            _bounds[x][y] = -chains.Count;
                            _extremeVertex[x][y] =
                                Tuple.Create(nextChain[0].Item1, nextChain[0].Item2);

                        }

                        }
                        calculateVertexDegree(area);//пересчитываем степени вершин
                        currentVertex = getVertexWithMinimumDegree(area);//берем новую вершину с минимальной степенью
                    }
                    bool over = nextChain == null || currentVertex == null;
                    if (over)
                    {
                        while (true)
                        {
                          Tuple<int,int> beginOfPath = findIsolatedVertex(area);
                          if (beginOfPath == null) break;
                          Tuple<int, int> endVertex = null;
                          List<Tuple<int,int>> chainNumbers = new List<Tuple<int,int>>();
                          for (int i = 0; i < area.Rows; ++i)
                          {
                            for (int j = 0; j < area.Columns; ++j)
                            {
                                _used[i, j] = 0; 
                            }
                          }
                          Tuple<int, int> current = beginOfPath;
                          Tuple<int,int> endOfPath = findPathChainsSeq(area, beginOfPath, endVertex, current, chainNumbers);
                          if (endOfPath == null) break;
                          moveChainSeq(area, chainNumbers, chains, beginOfPath, endOfPath);
                        }
                        Tuple<int, int> prevVertex = currentVertex;
                        tryToIncreaseChain(area, chains);
                        currentVertex = getVertexWithMinimumDegree(area);
                        if (currentVertex != null && currentVertex.Item1 == prevVertex.Item1 && currentVertex.Item2 == prevVertex.Item2)
                        {
                            currentVertex = null;
                        }
                        over = currentVertex == null;
                    }
                    if (over)
                    {

                        if (!isFilled(area))// не нашли цепочку но есть непокрытые вершины
                        {


                            chains.Clear();//все заново делаем
                            setVisited(area);
                            setVertexDegree(area);
                        }
                        else//не нашли но вершин нет непокрытых тогда рисуем
                        {
                            foreach (var c in chains)
                            {
                                fillField(area, c);
                            }
                        return true;
                            break;
                        }
                    }
                
            }
            return false;
        }

        private Tuple<int,int> findIsolatedVertex(Field area)
        {
            for (int r = 0; r < area.Rows; ++r)
            {
                for (int c = 0; c < area.Columns; ++c)
                {
                    if (isIsolatedVertex(area, r, c)) return Tuple.Create(r, c);
                }
            }
            return null;
        }
        private Tuple<int,int> findPathChainsSeq(Field area, Tuple<int,int> firstVertex, Tuple<int, int> lastVertex, Tuple<int,int> currentVertex, List<Tuple<int,int>> chainNumbers)
        {
            
            int x = currentVertex.Item1;
            int y = currentVertex.Item2;
            _used[x, y] = 1;

                for (int d = 0; d < 4; ++d)
                {
                    int nx = x + steps[d, 0];
                    int ny = y + steps[d, 1];
                    if (inBounds(area, nx, ny) && _visited[nx][ny] != 0 && _bounds[nx][ny] != 0 && _used[nx, ny] == 0)
                    {
                        _used[nx,ny] = 1;
                        int tail = _bounds[nx][ny] > 0 ? 1 : -1;
                        int number = Math.Abs(_bounds[nx][ny]) - 1;
                        Tuple<int,int> tuple = Tuple.Create(number, tail);
                        chainNumbers.Add(tuple);
                        Tuple<int, int> nextVertex = _extremeVertex[nx][ny];
                        lastVertex = findPathChainsSeq(area, firstVertex, lastVertex, nextVertex, chainNumbers);
                        if (lastVertex != null) return lastVertex;
                        chainNumbers.Remove(tuple);
                    }
                    if (lastVertex != null) return lastVertex;
                    if (currentVertex != firstVertex && inBounds(area, nx, ny) && isIsolatedVertex(area, nx, ny) && _used[nx, ny] == 0)
                    {
                       _used[nx,ny] = 1;
                       return Tuple.Create(nx, ny);
                    }
                    if (lastVertex != null) return lastVertex;
                }
            return lastVertex;
        }
        private void moveChainSeq(Field area, List<Tuple<int, int>> chainNumber, List<ComparableList<Tuple<int, int>>> chains, Tuple<int,int> firstVertex, Tuple<int,int> lastVertex)
        {
            List<List<int>> prevBounds = new List<List<int>>(area.Rows);
            List<List<Tuple<int,int>>> prevExtreme = new List<List<Tuple<int,int>>>(area.Rows);
            for (int i = 0; i < area.Rows; ++i)
            {
                prevBounds.Add(new List<int>());
                prevExtreme.Add(new List<Tuple<int,int>>());
                for (int j = 0; j < area.Columns; ++j)
                {
                    prevBounds[i].Add(_bounds[i][j]);
                    prevExtreme[i].Add(_extremeVertex[i][j]);
                }
            }
            _visited[firstVertex.Item1][firstVertex.Item2] = chains[chainNumber[0].Item1].Count;
            for (int i = chainNumber.Count - 1; i >= 0; --i)
            {
                int number = chainNumber[i].Item1;
                int tail = chainNumber[i].Item2;
                Tuple<int, int> last = tail > 0 ? chains[number][chains[number].Count - 1] : chains[number][0];
                Tuple<int, int> prev = tail > 0 ? chains[number][chains[number].Count - 2] : chains[number][1];
                Tuple<int, int> first = tail < 0 ? chains[number][chains[number].Count - 1] : chains[number][0];
                Tuple<int, int> prevFirst;
                if (i == 0) { prevFirst = firstVertex; }
                else {
                    int prevNumber = chainNumber[i - 1].Item1;
                    int prevTail = chainNumber[i - 1].Item2;    
                    prevFirst = prevTail > 0? chains[prevNumber][chains[prevNumber].Count - 1] : chains[prevNumber][0];
                }
                if (i == chainNumber.Count - 1)
                _visited[last.Item1][last.Item2] = 0;
                _bounds[prev.Item1][prev.Item2] = prevBounds[last.Item1][last.Item2];
                _bounds[prevFirst.Item1][prevFirst.Item2] = prevBounds[first.Item1][first.Item2];
                _bounds[last.Item1][last.Item2] = 0;
                _bounds[first.Item1][first.Item2] = 0;
                _extremeVertex[last.Item1][last.Item2] = null;
                _extremeVertex[first.Item1][first.Item2] = null;
                _extremeVertex[prev.Item1][prev.Item2] = Tuple.Create(prevFirst.Item1, prevFirst.Item2);
                _extremeVertex[prevFirst.Item1][prev.Item2] = Tuple.Create(prev.Item1, prev.Item2);
                if (tail > 0)
                {
                    chains[number].RemoveAt(chains[number].Count - 1);
                    chains[number].Insert(0, prevFirst);
                }
                else
                {
                    chains[number].RemoveAt(0);
                    chains[number].Add(prevFirst);
                }
            }
            calculateVertexDegree(area);
        }
        private bool isIsolatedVertex(Field area, int r, int c)
        {
            if (!inBounds(area, r, c) || _visited[r][c] != 0) return false;
            bool isolated = true;
            for (int d = 0; d < 4; ++d)
            {
                int nr = r + steps[d, 0];
                int nc = c + steps[d, 1];
                if (inBounds(area, nr, nc) && _visited[nr][nc] == 0)
                {
                    isolated = false;
                }
            }
            return isolated;
        }
        private void tryToIncreaseChain(Field area, List<ComparableList<Tuple<int, int>>> chains)
        {
            for (int r = 0; r < area.Rows; ++r)
            {
                for (int c = 0; c < area.Columns; ++c)
                {
                    if (!inBounds(area,r,c) || _visited[r][c] != 0) continue;
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
                                        _bounds[nr][nc] = chainIndex + 1;
         

                                    }
                                    else
                                    {
                                        chainIndex = -_bounds[r1][c1] - 1;
                                        chains[chainIndex].Add(Tuple.Create(r, c));
                                        chains[chainIndex].Add(Tuple.Create(nr, nc));
                                        _bounds[r1][c1] = 0;
                                        _bounds[nr][nc] = -chainIndex - 1;
                                    }
                                    _extremeVertex[nr][nc] = _extremeVertex[r1][c1];
                                    _extremeVertex[r1][c1] = null;
                                    int ex = _extremeVertex[nr][nc].Item1;
                                    int ey = _extremeVertex[nr][nc].Item2;
                                    _extremeVertex[ex][ey] = Tuple.Create(nr, nc);
                                    _visited[nr][nc] = 1;
                                    _visited[r][c] = 1;
                                    tryToIncreaseChain(area, chains);
                                    return;
                                    
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
                                        _bounds[r][c] = chainIndex + 1;
                                        _bounds[r1][c1] = 0;
                                    }
                                    else
                                    {
                                        chainIndex = -_bounds[r1][c1] - 1;
                                        chains[chainIndex].Add(Tuple.Create(nr, nc));
                                        chains[chainIndex].Add(Tuple.Create(r, c));
                                        _bounds[r][c] = -chainIndex - 1;
                                        _bounds[r1][c1] = 0;
                                    }
                                    _extremeVertex[r][c] = _extremeVertex[r1][c1];
                                    _extremeVertex[r1][c1] = null;
                                    int ex = _extremeVertex[r][c].Item1;
                                    int ey = _extremeVertex[r][c].Item2;
                                    _extremeVertex[ex][ey] = Tuple.Create(r, c);
                                    _visited[nr][nc] = 1;
                                    _visited[r][c] = 1;
                                    tryToIncreaseChain(area, chains);
                                    return;
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
            _extremeVertex = new List<List<Tuple<int, int>>>();
            for (int r = 0; r < area.Rows; ++r)
            {
                _visited.Add(new List<int>());
                _bounds.Add(new List<int>());
                _extremeVertex.Add(new List<Tuple<int, int>>());    
                for (int c = 0; c < area.Columns; ++c)
                {
                    _visited[r].Add(0);
                    _bounds[r].Add(0);
                    _extremeVertex[r].Add(Tuple.Create(0, 0));
                }
            }
            for (int r = 0; r < area.Rows; ++r)
            {
                for (int c = 0; c < area.Columns; ++c)
                {
                    _visited[r][c] = 0;
                    _bounds[r][c] = 0;
                    _extremeVertex[r][c] = null;
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

        private bool isFilled(Field area)
        {
            for (int r = 0; r < _visited.Count; ++r)
            {
               for (int c = 0; c < _visited[r].Count; ++c)
                {
                    if (area[r,c].State != State.Hole && _visited[r][c] == 0)
                    {
                        return false;
                    }
                }
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
