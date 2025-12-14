using System.ComponentModel.DataAnnotations;

namespace PWA.Auth.Models
{
    public class ApplicationInfo
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public string? IconUrl { get; set; }
        public string Route { get; set; } = "/";
        public string Category { get; set; } = "General";
        public int DisplayOrder{get;set;}=0;
        public bool IsExternalRoute{get;set;}
    }
}