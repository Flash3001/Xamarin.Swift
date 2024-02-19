using System;
using System.Collections.Generic;
using System.Linq;

namespace SwiftSupport.Internals
{
    public class SafeList<T> : List<T>
    {
        private readonly object _locker = new object();

        public bool AddIfNotFound(Func<T, bool> where, Func<T> creator)
        {
            // Is there a native way to do this? 
            lock (_locker)
            {
                if (this.Any(where) == false)
                {
                    Add(creator());
                    return true;
                }
            }

            return false;
        }
    }
}
