namespace Reports.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string CompanyName { get; set; }

        public virtual ICollection<ContactInfo> ContactInfos { get; set; }
    }
}
