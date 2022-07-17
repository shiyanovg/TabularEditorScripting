foreach (var c in Model.AllColumns)
{
    var ur = c.UsedInRelationships.Any();
    if (ur == true)
    {
        c.DisplayFolder = "RelationshipColumns";
    }
    else if (
        // Check if a column is referenced by Any measure of your model
        c.ReferencedBy.AllMeasures.Any()
        )
    {
        c.DisplayFolder = "MetricColumns";
    }
}

