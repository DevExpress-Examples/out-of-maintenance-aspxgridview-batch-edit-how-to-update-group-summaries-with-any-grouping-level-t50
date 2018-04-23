Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DevExpress.Web

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        ASPxGridView1.DataSource = GridData
        ASPxGridView1.DataBind()
        If Not IsPostBack Then
            ASPxGridView1.ExpandAll()
        End If
    End Sub

    Protected ReadOnly Property GridData() As List(Of GridDataItem)
        Get
            Dim key = "34FAA431-CF79-4869-9488-93F6AAE81263"
            If (Not IsPostBack) OrElse Session(key) Is Nothing Then
                Session(key) = Enumerable.Range(0, 10).Select(Function(i) New GridDataItem With {.A = i, .B = i Mod 2, .C = 10*i, .D = i Mod 3}).ToList()
            End If
            Return DirectCast(Session(key), List(Of GridDataItem))
        End Get
    End Property
    Protected Sub C_Sum_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim lbl = TryCast(sender, ASPxLabel)
        Dim container = TryCast(lbl.NamingContainer, GridViewGroupFooterCellTemplateContainer)
        Dim groupIndex = container.VisibleIndex
        lbl.ClientInstanceName = GetName(container)
        lbl.Text = container.Text
        lbl.JSProperties("cp_prefix") = "SUM="
        lbl.JSProperties("cp_value") = container.Grid.GetGroupSummaryValue(groupIndex, container.Grid.GroupSummary("C"))
    End Sub

    Private Function GetName(ByVal container As GridViewGroupFooterCellTemplateContainer) As String
        Dim groupedColumns = container.Grid.GetGroupedColumns().Where(Function(g) g.GroupIndex <= container.GroupedColumn.GroupIndex).OrderBy(Function(g) g.GroupIndex).Select(Function(g) g.FieldName).ToArray()
        Dim data = container.Grid.GetRowValues(container.VisibleIndex, groupedColumns)
        Dim groupValues = If(groupedColumns.Length = 1, New Object() { data }, TryCast(data, Object()))
        Dim name = ""
        For i As Integer = 0 To groupedColumns.Length - 1
            name &= groupedColumns(i) + groupValues(i)
        Next i
        Return name
    End Function

    Protected Sub ASPxGridView1_CustomJSProperties(ByVal sender As Object, ByVal e As ASPxGridViewClientJSPropertiesEventArgs)
        Dim grid = TryCast(sender, ASPxGridView)
        Dim keySumPairs = New Hashtable()
        Dim groups = grid.GetGroupedColumns().OrderBy(Function(g) g.GroupIndex).Select(Function(g) g.FieldName).ToArray()
        For i As Integer = grid.VisibleStartIndex To grid.VisibleRowCount - 1
            If grid.IsGroupRow(i) Then
                Continue For
            End If
            Dim key = grid.GetRowValues(i, grid.KeyFieldName)
            Dim groupValue = grid.GetRowValues(i, groups)
            keySumPairs.Add(key, groupValue)
        Next i
        e.Properties.Add("cp_keys", keySumPairs)
        e.Properties.Add("cp_groups", groups)
    End Sub
End Class