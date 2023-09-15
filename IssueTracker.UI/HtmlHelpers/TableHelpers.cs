using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace IssueTracker.UI.HtmlHelpers
{
    public static class TableHelpers
    {
        public static IHtmlContent TablesForMemberManagement(this IHtmlHelper helper)
        {
            var sb = new StringBuilder();
            sb.Append(GetUserTableWithFilter("data-members-table", "data-members-filter-input"));
            sb.Append(GetUserTableWithFilter("data-other-users-table", "data-other-user-filter-input"));
            return new HtmlString(sb.ToString());
        }

        private static string GetUserTableWithFilter(string dataTable, string dataFilter)
        {
            return $"""
                <div class="col-lg-6">
                    <div class="d-flex justify-content-between align-items-center">
                        <div>
                            <h4 class="m-0 me-2">Members</h4>
                
                        </div>
                        <div class="my-2" style="width:20rem;">
                            <input type="search" placeholder="Search member.." class="form-control" {dataFilter}/>
                        </div>
                    </div>
                    <div class="table-responsive" style="max-height:30rem;">
                        <table class="table table-dark table-striped" id="users-table" {dataTable}>
                            <thead class="table-primary" style="position:sticky; top: -1px;">
                                <tr>
                                    <th scope="col">Email</th>
                                    <th scope="col">FirstName</th>
                                    <th scope="col">LastName</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                    </div>
                </div>
                """;
        }
    }
}
