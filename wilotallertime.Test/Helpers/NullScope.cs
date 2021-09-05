using System;

namespace wilotallertime.Test.Helpers
{
    public class NullScope : IDisposable
    {
        public static NullScope Instance { get; } 


        public void Dispose() { }


        private NullScope() { }
    }
}
