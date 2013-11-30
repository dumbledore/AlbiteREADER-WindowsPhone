using SvetlinAnkov.Albite.BookLibrary.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.Engine.Internal
{
    internal abstract class InitialLocation
    {
        public abstract string ToEngineKeyword();

        public static InitialLocation GetFirstLocation()
        {
            return FirstLocation.Instance;
        }

        public static InitialLocation GetLastLocation()
        {
            return LastLocation.Instance;
        }

        public static InitialLocation GetPageLocation(int page)
        {
            return new PageLocation(page);
        }

        public static InitialLocation GetDomLocation(DomLocation domLocation)
        {
            return new DomInitialLocation(domLocation);
        }

        public static InitialLocation GetFragmentLocation(string hash)
        {
            return new FragmentInitialLocation(hash);
        }

        private class FirstLocation : InitialLocation
        {
            private static readonly FirstLocation instance = new FirstLocation();
            public static FirstLocation Instance
            {
                get { return instance; }
            }

            private FirstLocation() { }

            public override string ToEngineKeyword()
            {
                return "'first'";
            }
        }

        private class LastLocation : InitialLocation
        {
            private static readonly LastLocation instance = new LastLocation();
            public static LastLocation Instance
            {
                get { return instance; }
            }

            private LastLocation() { }

            public override string ToEngineKeyword()
            {
                return "'last'";
            }
        }

        private class PageLocation : InitialLocation
        {
            private readonly string location;

            public PageLocation(int page)
            {
                location = page.ToString();
            }

            public override string ToEngineKeyword()
            {
                return location;
            }
        }

        private class DomInitialLocation : InitialLocation
        {
            private readonly string location;

            public DomInitialLocation(DomLocation domLocation)
            {
                location = string.Format("'{0}'", domLocation.ToString());
            }

            public override string ToEngineKeyword()
            {
                return location;
            }
        }

        private class FragmentInitialLocation : InitialLocation
        {
            private readonly string fragment;

            public FragmentInitialLocation(string fragment)
            {
                if (!fragment.StartsWith("#"))
                {
                    throw new ArgumentException();
                }

                this.fragment = string.Format("'{0}'", fragment);
            }

            public override string ToEngineKeyword()
            {
                return fragment;
            }
        }
    }
}
