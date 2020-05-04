using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ShieldTech.MongoDbMigrations.Scripts
{
    /// <summary>
    /// Class that represents the script that will be applied
    /// </summary>
    public abstract class ScriptBase
    {
        protected ScriptBase(int sequenceVersion, string name)
        {
            if(sequenceVersion < 1)
                throw new ArgumentException("The version should be greather than 0");
            SequenceVersion = sequenceVersion;
            Name = name;
        }

        public int SequenceVersion { get; }
        public string Name { get; }
        
        /// <summary>
        /// You should implement here every thing you wanna do, using mongoDbDriver, like create indexes, insert documents, remove documents, etc
        /// </summary>
        /// <returns>Returns true if all commands are successful or false if there is a failure, this will be used to stop the application of the scripts</returns>
        public abstract Task<bool> ApplyAsync(IMongoDatabase mongoDatabase);
        
        /// <summary>
        /// Implement here all commands necessary for revert all alterations that you do in Apply method
        /// </summary>
        /// <returns>Returns true if all commands are successful or false if there is a failure, this will be used to stop the application of the scripts</returns>
        public abstract Task<bool> RevertAsync(IMongoDatabase mongoDatabase);
    }
}