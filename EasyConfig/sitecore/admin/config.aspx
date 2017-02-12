<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="ConfigPatcher.aspx.cs" Inherits="Sitecore.ConfigPatcher.sitecore.admin.ConfigPatcher" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        
        function getSelectedNoteValue() {
            var selectedNode = document.getSelection().anchorNode;
            while (selectedNode.tagName != 'DIV') {
                selectedNode = selectedNode.parentNode;
            }
            console.info(selectedNode.innerText);
            alert(selectedNode);
            document.getElementById("nodeToPatchControl").value = selectedNode.innerText;
            var childNodes = selectedNode.childNodes;
            var attributesList = [];
            for (var i = 1; i < childNodes.length; i++) {
                if (childNodes[i].className == 't' && childNodes[i].innerText.indexOf('patch:source') == -1) {
                    attributesList.push(childNodes[i].innerText);
                }
            }
            addAttributesToDropdownControl(attributesList);
        }
        //function addAttributesToDropdownControl(attributesList) {
        //    var sel = document.getElementById('patchAttributeDropdownControl');
        //    while (sel.options.length > 0) {
        //        sel.remove(0);
        //    }
        //    for (var i = 0; i < attributesList.length; i++) {
        //        var opt = document.createElement('option');
        //        opt.value = attributesList[i];
        //        opt.text = attributesList[i];
        //        sel.appendChild(opt);
        //    }
        //}
        //function nodeClicked(event) {
        //    var obj = event.srcElement || event.target;
        //    if (obj.innerHTML.indexOf('<tbody>') == -1)
        //        document.getElementById("configFilePathControl").value = "/App_Config/Include/" +  obj.innerHTML;
        //}
        //function patchTypeChanged(){
        //    var list = document.getElementById("patchTypeControl");
        //    var inputs = list.getElementsByTagName("input");
        //    var selected;
        //    for (var i = 0; i < inputs.length; i++) {
        //        if (inputs[i].checked) {
        //            selected = inputs[i];
        //            break;
        //        }
        //    }
        //    if (selected) {
        //        if (selected.value == "patchDelete") {
        //            document.getElementById("patchAttributeDropdownControl").disabled = true;
        //            document.getElementById("patchValueControl").disabled = true;
        //            document.getElementById("patchAttributeSetTextBox").disabled = true;
        //        } else if (selected.value == "patchSet") {
        //            document.getElementById("patchAttributeDropdownControl").disabled = false;
        //            document.getElementById("patchValueControl").disabled = false;
        //            document.getElementById("patchAttributeSetTextBox").disabled = false;
        //        } else {
        //            document.getElementById("patchAttributeDropdownControl").disabled = false;
        //            document.getElementById("patchValueControl").disabled = false;
        //            document.getElementById("patchAttributeSetTextBox").disabled = true;
        //        }
        //    }
        //}
        //function patchAttributeSetTextBoxChanged() {
        //    var value = document.getElementById("patchAttributeSetTextBox").value;
        //    if (value != '') {
        //        document.getElementById("patchAttributeDropdownControl").disabled = true;
        //    } else {
        //        document.getElementById("patchAttributeDropdownControl").disabled = false;
        //    }
        //}
        //function nodeToPatchControlTextChanged() {
        //    var value = document.getElementById("nodeToPatchControl").value;
        //    if (value.replace(/\s+/g, '') == '') {
        //        var sel = document.getElementById('patchAttributeDropdownControl');
        //        while (sel.options.length > 0) {
        //            sel.remove(0);
        //        }
        //    }
        //}
    </script>
    <style>
        /*#form1 {
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
        }
        #mainUserInterectionForm span {
            font-weight: bold;
        }
        #mainUserInterectionForm input {
            width: 90%;
            border-width: 1px;
            border-style: ridge;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
        }
        #configFileManipulationsContainer input {
            width: 90%;
            border-width: 1px;
            border-style: ridge;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 12px;
        }
        #applyPatchButton {
            float: left;
        }
        #updatePanelInfoArea {
            margin-left: 100px; 
        }
        #errorLable {
            font-size: 16px;
            color: red;
        }
        #xmlOriginalContainer {
            border-width: 1px;
            border-style: ridge;
            width:auto;
            height:400px;
            overflow: auto; 
        }
        #updatePanelXmlArea {
            border-width: 1px;
            border-style: ridge;
            width:auto;
            height:200px;
            overflow: auto; 
        }
        #resetResultXmlButton {
            margin-left: 10px;
        }*/
    </style>
   
</head>
<body>
    <form id="form1" runat="server" autocomplete="off">
        <%-- <asp:ScriptManager ID="configPatcherScriptManager" runat="server" EnablePageMethods="true">
         </asp:ScriptManager>--%>
        <div id="xmlOriginalContainer" onclick="getSelectedNoteValue()">
            <asp:Xml runat="server" id="xmlOriginalControl" TransformSource="/sitecore/admin/ConfigPatcher.xslt" ></asp:Xml>
        </div>
       <%-- <br/>
        <div id="mainUserInterectionForm">
        <div id="nodeToPatchContainer">
            <asp:Label runat="server" id="nodeToPatchLable">Node to be patched:</asp:Label>
            <br/>
            <asp:TextBox runat="server" id="nodeToPatchControl" >
                
            </asp:TextBox>
        </div>
            <br/>
        <div id="patchTypeContainer">
            <asp:Label runat="server" id="patchTypeLabel">Patch type to be applyed:</asp:Label>
            <br/>
            <asp:RadioButtonList RepeatDirection="Horizontal" ID="patchTypeControl" runat="server"/>
        </div>
            <br/>
        <div id="patchAttributeContainer">
            <asp:Label runat="server" id="patchAttributeLabel">Attribute to patch/bind patching:</asp:Label>
            <br/>
            <asp:UpdatePanel runat="server" id="PatchDropDownUpdatePanel" UpdateMode="Conditional">
                <ContentTemplate>
                   <asp:DropDownList ID="patchAttributeDropdownControl" runat="server"/>
               </ContentTemplate>
                <Triggers>
                   <asp:AsyncPostBackTrigger ControlID="nodeToPatchControl" EventName="TextChanged" />
               </Triggers>
            </asp:UpdatePanel>
        </div>
            <br/>
            <div id="patchSetContainer">
                <asp:Label runat="server" id="patchAttributeSetLabel">Add attribute with the name:</asp:Label>
            <br/>
                <asp:TextBox runat="server" id="patchAttributeSetTextBox"></asp:TextBox>
            </div>
        <br/>
        <div id="patchValueContainer">
            <asp:Label runat="server" id="patchValueLable">Patched node new value:</asp:Label>
            <br/>
            <asp:TextBox runat="server" id="patchValueControl"></asp:TextBox>
        </div>
            </div>
        <br/>
        <div id="updatePanelContainer">
            <asp:Button runat="server" id="applyPatchButton" OnClick="applyPatchButton_OnClick" Text="Apply patch"/>
            <asp:Button runat="server" id="resetResultXmlButton" OnClick="resetResultXmlButton_OnClick" Text="Reset patched xml"/>
            <asp:UpdatePanel runat="server" id="updatePanelInfoArea" UpdateMode="Conditional">
                <ContentTemplate>
                   <asp:Label runat="server" id="errorLable" ></asp:Label>
               </ContentTemplate>
                <Triggers>
                   <asp:AsyncPostBackTrigger ControlID="applyPatchButton" EventName="Click" />
               </Triggers>
            </asp:UpdatePanel>
            </div>
            <br/><br/>
            <asp:UpdatePanel id="updatePanelXmlArea" runat="server" UpdateMode="Conditional">
               <ContentTemplate>
                   <asp:Xml runat="server" id="xmlPatchedControl" TransformSource="/sitecore/admin/ConfigPatcher.xslt"></asp:Xml>
               </ContentTemplate>
               <Triggers>
                   <asp:AsyncPostBackTrigger ControlID="applyPatchButton" EventName="Click" />
                   <asp:AsyncPostBackTrigger ControlID="resetResultXmlButton" EventName="Click" />
               </Triggers>
           </asp:UpdatePanel>
        <br/>
        <div id="configFileManipulationsContainer">
           <asp:TreeView runat="server" id="environmentFoldersTreeView" ImageSet="Simple">
               <HoverNodeStyle Font-Underline="True" ForeColor="#6666AA" />
               <NodeStyle Font-Names="Tahoma" Font-Size="8pt" ForeColor="Black" HorizontalPadding="2px" NodeSpacing="0px" VerticalPadding="2px"></NodeStyle>
               <ParentNodeStyle Font-Bold="False" />
               <SelectedNodeStyle BackColor="#B5B5B5" Font-Underline="False" HorizontalPadding="0px" VerticalPadding="0px" />
           </asp:TreeView>
            <br/>
            <asp:Label runat="server" id="configFilePathLabel">Patched file path:</asp:Label>
            <br/>
            <asp:TextBox runat="server" id="configFilePathControl" Enabled="False">/App_Config/Include/</asp:TextBox>
            <br/>
            </div>
        <asp:TextBox runat="server" id="configFileNameControl"></asp:TextBox>
        <br/>
            <asp:Button runat="server" id="createPatchFileButton" OnClick="createPatchFileButton_OnClick" Text="Create/Update patch file"/>
            <br/>--%>
    </form>
</body>
</html>