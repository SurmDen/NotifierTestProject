namespace NotifierTestProject.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public long UserNumber { get; set; }

        public string UserName { get; set; } = string.Empty;

        public List<Notice> Notices { get; set; }
    }
}
