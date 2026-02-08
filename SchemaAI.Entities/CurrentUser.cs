namespace SchemaAI.Entities
{
    public class CurrentUser : ICurrentUser
    {
        public Guid UserGuid { get; set; } = Guid.Empty;
        public string UserName { get; set; } = string.Empty;
        public Guid TenantGuid { get; set; } = Guid.Empty;
        public bool IsAuthenticated { get; set; }
        public string Phone { get ; set ; } = string.Empty;
    }


}
