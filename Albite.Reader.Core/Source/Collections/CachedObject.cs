using System;

namespace Albite.Reader.Core.Collections
{
    public abstract class CachedObject<TValue> where TValue : class
    {
        protected WeakReference<TValue> cachedValue
            = new WeakReference<TValue>(null);

        protected abstract TValue GenerateValue();

        public TValue Value
        {
            get
            {
                TValue value = null;

                lock (cachedValue)
                {
                    if (!cachedValue.TryGetTarget(out value))
                    {
                        value = GenerateValue();
                        cachedValue.SetTarget(value);
                    }
                }

                return value;
            }
        }
    }
}
