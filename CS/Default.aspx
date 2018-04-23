<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v16.2, Version=16.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ASPxGridView - Batch Edit - How to update group summaries with any grouping level</title>
    <script type="text/javascript">
        function OnEndEditing(s, e) {
            var summaryField = "C";
            var key = s.GetRowKey(e.visibleIndex);
            var originalValue = s.batchEditApi.GetCellValue(e.visibleIndex, summaryField);
            var newValue = e.rowValues[(s.GetColumnByField(summaryField).index)].value;
            var dif = newValue - originalValue;            
            var groupValues = s.cp_keys[key];
            var groups = s.cp_groups;
            updateSummaries(groupValues,groups, dif);
        }
        function updateSummaries(values, groups, dif) {
            if (!values) return;
            var name = "";
            var length = values.length;
            for (var i = 0; i < length; i++) {
                name += groups[i] + values[i];
                updateSummary(ASPxClientLabel.Cast(name),dif);
            }
        }
        function updateSummary(summary, dif) {
            if (!summary) return;
            var newValue = summary.cp_value + dif;
            summary.cp_value = newValue;
            summary.SetText(summary.cp_prefix + newValue);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <dx:ASPxGridView ID="ASPxGridView1" runat="server" KeyFieldName="A" 
                OnCustomJSProperties="ASPxGridView1_CustomJSProperties">
                <Settings ShowGroupFooter="VisibleIfExpanded" />
                <SettingsEditing Mode="Batch"></SettingsEditing>
                <Columns>
                    <dx:GridViewDataTextColumn FieldName="A" ReadOnly="true"></dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn FieldName="B" GroupIndex="0"></dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn FieldName="D" GroupIndex="1"></dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn FieldName="C">
                        <GroupFooterTemplate>
                            <dx:ASPxLabel ID="C_Sum" runat="server" EnableClientSideAPI="true" OnInit ="C_Sum_Init">
                            </dx:ASPxLabel>
                        </GroupFooterTemplate>
                    </dx:GridViewDataTextColumn>
                </Columns>
                <GroupSummary>
                    <dx:ASPxSummaryItem FieldName="C" ShowInGroupFooterColumn="C" SummaryType="Sum" />
                </GroupSummary>
                <SettingsPager Mode="ShowAllRecords"></SettingsPager>
                <ClientSideEvents BatchEditEndEditing="OnEndEditing" />
            </dx:ASPxGridView>
        </div>
    </form>
</body>
</html>
