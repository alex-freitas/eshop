namespace WebApp.Models
{
    public class Owner
    {
        public Owner(int id, string name)
        {
            Id = id;
            Name = name;
        }

        int _Id;

        public virtual int Id
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }

        public string Name { get; set; } 
    }
}
