using System;
using System.Collections.Generic;

namespace SmartSearch.Models;

public class SearchResult
{
    public List<Business> Businesses { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasMore { get; set; }
    public string SearchQuery { get; set; } = string.Empty;
    public List<string> SuggestedCategories { get; set; } = new();
    public List<string> PopularSearches { get; set; } = new();
    public Dictionary<string, int> CategoryCounts { get; set; } = new();
    public List<FilterOption> AvailableFilters { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class FilterOption
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<string> Values { get; set; } = new();
    public bool IsSelected { get; set; }
    public string? SelectedValue { get; set; }
} 