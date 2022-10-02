// Iterate model annotations
foreach (var a in Model.GetAnnotations())
{
    if (
             // Check if model has at least one annotation that contains word "PBI"
             (a.Contains("PBI") || a.Contains("PowerBI"))
             // Check if model is loaded from a file of folder structure
             & Model.Database.ToString().Contains("file") == false
       )
    {
        "You are connected to PBI Desktop".Output();
        // Run code that can be executed against Power BI Desktop
        break;
    }
    else
    {
        "You are NOT connected to PBI Desktop".Output();
        // Execute code that is supported by XMLA-endpoint (like setting Avalability in MDX)
        break;
    }

} // end of loop