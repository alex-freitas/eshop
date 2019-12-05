namespace WebApp.Models
{
    public class BlogStatus : Enumeration
    {
        public readonly static BlogStatus On = new BlogStatus(1, nameof(On).ToLowerInvariant());
        public readonly static BlogStatus Off = new BlogStatus(2, nameof(Off).ToLowerInvariant());
        
        public BlogStatus(int id, string name) : base(id, name) { }
    }
}
