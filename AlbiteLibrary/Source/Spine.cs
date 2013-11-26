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
        private readonly Chapter[] chapters;

        private Spine(Chapter[] chapters)
        {
            this.chapters = chapters;
        }

        public int Length
        {
            get
            {
                return chapters.Length;
            }
        }

        public Chapter this[int number]
        {
            get
            {
                return chapters[number];
            }
        }

        public Chapter this[string url]
        {
            get
            {
                foreach (Chapter element in chapters)
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
            List<Chapter> spine = new List<Chapter>();

            Chapter previous = null;
            Chapter current = null;
            int number = 0;

            foreach (string url in container.Spine)
            {
                // Add the chapter to the spine
                current = new Chapter(
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
