using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Classes
{
    public interface IFieldManager
    {
        void SetFieldByIndex(int index, string s);
        string GetFieldByIndex(int index);
    }
}
