using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Android.Content;
using Java.IO;
using Org.XmlPull.V1;
namespace BottomNavigationBar.Parsers
{
    public class TabParser
    {
        private Context _context;
        private XmlReader _reader;

        private BottomBarTab _workingTab;

        private List<BottomBarTab> _tabs;
        public List<BottomBarTab> Tabs { get { return _tabs; } }

        public TabParser(Context context, int tabsXmlResId)
        {
            //_context = context;
            //_reader = context.Resources.GetXml(tabsXmlResId);

            //_tabs = new List<BottomBarTab>();

            //Parse();
        }

        private void Parse()
        {
            //try
            //{
            //    while(_reader.Read())
            //    {
            //        switch (_reader.NodeType)
            //        {
            //            case XmlNodeType.Element:
            //                var elementName = _reader.LocalName;
            //                if (elementName == "tab")
            //                {
            //                    //_tabs.Add(ParseNewTab(_reader));
            //                    var s = new XmlSerializer(typeof(BottomBarTab));
            //                    var tab = s.Deserialize(_reader);
            //                }
            //                break;
                            
            //        }
            //    }
            //}
            //catch (XmlPullParserException e)
            //{
            //    e.PrintStackTrace();
            //}
            //catch (IOException e)
            //{
            //    e.PrintStackTrace();
            //}
        }

        private BottomBarTab ParseNewTab(XmlReader parser)
        {
            var tab = new BottomBarTab();

            //for (int i = 0; i < parser.AttributeCount; i++)
            //{
                var attrName = parser.Name;
                //switch (attrName)
                //{
                //    case "id":
                //        tab.Id = parser.ReadElementContentAsInt();
                //        break;
                //    case "color":
                //        tab.Color = GetColorValue(i, parser);
                //        break;
                //    case "title":
                //        tab.Title = GetTitleValue(i, parser);
                //        break;
                //    case "icon":
                //        tab.IconResId = parser.Value(i, 0);
                //        break;
                //}
            //}

            return tab;
        }

        //private string GetTitleValue(int attrIndex, XmlReader parser)
        //{
        //    var titleResource = parser.GetAttributeResourceValue(attrIndex, 0);

        //    return titleResource != 0 ? _context.GetString(titleResource) : parser.GetAttributeValue(attrIndex);
        //}

        //private int GetColorValue(int attrIndex, XmlReader parser)
        //{
        //    var colorResource = parser.GetAttributeResourceValue(attrIndex, 0);

        //    return colorResource != 0 ? ContextCompat.GetColor(_context, colorResource) : Color.ParseColor(parser.GetAttributeValue(attrIndex));
        //}
   }
}
