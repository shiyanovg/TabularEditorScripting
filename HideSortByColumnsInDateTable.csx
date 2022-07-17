foreach (var t in Model.Tables)
{
    var CurrentTableName = t.Name;
    if (CurrentTableName == "Date" || CurrentTableName == "Calendar")
    {
        foreach (var c in t.Columns)
        {
            if (c.UsedInSortBy.Any())
            {
                c.IsHidden = true;
            }
        }
    }
}
