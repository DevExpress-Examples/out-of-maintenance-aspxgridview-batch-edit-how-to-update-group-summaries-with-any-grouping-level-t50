# ASPxGridView - Batch Edit - How to update group summaries with any grouping level


In Batch Edit mode, ASPxGridView data changes are passed to the server only after a user clicks the "Save changes" button. As a result, ASPxGridView summaries are not updated as summaries are updated on the server. It is possible to overcome this behavior by implementing a custom summaries update on the client side. <br><br>1. To update group summaries, it is necessary to know to which group a changed row belongs. Every group is uniquely identified by grouped columns' FieldName and grouped rows' values. This info is not available on the client side by default. Pass this info to the client side by handling the ASPxGridView.CustomJSProperties event.:<br>


```cs
protected void ASPxGridView1_CustomJSProperties(object sender, ASPxGridViewClientJSPropertiesEventArgs e) {
    var grid = sender as ASPxGridView;
    var keySumPairs = new Hashtable();
    var groups = grid.GetGroupedColumns()
        .OrderBy(g => g.GroupIndex)
        .Select(g => g.FieldName)
        .ToArray();
    for (int i = grid.VisibleStartIndex; i < grid.VisibleRowCount; i++) {
        if (grid.IsGroupRow(i)) continue;
        var key = grid.GetRowValues(i, grid.KeyFieldName);
        var groupValue = grid.GetRowValues(i, groups);
        keySumPairs.Add(key, groupValue);
    }
    e.Properties.Add("cp_keys", keySumPairs);
    e.Properties.Add("cp_groups", groups);
}

```


<br>2. To be able to change group summary values on the client side, implement your own GroupFooterTemplate and place ASPxLabel in it:<br>


```aspx
<dx:GridViewDataTextColumn FieldName="C">
    <GroupFooterTemplate>
        <dx:ASPxLabel ID="C_Sum" runat="server" EnableClientSideAPI="true" OnInit ="C_Sum_Init">
        </dx:ASPxLabel>
    </GroupFooterTemplate>
</dx:GridViewDataTextColumn>

```


<br>3. Every ASPxLabel should be uniquely identified to address its text on the client. Handle the ASPxLabel.Init event to assign a unique ClientInstanceName property value. In the sample, the ClientInstanceName property is formed as a string of the group column FieldName property plus a group row value. If a group is nested, the string starts with a root group name-value pair(GroupIndex=0) and then a nested name-value pair; for example, FieldName0SecondFieldName0. Using ASPxLabel JSProperties, pass information about the group summary prefix and the first group summary value that is calculated on the first page load:<br>


```cs
protected void C_Sum_Init(object sender, EventArgs e) {
    var lbl = sender as ASPxLabel;
    var container = lbl.NamingContainer as GridViewGroupFooterCellTemplateContainer;
    var groupIndex = container.VisibleIndex;
    lbl.ClientInstanceName = GetName(container);
    lbl.Text = container.Text;
    lbl.JSProperties["cp_prefix"] = "SUM=";
    lbl.JSProperties["cp_value"] = container.Grid.GetGroupSummaryValue(groupIndex, container.Grid.GroupSummary["C"]);
}

private string GetName(GridViewGroupFooterCellTemplateContainer container) {
    var groupedColumns = container.Grid.GetGroupedColumns()
        .Where(g => g.GroupIndex <= container.GroupedColumn.GroupIndex)
        .OrderBy(g => g.GroupIndex)
        .Select(g => g.FieldName)
        .ToArray();
    var data = container.Grid.GetRowValues(container.VisibleIndex, groupedColumns);
    var groupValues = groupedColumns.Length == 1 ? new object[] { data } : data as object[];
    var name = "";
    for (int i = 0; i < groupedColumns.Length; i++) {
        name += groupedColumns[i] + groupValues[i];
    }
    return name;
}

```


<p><br>4. On the client side, handle the BatchEditEndEditing event. This event is raised on every ASPxGridView value changing. Using information passed through the ASPxGridView.CustomJSProperties event, get group rows' values for a changed grid row by its key. This info and group columns' names will allow you to form a unique ASPxLabel name and call the ASPxClientLabel.SetText method to change the group summary value:</p>


```js
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
```


<p><strong><br>See Also:</strong></p>
<p><a href="https://www.devexpress.com/Support/Center/p/T114539">ASPxGridView - Batch Edit - How to calculate values on the fly</a> <br><a href="https://www.devexpress.com/Support/Center/p/T116925">ASPxGridView - Batch Edit - How to calculate unbound column and total summary values on the fly</a> <br><br><strong>ASP.NET MVC Example:</strong><br><a href="https://www.devexpress.com/Support/Center/p/T137186">GridView - How to update total summaries on the client side in Batch Edit mode</a></p>

<br/>


