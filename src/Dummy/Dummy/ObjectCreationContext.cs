using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dummy
{
    public class ObjectCreationContext
    {
        public IDictionary<string, object> TempData { get; set; }

        public object CurrentObject { get; set; }

        public int Index { get; set; }
    }
}
