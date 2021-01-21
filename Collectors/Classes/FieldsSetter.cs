using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Classes
{
    public interface FieldsSetter
    {
        void SetInt(string value, int index);
        void SetString(string value, int index);
        void SetText(string value, int index);
        void SetBool(string value, int index);
        void SetDate(string value, int index);

    }
}
