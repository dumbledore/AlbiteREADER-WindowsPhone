using SvetlinAnkov.Albite.BookLibrary.Location;
using SvetlinAnkov.Albite.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvetlinAnkov.Albite.BookLibrary
{
    public class Spine
    {
        private readonly SpineElement[] spineElements;

        private Spine(SpineElement[] spineElements)
        {
            this.spineElements = spineElements;
        }

        public int Length
        {
            get
            {
                return spineElements.Length;
            }
        }

        public SpineElement this[int number]
        {
            get
            {
                return spineElements[number];
            }
        }

        public SpineElement this[string url]
        {
            get
            {
                foreach (SpineElement element in spineElements)
                {
                    if (element.Url == url)
                    {
                        return element;
                    }
                }

                return null;
            }
        }

        internal static Spine Create(Book book, BookContainer container)
        {
            List<SpineElement> spine = new List<SpineElement>();

            SpineElement previous = null;
            SpineElement current = null;
            int number = 0;

            foreach (string url in container.Spine)
            {
                // Add the chapter to the spine
                current = new SpineElement(
                        book,
                        number++,
                        url,
                        previous
                );
                spine.Add(current);
                previous = current;
            }

            return new Spine(spine.ToArray());
        }
    }
}
