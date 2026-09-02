using Relora.Items.Application.Queries;
using Relora.Items.Domain;
using Relora.Items.Domain.Enums;

using Microsoft.EntityFrameworkCore;

public static class LotQueryExtensions
{
    public static IQueryable<Lot> ApplyCatalogFilters(
        this IQueryable<Lot> query,
        GetLotsListQuery request,
        DateTime utcNow)
    {
        if (request.Genders.Length > 0)
        {
            query = query.Where(
                lot => request.Genders.Contains(lot.Gender));
        }

        if (request.Departments.Length > 0)
        {
            query = query.Where(
                lot => request.Departments.Contains(lot.Department));
        }

        if (request.Categories.Length > 0)
        {
            query = query.Where(
                lot => request.Categories.Contains(lot.Category));
        }

        if (request.CategoryIds.Length > 0)
        {
            query = query.Where(
                lot => request.CategoryIds.Contains(lot.CategoryId));
        }

        if (request.Sizes.Length > 0)
        {
            query = query.Where(
                lot => request.Sizes.Contains(lot.Size));
        }

        if (request.Conditions.Length > 0)
        {
            query = query.Where(
                lot => request.Conditions.Contains(lot.Condition));
        }

        if (request.Brands.Length > 0)
        {
            var brands = request.Brands
                .Where(brand => !string.IsNullOrWhiteSpace(brand))
                .Select(brand => brand.Trim().ToLower())
                .Distinct()
                .ToArray();

            query = query.Where(
                lot => brands.Contains(lot.Brand.ToLower()));
        }

        if (request.Countries.Length > 0)
        {
            var countries = request.Countries
                .Where(country => !string.IsNullOrWhiteSpace(country))
                .Select(country => country.Trim().ToLower())
                .Distinct()
                .ToArray();

            query = query.Where(
                lot =>
                    lot.Country != null &&
                    countries.Contains(lot.Country.ToLower()));
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(
                lot => lot.Price.Amount >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(
                lot => lot.Price.Amount <= request.MaxPrice.Value);
        }

        if (request.MaterialIds.Length > 0)
        {
            query = query.Where(lot =>
                lot.Materials.Any(material => request.MaterialIds.Contains(material.MaterialId)));
        }

        if (request.PrimaryColorIds.Length > 0)
        {
            query = query.Where(lot => request.PrimaryColorIds.Contains(lot.PrimaryColorId));
        }

        if (request.ProductionYearFrom.HasValue)
        {
            query = query.Where(lot =>
                lot.ProductionYear.HasValue &&
                lot.ProductionYear.Value >= request.ProductionYearFrom.Value);
        }

        if (request.ProductionYearTo.HasValue)
        {
            query = query.Where(lot =>
                lot.ProductionYear.HasValue &&
                lot.ProductionYear.Value <= request.ProductionYearTo.Value);
        }

        if (request.VintageOnly)
        {
            query = query.Where(lot => lot.IsVintage);
        }

        if (request.HasMeasurements)
        {
            query = query.Where(lot => lot.Measurements.Any());
        }

        if (request.HasProofOfOrigin)
        {
            query = query.Where(lot => lot.ProofDocuments.Any());
        }

        if (request.NewlyListed)
        {
            var listedAfter = utcNow.AddDays(-7);

            query = query.Where(
                lot => lot.CreatedAt >= listedAfter);
        }

        //if (request.EndingSoon)
        //{
        //    var endingBefore = utcNow.AddHours(24);

        //    query = query.Where(lot =>
        //        lot.Auction != null &&
        //        lot.Auction.EndsAtUtc > utcNow &&
        //        lot.Auction.EndsAtUtc <= endingBefore);
        //}

        return query;
    }

    public static IOrderedQueryable<Lot> ApplyCatalogSorting(
        this IQueryable<Lot> query,
        LotSort sort)
    {
        return sort switch
        {
            //LotSort.EndingSoon =>
            //    query
            //        .OrderBy(lot => lot.Auction!.EndsAtUtc)
            //        .ThenBy(lot => lot.Id),

            LotSort.NewlyListed =>
                query
                    .OrderByDescending(lot => lot.CreatedAt)
                    .ThenBy(lot => lot.Id),

            LotSort.PriceLowToHigh =>
                query
                    .OrderBy(lot => lot.Price.Amount)
                    .ThenBy(lot => lot.Id),

            LotSort.PriceHighToLow =>
                query
                    .OrderByDescending(lot => lot.Price.Amount)
                    .ThenBy(lot => lot.Id),

            //LotSort.MostBids =>
            //    query
            //        .OrderByDescending(lot => lot.Auction!.Bids.Count)
            //        .ThenBy(lot => lot.Id),

            _ =>
                query
                    .OrderByDescending(lot => lot.CreatedAt)
                    .ThenBy(lot => lot.Id)
        };
    }

    public static IQueryable<Lot> ApplySearch(
        this IQueryable<Lot> query,
        string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return query;
        }

        var terms = search
            .Trim()
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);

        foreach (var term in terms)
        {
            var pattern = $"%{term}%";

            query = query.Where(lot =>
                EF.Functions.ILike(lot.Title, pattern) ||
                EF.Functions.ILike(lot.Description, pattern) ||
                EF.Functions.ILike(lot.Brand, pattern) ||
                (
                    lot.Color != null &&
                    EF.Functions.ILike(lot.Color, pattern)
                ));
        }

        return query;
    }
}
