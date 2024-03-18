using Swashbuckle.AspNetCore.Filters;

namespace LogicsLib
{
    public class GetAllAdsArgsExample : IExamplesProvider<GetAllAdsArgs>
    {
        public GetAllAdsArgs GetExamples()
        {
            return new GetAllAdsArgs
            {
                Criterion = Logics.SortCriteria.CreationDate,
                IsASC = true,
                KeyWord = null,
                Page = 1,
                PageSize = 10,
                RatingHigh = null,
                RatingLow = null
            };
        }
    }
}
