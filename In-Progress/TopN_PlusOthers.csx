/*
 This script was inspired by the article from SQLBI https://sql.bi/695263
 
 */


//Info("Select TopN What-If Parameter");

Table StartTableTopN = Model.SelectTable(null, "Select TopN What-If Parameter");
bool CheckForTopN = StartTableTopN == null;
if (CheckForTopN)
{
    Error("No TopN What-If");
    return;
}
else
{
    Info("OK");

    var ListOfFactTables = new List<Table>();
    var ListOfDimentionTables = new List<Table>();

    foreach (var r in Model.Relationships)
    {
        Table ManyColTable = r.FromColumn.Table;
        // Table ManyColTable = r.FromColumn.Table;
        bool notInListFact = ListOfFactTables.Contains(ManyColTable);
        if (notInListFact == false)
        {
            ListOfFactTables.Add(ManyColTable);
        }
    }

    foreach (var r in Model.Relationships)
    {
        Table OneColTable = r.ToColumn.Table;
        bool NotDate = r.ToColumn.DataType == DataType.DateTime;
        bool notInListDim = ListOfDimentionTables.Contains(OneColTable);
        if (notInListDim == false && NotDate == false)
        {
            ListOfDimentionTables.Add(OneColTable);
        }
    }

    // Set Annotation for object StartTable
    StartTableTopN.SetAnnotation(
        "TopNScript_StartTableTopN_ShiyanovG"
        , "TopNScript_StartTableTopN_ShiyanovG"
        );
    Measure MeasureTopN = StartTableTopN.SelectMeasure(null, "Select TopN measure");
    var MeasureTopNReference = MeasureTopN.DaxObjectName;
    Table StartTable = ListOfDimentionTables.SelectTable(null, "Select the table to implement TopN + Others for");
    string StartTableName = StartTable.DaxObjectFullName;
    // Set Annotation for object StartTable
    StartTable.SetAnnotation(
        "TopNScript_StartTable_ShiyanovG"
        , "TopNScript_StartTable_ShiyanovG"
        );

    Column StartColumn =
    StartTable.Columns
    .Where(c => c.UsedInRelationships.Any() == false)
    .SelectColumn(null, "Select the column to use for TopN + Others");


    Table FactTable = ListOfFactTables.SelectTable(null, "Where is your main Fact Table");
    Measure RankingMeasureReference = FactTable.SelectMeasure(null, "What is you base measure for the pattern");
    string RankingMeasureReferenceName = RankingMeasureReference.DaxObjectFullName;

    string TopNTableExpression =
    "UNION( "
    + "ALLNOBLANKROW( " + StartColumn.DaxObjectFullName + "),"
    + "\n"
    + "{ \"Others\" }"
    + "\n )";

    string TopNTableName = StartTable.Name + " Names";
    string FullPowerBITableExpression =
    TopNTableName + " = "
    + "\n"
    + TopNTableExpression;

    Info("Copy to Clipboard the following code and create new Calucated Table");
    FullPowerBITableExpression.Output();

    Table ReferenceTable = Model.Tables[TopNTableName];
    ReferenceTable.SetAnnotation(
        "TopNScript_ReferenceTable_ShiyanovG"
        , "TopNScript_ReferenceTable_ShiyanovG"
        );

    string ReferenceTableName = ReferenceTable.DaxObjectFullName;
    string ReferenceColumnName = ReferenceTable.Columns.First().DaxObjectFullName;
    string RankingMeasureName = "Ranking";
    string Others = " \"Others\" ";
    string RankingMeasureDax =
      @"
IF (
    ISINSCOPE ( {1} ),
    VAR ProductsToRank = [TopN Value] 
    VAR SalesAmount = {3} 
    VAR IsOtherSelected =
        SELECTEDVALUE ( {1} ) = {0}  
    RETURN
        IF(
            IsOtherSelected,
            -- Rank for Others
            ProductsToRank + 1,
            -- Rank for regular products
            IF (
                SalesAmount > 0,
                VAR VisibleProducts =
                    CALCULATETABLE(VALUES({1}), ALLSELECTED({2}))
                VAR Ranking =
                    RANKX(VisibleProducts, {3}, SalesAmount)
                RETURN
                    IF (Ranking > 0 && Ranking <= ProductsToRank, Ranking )
            )
        ) 
 ) 
 ";
    string RankingMeasureDaxFormatted = string.Format(
     RankingMeasureDax, Others, ReferenceColumnName,
     ReferenceTableName, RankingMeasureReferenceName
     );
    // Add Ranking measure to the table with Formatted code
    ReferenceTable.AddMeasure(RankingMeasureName, RankingMeasureDaxFormatted, null);
    string VisibleRowMeasureDax =
       @"
VAR Ranking = [Ranking] 
VAR TopNValue = [TopN Value]
VAR Result =  
    IF( 
        NOT ISBLANK(Ranking), 
        (Ranking <= TopNValue) - (Ranking = TopNValue + 1) 
    ) 
RETURN  Result ";


    // Add Visible Row measure to the table 
    ReferenceTable.AddMeasure("Visible Row", VisibleRowMeasureDax, null);
    // Generate code for desired measure (for example Sales Amount)
    string AddColsColumn = "@SalesAmount";
    string q = "\"";
    string AmountNAMeasureDax = @"
 VAR SalesOfAll =
 CALCULATE ( {6}, REMOVEFILTERS ( {5} ) )
RETURN
    IF (
        NOT ISINSCOPE ( {3} ),
        -- Calculation for a group of products 
        SalesOfAll,
        -- Calculation for one product name
        VAR ProductsToRank = [TopN Value]
        VAR SalesOfCurrentProduct = {6}
        VAR IsOtherSelected =
            SELECTEDVALUE ( {3} ) = {0}
        RETURN
            IF(
                NOT IsOtherSelected,
                -- Calculation for a regular product
                SalesOfCurrentProduct,
                -- Calculation for Others
                VAR VisibleProducts =
                    CALCULATETABLE(
                        VALUES({4}),
                        REMOVEFILTERS({3})
                    )
                VAR ProductsWithSales =
                    ADDCOLUMNS(VisibleProducts, {1}{2}{1} , [Sales Amount])
                VAR FilterTopProducts =
                TOPN(ProductsToRank, ProductsWithSales, [{2}])
                VAR FilterOthers =
                    EXCEPT(ProductsWithSales, FilterTopProducts)
                VAR SalesOthers =
                    CALCULATE(
                        {6},
                        FilterOthers,
                        REMOVEFILTERS ( {3} )
                    )
                RETURN
                    SalesOthers
            )
            )"
                ;
    string AmountNAMeasureDaxFormatted =
        string.Format(
            AmountNAMeasureDax
            , Others  // 0
            , q // 1
            , AddColsColumn // 2
            , ReferenceColumnName // 3
            , StartTableName // 4
            , ReferenceTableName // 5
            , RankingMeasureReferenceName // 6
            );
    string AmountNAMeasureName = RankingMeasureReference.Name + " NA";

    // Add AmountNA measure to the table 
    ReferenceTable.AddMeasure(
        AmountNAMeasureName
        , AmountNAMeasureDaxFormatted
        , null
        );

    // Format all created measures
    ReferenceTable.Measures.FormatDax();
}


//==================================================================================//

/* Additinal scripting - In progress */
#r "Microsoft.VisualBasic"
using Microsoft.VisualBasic;


string ConfigInpuBoxText =
    "1 - Measures"
    + "\n"
    + "2 - Calculation Group"
    ;
int ConfigDefaultInput = 1;
Interaction.InputBox(
ConfigInpuBoxText,
   "Choose configuration of execution",
   ConfigDefaultInput.ToString(),
   740,
   400);

string ParameterTableNameInpuBoxTitle = "What-If Parameter Name";
string ParameterTableNameInpuBoxText = "Provide a name for your What-If Parameter";
string ParameterTableNameDefaultInput = "Top N";


/* Generate a What-If Parameter (Power BI feature)*/
string ParameterTableName =
    Interaction.InputBox(
        ParameterTableNameInpuBoxText,
        ParameterTableNameInpuBoxTitle,
        ParameterTableNameDefaultInput,
        740,
        400);

int ParameterStart = 1;
int ParameterEnd = 20;
int ParameterStep = 1;
int ParameterDefault = 5;

string ParameterTableDax =
    String.Format("SELECTCOLUMNS( GENERATESERIES({0},{1},{2}),  \"" + ParameterTableName + "\", [Value] )",
    ParameterStart,
    ParameterEnd,
    ParameterStep
    );

string ParameterColumnName =
    "'" + ParameterTableName + "'"
    + "[" + ParameterTableName + "]"
    ;


string ParameterMeasureName = ParameterTableName + " Value";
string ParameterMeasureDax =
        string.Format(
            "SELECTEDVALUE ({0}), {1}",
            ParameterColumnName,
            ParameterDefault
        );

Table ParamemerCalcTable = Model.AddCalculatedTable(ParameterTableName, ParameterTableDax);
Measure ParameterMeasure = ParamemerCalcTable.AddMeasure(
    ParameterMeasureName,
    ParameterMeasureDax,
    ParameterColumnName.DisplayFolder
    );
ParameterMeasure.FormatDax();

ParameterTableName.Output();

/*---------- Addition 2 ---------------*/


Info("Select the table to implement TopN + Others for");

Table StartTable = Model.SelectTable();
Column StartColumn = StartTable.SelectColumn();

// StartColumn.DaxObjectFullName.Output();


string TopNTableExpression =
"UNION( "
+ "ALLNOBLANKROW( " + StartColumn.DaxObjectFullName + "),"
+ "\n"
+ "{ \"Others\" }"
+ "\n )";


string TopNTableName = StartTable.Name + " Names";

string extendedpropertyname = "ParameterMetadata";
string extendedpropertyvalue = "{\"version\":0}";
string extendedpropertytype = "JSON";


/* Generate a What-If Parameter (Power BI feature)*/
string ParameterTableName = "Top N";

int ParameterStart = 1;
int ParameterEnd = 20;
int ParameterStep = 1;
int ParameterDefault = 5;

string ParameterTableDax =
    String.Format("GENERATESERIES({0},{1},{2})",
    ParameterStart,
    ParameterEnd,
    ParameterStep
    );

string ParameterMeasureDax = 
        string.Format(
            "SELECTEDVALUE ({0}), {1}", 
            ParameterColumnName, 
            ParameterDefault
        );

Table ParamemerCalcTable = Model.AddCalculatedTable(ParameterTableName, ParameterTableDax) ;
Measure ParamemerCalcTable.AddMeasure();

//TopNTable = Model.AddCalculatedTable(ParameterTableName, ParameterTableDax) ;

//ParameterTableDax.Output();


/* Generate Measures for "TopN + Others" pattern */

string RankingMeasureName = "Ranking";
string RankingMeasureDax =
    "IF( ISINSCOPE( " + TopNTableName.DaxObjectFullName + StartColumn

    ;


Selected.Column.DaxObjectName.Output();

/*-------- Add Measures ---------*/

/* Generate Measures for "TopN + Others" pattern */




Table TopNTable = Selected.Table;

string TopNTableName = "'" + TopNTable.Name + " Names'";
Column StartColumn = (Model.Tables["Provider"].Columns["Supplier"] as DataColumn);


string RankingMeasureName = "Ranking";
string RankingMeasureDax =
    "IF( ISINSCOPE( " + TopNTableName + StartColumn.DaxObjectName + "), " + "\n"
    + "VAR ProductsToRank = [TopN Value]" + "\n"
    + "VAR SalesAmount = [Sales Amount]" + "\n"
    + "VAR IsOtherSelected = SELECTEDVALUE ( 'Product Names'[Product Name] ) = \"Others\" " + "\n"
    + "RETURN " + "\n"
    + "IF( IsOtherSelected," + "\n"
    + "ProductsToRank + 1 " + "\n"
    + "IF( SalesAmount > 0," + "\n"
    + "VAR VisibleProducts = CALCULATETABLE ( VALUES ( 'Product' ), ALLSELECTED ( 'Product Names' ) )" + "\n"
    + "VAR Ranking = RANKX ( VisibleProducts, [Sales Amount], SalesAmount )" + "\n"
    + "RETURN" + "\n"
    + "IF ( Ranking > 0 && Ranking <= ProductsToRank, Ranking )" + "\n"
    + ")))"

    ;

FormatDax("Test:= " + RankingMeasureDax).Output();


/*----- Check if Column is Used In Relationships*/
(Model.Tables["Supplier"] as Table).Columns
.Where(c => c.UsedInRelationships.Any() == false)
.Output();


// Set Annotation for object
Selected.Table.SetAnnotation("Test Name", "Test Value")

    // Search model tables for annotations
foreach (var t in Model.Tables
    .Where(t => t.GetAnnotation("Test") == "Test Value")
    .ToList())
{
    t.Output();
}

// Get ReferencedBy Tables
Selected.Table
.ReferencedBy
.Where(r => r.ObjectType == ObjectType.Table)
.First()
.Output();




Model
.AllColumns
.Where(c => c.UsedInRelationships.Any() == true)
.ToList()
.Output()
;

