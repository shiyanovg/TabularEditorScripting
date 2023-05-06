#r "Microsoft.AnalysisServices.Core.dll"
#r "Microsoft.VisualBasic"
using Microsoft.VisualBasic;

int value_start_default = 1;

int value_start_input = 1;

int value_start = 1;

int value_end_default = 20;

int value_end = 1;

int value_step_default = 1;

int value_step = 1;


string TableName = "My Table";
string TableExpression = "GENERATESERIES({0},{1},{2})";
string TableExpression_formatted = String.Format(TableExpression
    , value_start, value_end, value_step);


// Add New Calculated Table to the Model (What-If Parameter)
Model.AddCalculatedTable(TableName, TableExpression);
