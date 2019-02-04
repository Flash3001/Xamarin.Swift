using System;

namespace SwiftSupport.Internals
{
    internal class Dependency
    {
        public string Dylib { get; }
        public bool Pending { get; private set; } = true;

        public Dependency(string dylib)
        {
            Dylib = dylib;
        }

        public void MarkAsScanned() => Pending = false;
    }
}
