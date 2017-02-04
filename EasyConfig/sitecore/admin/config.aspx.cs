using Sitecore.sitecore.admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EasyConfig.sitecore.admin
{
    public partial class Config : AdminPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                base.CheckSecurity();
                //this.InitializeInitialRequestData();
                //this.PopulateFormControlsWithInitialValues();
            }
            //this.LoadShowConfig();
            //this.LoadResultConfig();
        }

        /*
        protected void applyPatchButton_OnClick(object sender, EventArgs e)
        {
            this.errorLable.Text = "";
            string message = "";
            if (this.ValidateFormData(ref message) == false)
            {
                this.errorLable.Text = message;
            }
            else
            {
                this.PopulateResultXmlWithNewNodes();
                this.LoadResultConfig();
            }
        }
        protected void resetResultXmlButton_OnClick(object sender, EventArgs e)
        {
            ResultXmlConfiguration = new XmlDocument();
            LoadResultConfig();
        }

        protected void createPatchFileButton_OnClick(object sender, EventArgs e)
        {
            if (ResultXmlConfiguration != null)
            {
                string patchFileNamePath = "Sitecore.Support.000000";
                if (!string.IsNullOrEmpty(this.configFileNameControl.Text))
                {
                    patchFileNamePath = this.configFileNameControl.Text;
                }
                patchFileNamePath = AppDomain.CurrentDomain.BaseDirectory + this.configFilePathControl.Text + "/" +
                                    patchFileNamePath + ".config";
                ResultXmlConfiguration.Save(patchFileNamePath);
                Factory.Reset();
                HttpRuntime.UnloadAppDomain();
                Response.RedirectPermanent(Request.RawUrl, true);
                Response.End();
            }
        }

#endregion

        #region MainPatchLogic

        protected void PopulateResultXmlWithNewNodes()
        {
            XmlNode selectedXmlNode = ResolveNodeByString(this.nodeToPatchControl.Text);
            Stack<XmlNode> xmlNodeParentNodes = CollectParentNodesList(selectedXmlNode);
            this.UpdateResultXml(xmlNodeParentNodes);
            string patchAttribute = "";
            if (string.IsNullOrEmpty(this.patchAttributeSetTextBox.Text) &&
                !string.IsNullOrEmpty(this.Page.Request.Form["patchAttributeDropdownControl"]))
            {
                patchAttribute = this.Page.Request.Form["patchAttributeDropdownControl"];
            }
            else if (!string.IsNullOrEmpty(this.patchAttributeSetTextBox.Text))
            {
                patchAttribute = this.patchAttributeSetTextBox.Text;
            }
            string patchValue = patchValueControl.Text;
            patchValue = patchValue.Replace(" ", "");
            patchAttribute = patchAttribute.Replace(" ", "");
            this.PatchNode(selectedXmlNode, this.patchTypeControl.SelectedItem.Text, patchAttribute,
                patchValue);
            this.UpdateResultXml(xmlNodeParentNodes);
        }

        protected void ApplyPatch()
        {
            switch (this.patchTypeControl.SelectedValue)
            {
                case ("patchDelete"):
                    ;
                    break;
            }
        }

        protected void UpdateResultXml(Stack<XmlNode> patchNodeParentList)
        {
            XmlNode tempXmlNode = ResultXmlConfiguration.DocumentElement;
            var xmlNameSpaceManager = GenerateXmlNamespaceManager(ResultXmlConfiguration);
            string localPath = "";
            XmlNode selectedXmlNode = null;
            if (tempXmlNode != null)
            {
                while (patchNodeParentList.Count != 0)
                {
                    localPath = GenerateNodeLocalPath(patchNodeParentList.Peek());
                    if (tempXmlNode.HasChildNodes)
                        selectedXmlNode = tempXmlNode.SelectSingleNode("child::" + localPath, xmlNameSpaceManager);
                    if (selectedXmlNode == null)
                    {
                        XmlNode currentXmlNode = ResultXmlConfiguration.ImportNode(patchNodeParentList.Pop(), false);
                        tempXmlNode.AppendChild(currentXmlNode);
                        tempXmlNode = currentXmlNode;
                    }
                    else
                    {
                        tempXmlNode = selectedXmlNode;
                        patchNodeParentList.Pop();
                    }
                }
            }
        }

        #endregion

        #region MainXMLManipulations

        public static XmlNamespaceManager GenerateXmlNamespaceManager(XmlDocument xmlDocument)
        {
            var xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
            xmlNamespaceManager.AddNamespace("patch", PatchNmsp);
            xmlNamespaceManager.AddNamespace("set", SetNmsp);
            return xmlNamespaceManager;
        }

        public XmlNode ResolveNodeByString(string nodeToPatch)
        {
            try
            {
                nodeToPatch = new Regex(@"^\s*|\s*$").Replace(nodeToPatch, string.Empty).Replace("< ", "<");
                var nodeNameLenght = nodeToPatch.IndexOf(' ');
                if (nodeNameLenght == -1 || (nodeToPatch.IndexOf('>') < nodeToPatch.IndexOf(' ')))
                    nodeNameLenght = nodeToPatch.IndexOf('>');
                if (nodeNameLenght == -1)
                    nodeNameLenght = nodeToPatch.IndexOf('/');
                if (nodeNameLenght == -1)
                {
                    throw new Exception("Incorrect node. Check the node");
                }
                string xmlNodeToBePatched = nodeToPatch.Substring(1, nodeNameLenght - 1);
                XmlNodeList xmlNodeListSelectedByName = InitialXmlConfiguration.SelectNodes("//" + xmlNodeToBePatched);
                List<XmlNode> xmlNodeListSelectedByMatch = new List<XmlNode>();
                foreach (XmlNode xmlNode in xmlNodeListSelectedByName)
                {
                    if (xmlNode.OuterXml.Replace(" ", "").Replace(XmlNs, "").StartsWith(nodeToPatch.Replace(" ", "")) ==
                        true)
                    {
                        xmlNodeListSelectedByMatch.Add(xmlNode);
                    }
                }
                return xmlNodeListSelectedByMatch[0];
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public string GenerateNodeLocalPath(XmlNode xmlNode)
        {
            var xmlNodeAttributes = new StringBuilder("[");
            if (!string.IsNullOrEmpty(xmlNode.InnerText) && (xmlNode.InnerText == xmlNode.InnerXml))
            {
                xmlNodeAttributes.AppendFormat("{0}='{1}'", "text()", xmlNode.InnerText);
                if (xmlNode.Attributes.Count > 0)
                {
                    xmlNodeAttributes.Append(" and ");
                }
            }
            if (xmlNode.Attributes.Count == 0)
            {
                xmlNodeAttributes.Append("not(@*)");
            }
            else
            {
                for (int i = 0; i < xmlNode.Attributes.Count; i++)
                {
                    if (!xmlNode.Attributes[i].Name.Contains("xmlns"))
                    {
                        if (xmlNode.Attributes[i].Name.Contains("patch:") || xmlNode.Attributes[i].Name.Contains("set:"))
                            xmlNodeAttributes.AppendFormat("@{0}", xmlNode.Attributes[i].Name);
                        else
                            xmlNodeAttributes.AppendFormat("@{0}='{1}'", xmlNode.Attributes[i].Name,
                                xmlNode.Attributes[i].Value);
                        if (i + 1 < xmlNode.Attributes.Count && !xmlNode.Attributes[i + 1].Name.Contains("xmlns"))
                        {
                            xmlNodeAttributes.Append(" and ");
                        }
                    }
                }
            }

            xmlNodeAttributes.Append("]");
            return xmlNode.Name + xmlNodeAttributes;
        }

        public static string GenerateDirectNodePath(XmlNode xmlNode, string xmlNodePath)
        {
            XmlNode tempXmlNode = xmlNode;
            if (xmlNode.Attributes != null && xmlNode.Attributes.Count > 0 &&
                (xmlNode.ParentNode != null && xmlNode.ParentNode.Name != "#document"))
            {
                var xmlNodeAttributes = new StringBuilder("[");
                if (!string.IsNullOrEmpty(xmlNode.InnerText) && (xmlNode.InnerText == xmlNode.InnerXml))
                {
                    xmlNodeAttributes.AppendFormat("{0}='{1}'", "text()", xmlNode.InnerText);
                    if (xmlNode.Attributes.Count > 0)
                    {
                        xmlNodeAttributes.Append(" and ");
                    }
                }
                for (int i = 0; i < xmlNode.Attributes.Count; i++)
                {
                    if (!xmlNode.Attributes[i].Name.Contains("xmlns"))
                    {
                        if (xmlNode.Attributes[i].Name.Contains("patch:") || xmlNode.Attributes[i].Name.Contains("set:"))
                            xmlNodeAttributes.AppendFormat("@{0}", xmlNode.Attributes[i].Name);
                        else
                            xmlNodeAttributes.AppendFormat("@{0}='{1}'", xmlNode.Attributes[i].Name,
                                xmlNode.Attributes[i].Value);
                        if (i + 1 < xmlNode.Attributes.Count && !xmlNode.Attributes[i + 1].Name.Contains("xmlns"))
                        {
                            xmlNodeAttributes.Append(" and ");
                        }
                    }
                }

                xmlNodeAttributes.Append("]");
                xmlNodePath = xmlNode.Name + xmlNodeAttributes + xmlNodePath;
            }
            else
            {
                xmlNodePath = xmlNode.Name + xmlNodePath;
            }

            if (tempXmlNode.ParentNode != null && tempXmlNode.ParentNode.Name != "#document")
            {
                xmlNodePath = "/" + xmlNodePath;
                tempXmlNode = tempXmlNode.ParentNode;
                xmlNodePath = GenerateDirectNodePath(tempXmlNode, xmlNodePath);
            }
            else
            {
                return xmlNodePath;
            }
            return xmlNodePath;
        }

        public Stack<XmlNode> CollectParentNodesList(XmlNode xmlNode)
        {
            Stack<XmlNode> parentNodesList = new Stack<XmlNode>();
            while (xmlNode.ParentNode != null && xmlNode.ParentNode.Name != "#document")
            {
                parentNodesList.Push(xmlNode);
                xmlNode = xmlNode.ParentNode;
            }
            if (xmlNode.Attributes["xmlns:patch"] != null)
            {
                xmlNode.Attributes.Remove(xmlNode.Attributes["xmlns:patch"]);
            }
            parentNodesList.Push(xmlNode);
            return parentNodesList;
        }

        public void PatchNode(XmlNode xmlNodeToPatch, string patchType, string attributeToPatch, string valueToPatch)
        {
            if (patchType.Contains("patch:attribute") == true || patchType.Contains("patch:delete") == true)
            {
                AddSubNodeToNode(xmlNodeToPatch, patchType, attributeToPatch, valueToPatch);
            }
            else
            {
                AddAttributeToNode(xmlNodeToPatch, patchType, attributeToPatch, valueToPatch);
            }
        }

        protected void AddSubNodeToNode(XmlNode xmlNode, string patchType, string attributeToPatch, string valueToPatch)
        {
            XmlNamespaceManager xmlNamespaceManager = GenerateXmlNamespaceManager(ResultXmlConfiguration);
            XmlNode selectedXmlNode = ResultXmlConfiguration.SelectSingleNode("//" + GenerateDirectNodePath(xmlNode, ""));
            if (selectedXmlNode != null)
            {
                XmlNode newXmlNode = null;
                ResultXmlConfiguration.DocumentElement.SetAttribute("xmlns:patch", PatchNmsp);
                switch (patchType)
                {
                    case ("patch:delete"):
                        if (selectedXmlNode.SelectSingleNode("child::patch:delete", xmlNamespaceManager) == null)
                        {
                            newXmlNode = ResultXmlConfiguration.CreateNode(XmlNodeType.Element, patchType, PatchNmsp);
                            selectedXmlNode.AppendChild(newXmlNode);
                        }
                        break;
                    case ("patch:attribute"):
                        newXmlNode = ResultXmlConfiguration.CreateNode(XmlNodeType.Element, patchType, PatchNmsp);
                        XmlAttribute newXmlAttribute = ResultXmlConfiguration.CreateAttribute("name");
                        newXmlAttribute.Value = attributeToPatch;
                        newXmlNode.Attributes.Append(newXmlAttribute);
                        newXmlAttribute = ResultXmlConfiguration.CreateAttribute("value");
                        newXmlAttribute.Value = valueToPatch;
                        newXmlNode.Attributes.Append(newXmlAttribute);
                        newXmlNode.Attributes.Append(newXmlAttribute);
                        selectedXmlNode.AppendChild(newXmlNode);
                        break;
                }
            }
        }

        protected void AddAttributeToNode(XmlNode xmlNode, string patchType, string attributeToPatch,
            string valueToPatch)
        {
            XmlNamespaceManager xmlNamespaceManager = GenerateXmlNamespaceManager(ResultXmlConfiguration);
            XmlNode selectedXmlNode = ResultXmlConfiguration.SelectSingleNode("//" + GenerateDirectNodePath(xmlNode, ""));
            if (selectedXmlNode != null)
            {
                try
                {
                    if (patchType.Contains("set:"))
                        if (ResultXmlConfiguration.DocumentElement != null)
                            ResultXmlConfiguration.DocumentElement.SetAttribute("xmlns:set", SetNmsp);
                    if (patchType.Contains("patch:"))
                        if (ResultXmlConfiguration.DocumentElement != null)
                            ResultXmlConfiguration.DocumentElement.SetAttribute("xmlns:patch", PatchNmsp);

                    XmlAttribute newXmlAttribute = patchType.Contains("set:")
                        ? ResultXmlConfiguration.CreateAttribute(patchType + attributeToPatch, SetNmsp)
                        : ResultXmlConfiguration.CreateAttribute(patchType, PatchNmsp);
                    newXmlAttribute.Value = patchType.Contains("set:")
                        ? valueToPatch
                        : string.Format("*[@{0}='{1}']", attributeToPatch, xmlNode.Attributes[attributeToPatch].Value);
                    Debug.Assert(selectedXmlNode.Attributes != null, "selectedXmlNode.Attributes != null");
                    selectedXmlNode.Attributes.Append(newXmlAttribute);
                    if (!patchType.Contains("set:"))
                        selectedXmlNode.Attributes[attributeToPatch].Value = valueToPatch;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
        }

        #endregion

        #region IteractionsWithForm

        protected virtual void PopulateFormControlsWithInitialValues()
        {
            if (this.patchTypeControl.Items.Count <= 0)
                this.patchTypeControl.Items.AddRange(new ListItem[]
                {
                    new ListItem("patch:delete", "patchDelete"), new ListItem("patch:before", "patchBefore"),
                    new ListItem("patch:after", "patchAfter"), new ListItem("patch:instead", "patchInstead"),
                    new ListItem("patch:attribute", "patchAttribute"), new ListItem("set:", "patchSet")
                });
            environmentFoldersTreeView.Nodes.Clear();
            this.PopulateTreeView(new DirectoryInfo(Server.MapPath("~\\App_Config\\Include")), null);
            this.BindFormControlsEvents();
        }

        protected virtual void InitializeInitialRequestData()
        {
            if (ResultXmlConfiguration.DocumentElement == null)
            {
                XmlDeclaration xmlDeclaration = ResultXmlConfiguration.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = ResultXmlConfiguration.CreateElement("configuration", null);
                ResultXmlConfiguration.AppendChild(root);
                ResultXmlConfiguration.InsertBefore(xmlDeclaration, root);
            }
        }

        protected virtual void BindFormControlsEvents()
        {
            if (environmentFoldersTreeView.Attributes["onclick"] == null)
                environmentFoldersTreeView.Attributes.Add("onclick", "return nodeClicked(event);");
            if (patchTypeControl.Attributes["onclick"] == null)
                patchTypeControl.Attributes.Add("onclick", "return patchTypeChanged();");
            if (patchAttributeSetTextBox.Attributes["onkeyup"] == null)
                patchAttributeSetTextBox.Attributes.Add("onkeyup", "return patchAttributeSetTextBoxChanged();");
            if (nodeToPatchControl.Attributes["onkeyup"] == null)
                nodeToPatchControl.Attributes.Add("onkeyup", "return nodeToPatchControlTextChanged();");
        }

        protected virtual void LoadShowConfig()
        {
            InitialXmlConfiguration = Factory.GetConfiguration();
            XPathNavigator xPathNavigator = InitialXmlConfiguration.CreateNavigator();
            var commentsNavigator = xPathNavigator.SelectDescendants(XPathNodeType.Comment, false);
            while (commentsNavigator.MoveNext())
            {
                commentsNavigator.Current.DeleteSelf();
            }
            this.xmlOriginalControl.XPathNavigator = xPathNavigator;
        }

        protected virtual void LoadResultConfig()
        {
            XPathNavigator xPathNavigator = ResultXmlConfiguration.CreateNavigator();
            this.xmlPatchedControl.XPathNavigator = xPathNavigator;
        }

        private bool ValidateFormData(ref string message)
        {
            if (string.IsNullOrEmpty(this.nodeToPatchControl.Text))
            {
                message = "Please ensure that you selected the node that need to be patched.";
                return false;
            }
            if (string.IsNullOrEmpty(patchTypeControl.SelectedValue))
            {
                message = "Please ensure that you selected the patch type.";
                return false;
            }
            return true;
        }

        private void PopulateTreeView(DirectoryInfo directory, TreeNode treeNode)
        {

            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                TreeNode child = new TreeNode
                {
                    Text = dir.Name,
                    Value = dir.FullName,
                    SelectAction = TreeNodeSelectAction.Expand
                };
                if (treeNode == null)
                {
                    environmentFoldersTreeView.Nodes.Add(child);
                }
                else
                {
                    treeNode.ChildNodes.Add(child);
                }
                PopulateTreeView(dir, child);
            }
        }
        #endregion
        */
    }
}