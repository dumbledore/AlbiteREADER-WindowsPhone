using System;
using System.Net;
using System.Windows;
using System.Diagnostics;
using System.Threading;

namespace SvetlinAnkov.Albite.Core.Utils
{
    public class ThreadCheck
    {
        private readonly ThreadCheckData data = ThreadCheckData.Create();

        /// <summary>
        /// Checks for thread safety in debug builds. It's not 100%
        /// reliable, but it still can help spotting mistakes
        /// </summary>
        [Conditional("DEBUG")]
        public void Check()
        {
            ThreadCheckData newData = ThreadCheckData.Create();

            if (data != newData)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Thread {0} is different from {1}",
                        newData, data
                    )
                );
            }
        }

        private struct ThreadCheckData
            : IEquatable<ThreadCheckData>
        {
            private readonly int hash;
            private readonly int id;

            private ThreadCheckData(int hash, int id)
            {
                this.hash = hash;
                this.id = id;
            }

            public static ThreadCheckData Create()
            {
                Thread current = Thread.CurrentThread;
                int hash = current.GetHashCode();
                int id = current.ManagedThreadId;
                return new ThreadCheckData(hash, id);
            }

            public bool Equals(ThreadCheckData other)
            {
                return this.hash == other.hash
                    && this.id == other.id;
            }

            public override bool Equals(object obj)
            {
                return this.Equals((ThreadCheckData) obj);
            }

            public override int GetHashCode()
            {
                return hash ^ id;
            }

            public override string ToString()
            {
                return string.Format("#{0}:{1}", hash, id);
            }


            public static bool operator ==(ThreadCheckData a, ThreadCheckData b)
            {
                // Note:
                // A common error in overloads of operator == is to use (a == b),
                // (a == null), or (b == null) to check for reference equality.
                // This instead results in a call to the overloaded operator ==,
                // causing an infinite loop. Use ReferenceEquals or cast the
                // type to Object, to avoid the loop.

                // If both are null, or both are same instance, return true.
                if (System.Object.ReferenceEquals(a, b))
                {
                    return true;
                }

                // If one is null, but not both, return false.
                if (((object) a == null) || ((object) b == null))
                {
                    return false;
                }

                return a.Equals(b);
            }

            public static bool operator !=(ThreadCheckData a, ThreadCheckData b)
            {
                return !(a == b);
            }
        }
    }
}
