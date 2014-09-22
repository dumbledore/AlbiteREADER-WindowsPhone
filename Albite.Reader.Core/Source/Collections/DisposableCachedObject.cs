using System;

namespace Albite.Reader.Core.Collections
{
    public abstract class DisposableCachedObject<TValue> : CachedObject<TValue> where TValue : class, IDisposable
    {
        public void Dispose()
        {
            lock (cachedValue)
            {
                TValue value = null;

                if (cachedValue.TryGetTarget(out value))
                {
                    value.Dispose();
                    cachedValue.SetTarget(null);
                }
            }
        }
    }
}
