foreach (var c in Model.AllColumns)
{
    if (c.UsedInRelationships.Any())
    {
        c.IsHidden = true;
        c.DisplayFolder = "RelationshipColumns";
    }
}

