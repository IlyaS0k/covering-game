using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class ComparableList<T> : List<T>, IComparable where T : IComparable
    {
        public int CompareTo(object other)
        {
            if (other == null) return 1;
            int otherCount = ((ComparableList<T>)other).Count;
            ComparableList<T> otherList = (ComparableList<T>)other;
            if (this.Count < otherCount) return -1;
            if (this.Count > otherCount) return 1;
            for (int i =0; i < this.Count; i++)
            {
                int comparison = this[i].CompareTo(otherList[i]);
                if (comparison < 0) return -1;
                if (comparison > 0) return 1;
            }
            return 0;
        }
    }
}
