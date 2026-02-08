namespace SchemaAI.Entities
{
    public interface ICurrentUser
    {
        Guid UserGuid { get; set; }
        string UserName { get; set; }
        string Phone { get; set; }
        Guid TenantGuid { get; set; }
        bool IsAuthenticated { get; set; }
    }


}
