#r "System.Drawing"

using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;


using (Form prompt = new Form())
{
    Font formFont = new Font("Segoe UI", 11);


    // Prompt config
    prompt.AutoSize = true;
    prompt.MinimumSize = new Size(485, 400);
    prompt.Text = "What-If Parameter";
    prompt.StartPosition = FormStartPosition.CenterScreen;

    // Set the AutoScaleMode property to Dpi
    prompt.AutoScaleMode = AutoScaleMode.Dpi;

    // Name: label
    Label nameLabel = new Label()
    {
        Text = "Name:"
        ,
        Left = 20
        ,
        Top = 20
        ,
        Width = 200
        ,
        Font = formFont
        ,
        Height = 25
    };


    // Textbox for inputing paramter name 
    TextBox nameBox = new TextBox()
    {
        Left = nameLabel.Location.X + 4
           ,
        Top = nameLabel.Location.Y + nameLabel.Height // 25
           ,
        Width = 200
              ,
        Height = 20
              ,
        Text = "Parameter"
           ,
        Font = formFont
    };



    Label minLabel = new Label()
    {
        Text = "Minimum:"
    ,
        Left = 20
    ,
        Top = nameBox.Location.Y + nameBox.Height + 20
    ,
        Width = 200
    ,
        Font = formFont
     ,
        Height = 25
    };

    TextBox minimumBox = new TextBox()
    {
        Left = minLabel.Location.X + 4
         ,
        Top = minLabel.Location.Y + minLabel.Height // 25
         ,
        Width = 200
         ,
        Height = 25
            ,
        Text = "0"
         ,
        Font = formFont
    };

    Label maxLabel = new Label()
    {
        Text = "Maximum:"
        ,
        Left = minimumBox.Location.X + minimumBox.Width + 20
        ,
        Top = minLabel.Location.Y
    ,
        Width = 200
    ,
        Font = formFont
     ,
        Height = 25
    };

    TextBox maximumBox = new TextBox()
    {
        Left = maxLabel.Location.X + 4
           ,
        Top = minimumBox.Location.Y
         ,
        Width = 200
            ,
        Height = 20
            ,
        Text = "20"
         ,
        Font = formFont
    };



    Label IncrementLabel = new Label()
    {
        Text = "Increment:"
     ,
        Left = 20
     ,
        Top = minimumBox.Location.Y + minimumBox.Height + 20
     ,
        Width = 200
     ,
        Font = formFont
      ,
        Height = 25
    };


    TextBox IncrementBox = new TextBox()
    {
        Left = IncrementLabel.Location.X + 4
         ,
        Top = IncrementLabel.Location.Y + IncrementLabel.Height // 25
         ,
        Width = 200
         ,
        Height = 25
         ,
        Text = "1"
         ,
        Font = formFont
    };



    Label DefaultLabel = new Label()
    {
        Text = "Default:"
     ,
        Left = IncrementLabel.Location.X + IncrementLabel.Width + 20
     ,
        Top = IncrementLabel.Location.Y
     ,
        Width = 200
     ,
        Font = formFont
      ,
        Height = 25
    };


    TextBox DefaultBox = new TextBox()
    {
        Left = DefaultLabel.Location.X + 4
        ,
        Top = DefaultLabel.Location.Y + DefaultLabel.Height // 25
        ,
        Width = 200
        ,
        Height = 25
        ,
        Text = ""
        ,
        Font = formFont
    };


    // OK Button
    Button okButton = new Button() { Text = "OK", Left = 20, Width = 75, Top = 400 - 100 };
    okButton.MinimumSize = new Size(75, 25);
    okButton.AutoSize = true;
    okButton.Font = formFont;

    // Cancel Button
    Button cancelButton = new Button() { Text = "Cancel", Left = okButton.Location.X + okButton.Width + 10, Top = okButton.Location.Y };
    cancelButton.MinimumSize = new Size(75, 25);
    cancelButton.AutoSize = true;
    cancelButton.Font = formFont;

    string TableName = "";
    string value_start_input = "";
    string value_end_input = "";
    string value_step_input = "";
    string value_default_input = "";


    // Button actions
    okButton.Click += (sender, e) => {
        prompt.DialogResult = DialogResult.OK;
        value_start_input = minimumBox.Text;
        value_end_input = maximumBox.Text;
        value_step_input = IncrementBox.Text;
        value_default_input = DefaultBox.Text;
        TableName = nameBox.Text;
    };
    cancelButton.Click += (sender, e) => { prompt.DialogResult = DialogResult.Cancel; };




    prompt.AcceptButton = okButton;
    prompt.CancelButton = cancelButton;

    prompt.Controls.Add(nameBox);


    prompt.Controls.Add(nameLabel);

    prompt.Controls.Add(minLabel);
    prompt.Controls.Add(minimumBox);

    prompt.Controls.Add(maxLabel);
    prompt.Controls.Add(maximumBox);

    prompt.Controls.Add(IncrementLabel);
    prompt.Controls.Add(IncrementBox);


    prompt.Controls.Add(DefaultLabel);
    prompt.Controls.Add(DefaultBox);


    prompt.Controls.Add(okButton);
    prompt.Controls.Add(cancelButton);

    // The user clicked OK, so perform the find-and-replace logic
    if (prompt.ShowDialog() == DialogResult.OK)
    {
        string MeasureDaxExpression = "";
        string TableExpression = "GENERATESERIES({0},{1},{2})";
        string TableExpression_formatted = String.Format(
                        TableExpression
                        , value_start_input
                        , value_end_input
                        , value_step_input
            );

        // Add New Calculated Table to the Model (What-If Parameter)
        CalculatedTable WhatIfParameterCalcTable = Model.AddCalculatedTable(TableName, TableExpression);

        // Name column same as table
        Column WhatIfColumn = WhatIfParameterCalcTable.AddCalculatedTableColumn(
        TableName
            , "[Value]" 
            , null
            , DataType.Int64
            );

        // Create DAX Expression for What-If Parameter measure
        if (value_default_input != "")
        {
            MeasureDaxExpression = String.Format(
                    "SELECTEDVALUE({0},{1})"
                    , WhatIfColumn.DaxObjectFullName
                    , value_default_input
                    );

        }
        else
        {
            MeasureDaxExpression = String.Format(
                       "SELECTEDVALUE({0})"
                       , WhatIfColumn.DaxObjectFullName
               );


        };

        string MeasureName = String.Format("{0} Value", TableName);
        // Add measure for What-If parameter
        Measure WhatIfValueMeasure = WhatIfParameterCalcTable.AddMeasure(
           MeasureName
           , MeasureDaxExpression
           );
        WhatIfValueMeasure.FormatString = "#,0";
        WhatIfValueMeasure.FormatDax();

        // Fix BPA Issues
        WhatIfParameterCalcTable.SetAnnotation(
                "BestPracticeAnalyzer_IgnoreRules"
                , "{\"RuleIDs\":[\"ENSURE_TABLES_HAVE_RELATIONSHIPS\",\"REDUCE_USAGE_OF_CALCULATED_TABLES\"]}"
        );
        WhatIfParameterCalcTable.Description = "What-If Parameter by Tabular Editor";
        WhatIfColumn.Description = "What-If Column by Tabular Editor";

        WhatIfValueMeasure.Description = "What-If Value by Tabular Editor";
        WhatIfColumn.SetExtendedProperty(
                 "ParameterMetadata"
                 , "{\"version\":0}"
                 , ExtendedPropertyType.Json
                 );


    }
    else
    {
        return;
    }
}