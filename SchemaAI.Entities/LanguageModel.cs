using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchemaAI.Entities
{
    public class LanguageModel : BaseEntity
    {
        public LanguageModel()
        {
            
        }

        public Guid LanguageModelGuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string ModelIdentifier { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int MaxTokens { get; set; } = 0;
        public decimal Temperature { get; set; } = 0;
        public decimal TopP { get; set; } = 0;
        public decimal FrequencyPenalty { get; set; } = 0;
        public decimal PresencePenalty { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
