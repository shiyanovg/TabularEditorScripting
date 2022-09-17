/*
 
 
 */

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


TopNTableExpression.Output();



/* Additinal scripting - In progress */
#r "Microsoft.VisualBasic"
using Microsoft.VisualBasic;


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
    "SELECTEDVALUE(" + ParameterColumnName + ","
    + String.Format("{0})", ParameterDefault)
    ;



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

//Model.AddCalculatedTable(ParameterTableName, ParameterTableDax) ;


//TopNTable = Model.AddCalculatedTable(ParameterTableName, ParameterTableDax) ;

//ParameterTableDax.Output();


/* Generate Measures for "TopN + Others" pattern */

string RankingMeasureName = "Ranking";
string RankingMeasureDax =
    "IF( ISINSCOPE( " + TopNTableName.DaxObjectFullName + StartColumn

    ;


Selected.Column.DaxObjectName.Output();

/*-------- Add Mes ---------*/

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





