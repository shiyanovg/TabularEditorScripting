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

var measure_default_value = Convert.ToInt64(
                                            Math.Round(
                                                Convert.ToDouble(
                                                        value_end / 2
                                                        )
                                                , 1, MidpointRounding.ToEven
                                            )
                                        );


string TableName = "My Table";
string TableExpression = "GENERATESERIES({0},{1},{2})";
string TableExpression_formatted = String.Format(TableExpression
    , value_start, value_end, value_step);


// Add New Calculated Table to the Model (What-If Parameter)
Table WhatIfParameterCalcTable = Model.AddCalculatedTable(TableName, TableExpression);

// Name column same as table
Column WhatIfColumn = WhatIfParameterCalcTable.Columns.First().Name = TableName;

// Create DAX Expression for What-If Parameter measure
string MeasureDaxExpression = String.Format(
            "SELECTEDVALUE({0},{1})"
            , WhatIfColumn.DaxObjectFullName()
            , measure_default_value
            );
string MeasureName = String.Format("{0} Value", TableName);


// Add measure for What-If parameter
WhatIfParameterCalcTable.AddMeasure(
    MeasureName
    , MeasureDaxExpression
    ).FormatString = "#,##0";

