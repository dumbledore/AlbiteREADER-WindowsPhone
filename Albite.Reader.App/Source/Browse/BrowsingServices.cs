using System;
using System.Collections.Generic;

namespace Albite.Reader.App.Browse
{
    public static class BrowsingServices
    {
        private static readonly IBrowsingService[] services_ =
        {
            OneDriveBrowsingService.Instance,
        };

        public static ICollection<IBrowsingService> Services
        {
            get
            {
                return Array.AsReadOnly<IBrowsingService>(services_);
            }
        }

        public static IBrowsingService GetService(string id)
        {
            foreach (IBrowsingService service in services_)
            {
                if (service.Id == id)
                {
                    return service;
                }
            }

            throw new InvalidOperationException("Unknown browsing service " + id);
        }
    }
}
