namespace IssueTracker.UI.CommonHelpers
{
    public static class CssHelpers
    {
        public static string AddClassConditionally(bool condition, string cssClass)
        {
            return condition ? cssClass : "";
        }
    }
}
