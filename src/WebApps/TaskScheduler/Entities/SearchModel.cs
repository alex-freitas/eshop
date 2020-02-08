namespace TaskScheduler.Entities
{
    public class SearchModel
    {
        public string NamePart { get; set; }

        public string JobType { get; set; }

        public bool? Enabled { get; set; }

        public int MaxRows { get; set; }

        public int StartRow { get; set; }

        public string SortBy { get; set; }
    }
}
