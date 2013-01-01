using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SvetlinAnkov.AlbiteREADER.Utils;
using System.IO;

namespace SvetlinAnkov.AlbiteREADER.Model.Containers
{
    public abstract class BookContainer : IAlbiteContainer
    {
        protected IAlbiteContainer container;
        
        public BookContainer(IAlbiteContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Creates an ePub book from the input file and
        /// extracts its contents into the storage.
        /// 
        /// Throws a BookContainerException if conversion fails.
        /// </summary>
        /// <param name="outputStorage"></param>
        public abstract void Install(AlbiteStorage outputStorage);

        public virtual Stream Stream(string entityName)
        {
            return container.Stream(entityName);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }

    public class BookContainerException : Exception
    {
        public BookContainerException(string message) : base(message) { }
        public BookContainerException(string message, Exception innerException) : base(message, innerException) { }
    }
}
