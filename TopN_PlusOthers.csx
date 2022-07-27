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


