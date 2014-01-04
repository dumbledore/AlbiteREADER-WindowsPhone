using SvetlinAnkov.Albite.Container.Epub;
using SvetlinAnkov.Albite.Core.Collections;
using SvetlinAnkov.Albite.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SvetlinAnkov.Albite.Container
{
    public abstract class BookContainer : IAlbiteHashableContainer
    {
        public abstract IEnumerable<string> Items { get; }

        public abstract string Title { get; }

        public abstract string Author { get; }

        public abstract string Cover { get; }

        public abstract IEnumerable<String> Spine { get; }

        public abstract ITree<IContentItem> Contents { get; }

        /// <summary>
        /// Returns true if there was a problem when creating
        /// the container.
        /// </summary>
        public bool HadErrors { get; protected set; }

        public bool Fallback { get; private set; }

        public BookContainer()
        {
            HadErrors = false;
        }

        protected BookContainer(bool fallback)
        {
            this.Fallback = fallback;
        }

        /// <summary>
        /// Extracts the contents into the isolated storage.
        /// Doesn't throw if the container has been initialised with
        /// fallback as true.
        /// </summary>
        /// <param name="path">The path on the isolated storage to copy to.</param>
        /// <returns>True if there were no errors</returns>
        public abstract bool Install(string path);

        public abstract Stream Stream(string entityName);

        public abstract byte[] ComputeHash(HashAlgorithm hashAlgorithm);

        public abstract void Dispose();

        public static BookContainer GetContainer(IAlbiteHashableContainer container, string extension)
        {
            extension = extension.ToLowerInvariant();

            switch (extension)
            {
                case ".epub":
                    return new EpubContainer(container);
            }

            throw new InvalidOperationException("Unknown container type");
        }
    }
}
