using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace LogicsLib
{
    
    public class GetAllAdsArgs
    {
        public Logics.SortCriteria Criterion;
        public bool IsASC;
        public string? KeyWord;
        public int? RatingLow;
        public int? RatingHigh;
        public int Page = 1;
        public int PageSize = 10; 
    }
}
