/*
    This code example was provided by Daniel Otykier 

    For more detail see the this link: https://github.com/TabularEditor/TabularEditor/issues/1010

*/


#r "Microsoft.AnalysisServices.Core.dll"

var server = Model.Database.TOMDatabase.Server as Microsoft.AnalysisServices.Tabular.Server;
var isLoadedFromFile = server == null;
var isPbiDesktop = server != null && server.ServerLocation == Microsoft.AnalysisServices.ServerLocation.OnPremise
    && server.CompatibilityMode == Microsoft.AnalysisServices.CompatibilityMode.PowerBI;

if (isLoadedFromFile)
    Info("Metadata loaded from file");
else if (isPbiDesktop)
    Info("Connected to PBI Desktop");
else
    Info("Not connected to PBI Desktop");