using IssueTracker.Application.Common.Models;

namespace IssueTracker.UI.CommonHelpers
{
    public static class PaginationHelpers
    {
        public static (int lowestPaginationPage, int highestPaginationPage) CalculatePaginationButtons<T>(PaginatedList<T> paginatedList, int numberOfButtonsOnSide = 2)
        {
            var totalNumberOfButtons = numberOfButtonsOnSide * 2 + 1;
            var lowestPaginationPage = Math.Max(1, paginatedList.PageNumber - numberOfButtonsOnSide);
            var highestPaginationPage = Math.Min(paginatedList.PageNumber + numberOfButtonsOnSide, paginatedList.TotalPages);
            var calculatedPaginationGap = totalNumberOfButtons - (highestPaginationPage - lowestPaginationPage + 1);
            if (calculatedPaginationGap > 0)
            {
                if (lowestPaginationPage == 1)
                {
                    highestPaginationPage = Math.Min(highestPaginationPage + calculatedPaginationGap, paginatedList.TotalPages);
                }
                if (highestPaginationPage == paginatedList.TotalPages)
                {
                    lowestPaginationPage = Math.Max(1, lowestPaginationPage - calculatedPaginationGap);
                }
            }

            return (lowestPaginationPage, highestPaginationPage);
        }
    }
}
