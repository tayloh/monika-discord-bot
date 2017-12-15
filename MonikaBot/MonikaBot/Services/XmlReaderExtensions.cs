using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MonikaBot.Extensions
{
    public static class XmlReaderExtensions
    {
        public static IEnumerable<Story> Stories(this XmlReader source)
        {
            while (source.Read())
            {
                if(source.NodeType == XmlNodeType.Element && source.Name == "Story")
                {
                    int.TryParse(source.GetAttribute("nr"), out int nr);
                    string title = source.GetAttribute("title");
                    string poem = source.ReadInnerXml();
                    yield return new Story
                    {
                        Nr = nr,
                        Poem = new string[] {title, poem}
                    };
                }
            }
        }
    }
}
