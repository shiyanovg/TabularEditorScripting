foreach (var c in Selected.Table.Columns)
{
    if (c.UsedInRelationships.Any())
    {
        c.IsHidden = true;
        c.DisplayFolder = "RelationshipColumns";
    }
}
