namespace LionFrame.Domain
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public Address Address { get; set; }
    }
}
