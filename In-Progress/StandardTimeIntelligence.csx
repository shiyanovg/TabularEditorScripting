#r "Microsoft.AnalysisServices.Core.dll"
#r "Microsoft.VisualBasic"
using Microsoft.VisualBasic;


var PreSelectedMeasuresList = Selected.Measures;
int LengthOfList = PreSelectedMeasuresList.Count;
if (LengthOfList <= 0)
{
    Error("Please select one or more measures");
    return;
}
else
{
    Info("OK");

    // Validate date table
    Table DateTable =
   Model.Tables.Where(t => t.DataCategory == "Time").First();
    Column DateColumn = DateTable.Columns
    .Where(
         c => c.DataType == DataType.DateTime
         && c.IsKey == true
    ).First();
    bool ValidDateTable = DateColumn != null;

    if (ValidDateTable == false)
    {
        Error("Date table is not valid! Key column is not DateTime");
        return;
    }
    else
    {

        //           DateColumn.Output();
        Column YearColumn = DateColumn.Table.Columns
        .Where(
             c => c.Name.StartsWith("Year")
             ).First();
        Column MonthColumn =
     DateColumn.Table.Columns
.Where(
c => c.Name == "Month"
     ).First();
        Column MonthNumberColumn = MonthColumn.SortByColumn.First();
        Column MonthNumberColumn = MonthColumn.SortByColumn;

        Column QuarterColumn =
               DateColumn.Table.Columns
          .Where(
          c => c.Name == "Quarter"
               ).First();
        Column QuarterNumberColumn = QuarterColumn.SortByColumn;

        Column MonthYearColumn =
           DateColumn.Table.Columns
           .Where(
               c => c.Name.IndexOf("Month") > -1
               && c.Name.IndexOf("Year") > -1
           && (
              c.DataType == DataType.Int64
              ||
              c.DataType == DataType.Decimal
               ||
               c.DataType == DataType.Double
               ) == false
             ).First();

        Column MonthYearNumberColumn =
        MonthYearColumn.SortByColumn;

        Column QuarterYearColumn =
        DateColumn.Table.Columns
        .Where(
            c => c.Name.IndexOf("Quarter") > -1
            && c.Name.IndexOf("Year") > -1
           //&& c.SortByColumn != null
           && (
           c.DataType == DataType.Int64
           ||
           c.DataType == DataType.Decimal
            ||
            c.DataType == DataType.Double
            ) == false
           ).First();

        Column QuarterYearNumberColumn =
       QuarterYearColumn.SortByColumn;

        Column SalesDateColumn =
        DateColumn.UsedInRelationships.First().FromColumn;

        //      MonthYearNumberColumn.Output();
        /*  
        Column DatesWithSales = DateColumn.Table.CalculatedColumns
          .Where(
            c => c.Expression != null
            ).First();
          */

        Column DatesWithSales = DateColumn.Table.Columns.Where(c => c.DataType == DataType.Boolean).First();
        bool HasDatesWithSales = DatesWithSales != null;

        DatesWithSales.Output();

    }


}