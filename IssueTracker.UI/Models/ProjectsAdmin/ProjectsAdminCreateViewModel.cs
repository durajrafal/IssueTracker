using System.ComponentModel.DataAnnotations;

namespace IssueTracker.UI.Models.ProjectsAdmin
{
    public class ProjectsAdminCreateViewModel
    {
        [Required]
        [RegularExpression("^[^:/?#[\\]@!$&\\()*+;=]*$",ErrorMessage = "Title must not containy any of the following characters => :/?#[]@!$&\\()*+;=")]
        public string Title { get; set; }
    }
}
