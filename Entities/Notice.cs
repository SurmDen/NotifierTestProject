namespace NotifierTestProject.Entities
{
    public class Notice
    {
        public Guid Id { get; set; }

        public string NotifierName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public List<User> Users { get; set; }
    }
}
