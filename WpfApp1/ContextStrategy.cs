using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class ContextStrategy
    {
        private IStrategy _strategy;

        public void setStrategy(IStrategy strategy)
        {
            _strategy = strategy;
        }
        
        public int executeStrategy(Field area)
        {
           return _strategy.executeStrategy(area);
        }

    }
}
