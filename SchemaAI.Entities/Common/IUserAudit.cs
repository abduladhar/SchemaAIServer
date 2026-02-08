namespace SchemaAI.Entities.Common
{
    public interface IUserAudit
    {
        Guid CreatedByUserGuid { get; set; }
        string CreatedByUserName { get; set; }
        DateTime CreatedOn { get; set; }

        Guid? ModifiedByUserGuid { get; set; }
        string? ModifiedByUserName { get; set; }
        DateTime? ModifiedOn { get; set; }
    }

}
