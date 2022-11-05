foreach (var c in Model.AllColumns)
{
    if (c.UsedInRelationships.Any())
    {
        c.IsHidden = true;
        // Comment out line below if you do not want to create folders
        c.DisplayFolder = "RelationshipColumns";
}

