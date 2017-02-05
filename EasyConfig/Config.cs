using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.XPath;

namespace Sitecore.Support.sitecore.admin
{
    public static class Config
    {
        #region Variables

        //public static string PatchNmsp = "http://www.sitecore.net/xmlconfig/";
        //public static string SetNmsp = "http://www.sitecore.net/xmlconfig/set/";
        //private static XmlDocument InitialXmlConfiguration;
        //private static XmlDocument ResultXmlConfiguration = new XmlDocument();
        //private XmlDocument ResultXmlConfigurationPreviousStep;
        //private const string XmlNs = "http://www.sitecore.net/xmlconfig/";

        #endregion

        public static string Render(this XmlDocument xmlDoc)
        {
            string result = string.Empty;
            //var configuration = Factory.GetConfiguration();
            //HttpUtility.HtmlEncode()
            

            //Remove comments
            //XPathNavigator xPathNavigator = xmlDoc.CreateNavigator();
            //var commentsNavigator = xPathNavigator.SelectDescendants(XPathNodeType.Comment, false);
            //while (commentsNavigator.MoveNext())
            //{
            //    commentsNavigator.Current.DeleteSelf();
            //}
            //------------------

            result = String.Format("<pre>{0}</pre>", HttpUtility.HtmlEncode(xmlDoc.InnerXml));

            //foreach (XmlNode node in xmlDoc.ChildNodes)
            //{
            //    node.Render(ref result);
            //}
            //Sitecore.Diagnostics.Log.Info("STRING - " + result,new object());
            return result;

        }

        public static void Render(this XmlNode xmlNode, ref string render)
        {
            //Sitecore.Diagnostics.Log.Info("STRING. - " + render, new object());
            if (xmlNode.NodeType==XmlNodeType.Text)
            {
                render += xmlNode.Value;
                return ;
            }

            render += "<" + xmlNode.Name + " ";

            if (xmlNode.Attributes!=null&xmlNode.Attributes.Count>0)
            {
                foreach(XmlAttribute attr in xmlNode.Attributes)
                {
                    render += attr.Name + "=\"" + attr.Value + "\" ";
                }
            }
            if (!xmlNode.HasChildNodes)
            {
                render += "/>" + System.Environment.NewLine; ;
                return ;
            }
            render += ">\n";
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                node.Render(ref render);
            }
            

            render += "</" + xmlNode.Name + ">"+ System.Environment.NewLine;
            return ;
        }
    }
}