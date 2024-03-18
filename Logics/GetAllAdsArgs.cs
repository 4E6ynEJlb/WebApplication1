using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace LogicsLib
{
    
    public class GetAllAdsArgs
    {
        public Logics.SortCriteria Criterion { get; set; }
        public bool IsASC { get; set; }
        public string? KeyWord { get; set; }
        public int? RatingLow { get; set; }
        public int? RatingHigh { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
