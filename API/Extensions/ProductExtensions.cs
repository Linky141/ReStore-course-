namespace API.Extensions;

public static class ProductExtensions
{
    public static IQueryable<API.Entities.Product> Sort(this IQueryable<API.Entities.Product> query, string orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return query.OrderBy(p => p.Name);
        query = orderBy switch
        {
            "price" => query.OrderBy(p => p.Price),
            "priceDesc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Name)
        };
        return query;
    }

    public static IQueryable<API.Entities.Product> Search(this IQueryable<API.Entities.Product> query, string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return query;
        var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
        return query.Where(p => p.Name.ToLower().Contains(lowerCaseSearchTerm));
    }

    public static IQueryable<API.Entities.Product> Filter(this IQueryable<API.Entities.Product> query, string brands, string types)
    {
        var brandList = new List<string>();
        var typeList = new List<string>();

        if (!string.IsNullOrEmpty(brands))
            brandList.AddRange(brands.ToLower().Split(",").ToList());
        if (!string.IsNullOrEmpty(types))
            typeList.AddRange(types.ToLower().Split(",").ToList());

        query = query.Where(p => brandList.Count == 0 || brandList.Contains(p.Brand.ToLower()));
        query = query.Where(p => typeList.Count == 0 || typeList.Contains(p.Type.ToLower()));
        return query;
    }
}