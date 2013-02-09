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
using System.Xml;
using System.IO;
using System.Diagnostics;

namespace SvetlinAnkov.AlbiteREADER.Utils.Xml
{
    /// <summary>
    /// This class is a tiny wrapper around an XmlReader.
    ///
    /// There are at least two problems with the Xml implementation on WP7:
    ///
    /// 1. XmlReader cannot parse the doctype so it needs to be explicitly disabled.
    /// 2. XDocument is not happy if an XmlReader is on a XmlDeclaration node,
    ///    so one needs to skip those kind of nodes.
    ///
    /// Both problems are incredibly annoying and it's astounding they haven't
    /// been fixed by Microsoft themselves.
    ///
    /// The first one is easily remedied by setting the XmlReader not to parse DTDs
    /// using the XmlSettings class.
    ///
    /// The second one needs the XmlReader to skip XmlDeclartion nodes. This might
    /// be done cleanly by overriding XmlReader.NodeType and pretending XmlDeclartion
    /// to be an XmlComment.
    /// </summary>
    public class AlbiteXmlReader : XmlReader
    {
        XmlReader reader;

        public AlbiteXmlReader(Stream stream) : this(stream, new XmlReaderSettings()) { }

        public AlbiteXmlReader(Stream stream, XmlReaderSettings settings)
        {
            settings.DtdProcessing = DtdProcessing.Ignore;
            reader = XmlReader.Create(stream, settings);
        }

        // That's the important part.
        public override XmlNodeType NodeType
        {
            get {
                // Get the type from the implementation
                XmlNodeType type = reader.NodeType;

                // If it's XmlDeclaration (which causes the problem with XDocument),
                // pretends it's a comment
                if (type == XmlNodeType.XmlDeclaration)
                {
                    type = XmlNodeType.Comment;
                }

                return type;
            }
        }

        // Now override the abstract methods / properties
        public override string LocalName
        {
            get { return reader.LocalName; }
        }

        public override void ResolveEntity()
        {
            reader.ResolveEntity();
        }

        public override string LookupNamespace(string prefix)
        {
            return reader.LookupNamespace(prefix);
        }

        public override XmlNameTable NameTable
        {
            get { return reader.NameTable; }
        }

        public override ReadState ReadState
        {
            get { return reader.ReadState; }
        }

        public override bool EOF
        {
            get { return reader.EOF; }
        }

        public override bool Read()
        {
            return reader.Read();
        }

        public override bool ReadAttributeValue()
        {
            return ReadAttributeValue();
        }

        public override bool MoveToElement()
        {
            return reader.MoveToElement();
        }

        public override bool MoveToNextAttribute()
        {
            return reader.MoveToNextAttribute();
        }

        public override bool MoveToFirstAttribute()
        {
            return reader.MoveToFirstAttribute();
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            return reader.MoveToAttribute(name, ns);
        }

        public override bool MoveToAttribute(string name)
        {
            return reader.MoveToAttribute(name);
        }

        public override string GetAttribute(int i)
        {
            return reader.GetAttribute(i);
        }

        public override string GetAttribute(string name, string namespaceURI)
        {
            return reader.GetAttribute(name, namespaceURI);
        }

        public override string GetAttribute(string name)
        {
            return reader.GetAttribute(name);
        }

        public override int AttributeCount
        {
            get { return reader.AttributeCount; }
        }

        public override bool IsEmptyElement
        {
            get { return reader.IsEmptyElement; }
        }

        public override string BaseURI
        {
            get { return reader.BaseURI; }
        }

        public override int Depth
        {
            get { return reader.Depth; }
        }

        public override string Value
        {
            get { return reader.Value; }
        }

        public override string Prefix
        {
            get { return reader.Prefix; }
        }

        public override string NamespaceURI
        {
            get { return reader.NamespaceURI; }
        }

        // It's important to forward all of the virtuals as well.

        public override bool CanReadBinaryContent
        {
            get { return reader.CanReadBinaryContent; }
        }

        public override bool CanReadValueChunk
        {
            get { return reader.CanReadValueChunk; }
        }

        public override bool CanResolveEntity
        {
            get { return reader.CanResolveEntity; }
        }

        public override void Close()
        {
            reader.Close();
        }

        protected override void Dispose(bool disposing)
        {
            reader.Dispose();
        }

        public override object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
        {
            return reader.ReadContentAs(returnType, namespaceResolver);
        }

        public override int ReadContentAsBase64(byte[] buffer, int index, int count)
        {
            return reader.ReadContentAsBase64(buffer, index, count);
        }

        public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
        {
            return reader.ReadContentAsBinHex(buffer, index, count);
        }

        public override bool ReadContentAsBoolean()
        {
            return reader.ReadContentAsBoolean();
        }

        public override DateTime ReadContentAsDateTime()
        {
            return reader.ReadContentAsDateTime();
        }

        public override decimal ReadContentAsDecimal()
        {
            return reader.ReadContentAsDecimal();
        }

        public override double ReadContentAsDouble()
        {
            return reader.ReadContentAsDouble();
        }

        public override float ReadContentAsFloat()
        {
            return reader.ReadContentAsFloat();
        }

        public override int ReadContentAsInt()
        {
            return reader.ReadContentAsInt();
        }

        public override long ReadContentAsLong()
        {
            return reader.ReadContentAsLong();
        }

        public override object ReadContentAsObject()
        {
            return reader.ReadContentAsObject();
        }

        public override string ReadContentAsString()
        {
            return reader.ReadContentAsString();
        }

        public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
        {
            return reader.ReadElementContentAs(returnType, namespaceResolver);
        }

        public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI)
        {
            return reader.ReadElementContentAs(returnType, namespaceResolver, localName, namespaceURI);
        }

        public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
        {
            return reader.ReadElementContentAsBase64(buffer, index, count);
        }

        public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
        {
            return reader.ReadElementContentAsBinHex(buffer, index, count);
        }

        public override bool ReadElementContentAsBoolean()
        {
            return reader.ReadElementContentAsBoolean();
        }

        public override bool ReadElementContentAsBoolean(string localName, string namespaceURI)
        {
            return reader.ReadElementContentAsBoolean(localName, namespaceURI);
        }

        public override DateTime ReadElementContentAsDateTime()
        {
            return reader.ReadElementContentAsDateTime();
        }

        public override DateTime ReadElementContentAsDateTime(string localName, string namespaceURI)
        {
            return reader.ReadElementContentAsDateTime(localName, namespaceURI);
        }

        public override decimal ReadElementContentAsDecimal()
        {
            return reader.ReadElementContentAsDecimal();
        }

        public override decimal ReadElementContentAsDecimal(string localName, string namespaceURI)
        {
            return reader.ReadElementContentAsDecimal(localName, namespaceURI);
        }

        public override double ReadElementContentAsDouble()
        {
            return reader.ReadElementContentAsDouble();
        }

        public override double ReadElementContentAsDouble(string localName, string namespaceURI)
        {
            return reader.ReadElementContentAsDouble(localName, namespaceURI);
        }

        public override float ReadElementContentAsFloat()
        {
            return reader.ReadElementContentAsFloat();
        }

        public override float ReadElementContentAsFloat(string localName, string namespaceURI)
        {
            return reader.ReadElementContentAsFloat(localName, namespaceURI);
        }

        public override int ReadElementContentAsInt()
        {
            return reader.ReadElementContentAsInt();
        }

        public override int ReadElementContentAsInt(string localName, string namespaceURI)
        {
            return reader.ReadElementContentAsInt(localName, namespaceURI);
        }

        public override long ReadElementContentAsLong()
        {
            return reader.ReadElementContentAsLong();
        }

        public override long ReadElementContentAsLong(string localName, string namespaceURI)
        {
            return reader.ReadElementContentAsLong(localName, namespaceURI);
        }

        public override object ReadElementContentAsObject()
        {
            return reader.ReadElementContentAsObject();
        }

        public override object ReadElementContentAsObject(string localName, string namespaceURI)
        {
            return reader.ReadElementContentAsObject(localName, namespaceURI);
        }

        public override string ReadElementContentAsString()
        {
            return reader.ReadElementContentAsString();
        }

        public override string ReadElementContentAsString(string localName, string namespaceURI)
        {
            return reader.ReadElementContentAsString(localName, namespaceURI);
        }

        public override void ReadEndElement()
        {
            reader.ReadEndElement();
        }

        public override string ReadInnerXml()
        {
            return reader.ReadInnerXml();
        }

        public override string ReadOuterXml()
        {
            return reader.ReadOuterXml();
        }

        public override void ReadStartElement()
        {
            reader.ReadStartElement();
        }

        public override void ReadStartElement(string localname, string ns)
        {
            reader.ReadStartElement(localname, ns);
        }

        public override void ReadStartElement(string name)
        {
            reader.ReadStartElement(name);
        }

        public override XmlReader ReadSubtree()
        {
            return reader.ReadSubtree();
        }

        public override bool ReadToDescendant(string localName, string namespaceURI)
        {
            return reader.ReadToDescendant(localName, namespaceURI);
        }

        public override bool ReadToDescendant(string name)
        {
            return reader.ReadToDescendant(name);
        }

        public override bool ReadToFollowing(string localName, string namespaceURI)
        {
            return reader.ReadToFollowing(localName, namespaceURI);
        }

        public override bool ReadToFollowing(string name)
        {
            return reader.ReadToFollowing(name);
        }

        public override bool ReadToNextSibling(string localName, string namespaceURI)
        {
            return reader.ReadToNextSibling(localName, namespaceURI);
        }

        public override bool ReadToNextSibling(string name)
        {
            return reader.ReadToNextSibling(name);
        }

        public override int ReadValueChunk(char[] buffer, int index, int count)
        {
            return reader.ReadValueChunk(buffer, index, count);
        }

        public override bool IsStartElement()
        {
            return reader.IsStartElement();
        }

        public override bool IsStartElement(string localname, string ns)
        {
            return reader.IsStartElement(localname, ns);
        }

        public override bool IsStartElement(string name)
        {
            return reader.IsStartElement(name);
        }

        public override void MoveToAttribute(int i)
        {
            reader.MoveToAttribute(i);
        }

        public override XmlNodeType MoveToContent()
        {
            return reader.MoveToContent();
        }

        public override void Skip()
        {
            reader.Skip();
        }

        public override bool HasAttributes
        {
            get { return reader.HasAttributes; }
        }

        public override bool HasValue
        {
            get { return reader.HasValue; }
        }

        public override bool IsDefault
        {
            get { return reader.IsDefault; }
        }

        public override string Name
        {
            get { return reader.Name; }
        }

        public override XmlReaderSettings Settings
        {
            get { return reader.Settings; }
        }

        public override string this[int i]
        {
            get { return reader[i]; }
        }

        public override string this[string name, string namespaceURI]
        {
            get { return reader[name, namespaceURI]; }
        }

        public override string this[string name]
        {
            get { return reader[name]; }
        }

        public override Type ValueType
        {
            get { return reader.ValueType; }
        }

        public override string XmlLang
        {
            get { return reader.XmlLang; }
        }

        public override XmlSpace XmlSpace
        {
            get { return reader.XmlSpace; }
        }
    }
}
