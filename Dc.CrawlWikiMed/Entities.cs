using System;

namespace Dc.CrawlWikiMed
{
    public class Field
    {
        public int UniqueKey { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
    }

    public class DiseasesDetails
    {
        public Field[] Fields { get; set; }
        public string EnglishName { get; set; }
        public bool IsLifeThreatened { get; set; }
        public string[] MedicationIds { get; set; }
        public string[] SpecialtyIds { get; set; }
        public string SourceId { get; set; }
        public string Name { get; set; }
        public string FeaturedImageUrl { get; set; }
        public int Version { get; set; }
        public bool IsPublished { get; set; }
        public string Id { get; set; }
        public string Code { get; set; }
        public string ValueToSearch { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? PostDate { get; set; }
        public DateTime? LastModified { get; set; }
        public int ProjectType { get; set; }
        public int ItemState { get; set; }
    }

    public class DiseasesDes
    {
        public float MatchScore { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ValueToSearch { get; set; }
        public DateTime? PostDate { get; set; }
    }
}
