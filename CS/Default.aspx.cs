using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;

public partial class _Default : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {
        ASPxGridView1.DataSource = GridData; 
        ASPxGridView1.DataBind();
        if(!IsPostBack)
            ASPxGridView1.ExpandAll();
    }

    protected List<GridDataItem> GridData {
        get {
            var key = "34FAA431-CF79-4869-9488-93F6AAE81263";
            if(!IsPostBack || Session[key] == null)
                Session[key] = Enumerable.Range(0, 10).Select(i => new GridDataItem {
                    A = i,
                    B = i % 2,
                    C = 10*i,
                    D = i % 3,
                }).ToList();
            return (List<GridDataItem>)Session[key];
        }
    }
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
}