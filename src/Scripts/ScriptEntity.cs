
using System;

namespace ShieldTech.MongoDbMigrations.Scripts
{
    public class ScriptEntity
    {
        public const string CollectionName = "_mongoDbMigrationsHistory";
        public ScriptEntity(Guid id, int sequenceVersion, string name, DateTime dateApplied)
        {
            Id = id;
            Name = name;
            SequenceVersion = sequenceVersion;
            DateApplied = dateApplied;
        }

        public ScriptEntity(int sequenceVersion, string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            SequenceVersion = sequenceVersion;
            DateApplied = DateTime.UtcNow;
        }
        
        public Guid Id { get; set; }
        public int SequenceVersion { get; set; }
        public string Name { get; set; }
        public DateTime DateApplied { get; }
    }
}