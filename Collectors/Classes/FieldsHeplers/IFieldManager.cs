using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Classes
{
    public interface IFieldManager
    {
        public abstract void SetFieldByIndex(int index, string s);
        public abstract string GetFieldByIndex(int index);
    }
}
