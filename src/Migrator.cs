using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ShieldTech.MongoDbMigrations.Scripts;

namespace ShieldTech.MongoDbMigrations
{
    /// <summary>
    /// Class used to update and revert mongoDB database state, this get all classes that extends from ScriptBase and use these for apply and revert scripts.
    /// </summary>
    public class Migrator
    {
        private List<ScriptBase> _scripts = new List<ScriptBase>();
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<ScriptEntity> _collection;

        /// <summary>
        /// </summary>
        /// <param name="mongoDbConnectionString">The mongoDB connection string</param>
        /// <param name="dataBaseName">The database you want to versionate, will be create one collection called _mongoDbMigrationsHistory</param>
        public Migrator(string mongoDbConnectionString, string dataBaseName)
        {
            LoadScripts();
            new ScriptEntityMap();
            var client = new MongoClient(mongoDbConnectionString);
            var session = client.StartSession();
            _mongoDatabase = session.Client.GetDatabase(dataBaseName);
            _collection = _mongoDatabase.GetCollection<ScriptEntity>(ScriptEntity.CollectionName);
        }

        private void LoadScripts()
        {
            _scripts.AddRange(AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.IsSubclassOf(typeof(ScriptBase)) && !x.IsInterface && !x.IsAbstract)
                .Select(x => (ScriptBase) Activator.CreateInstance(x))
                .OrderBy(x => x.SequenceVersion)
            );
        }

        private async Task CreateScriptIndexAsync()
        {
            await _collection.Indexes.CreateOneAsync(Builders<ScriptEntity>.IndexKeys.Ascending(_ => _.SequenceVersion));
        }

        /// <summary>
        /// 
        /// Configure the database to the desired version, this method can increase or decrease the version of the database, it depends on the version parameter and the current version of the database
        /// </summary>
        /// <param name="version">The version you wish the database to be, if default -1 will update to the last version available in your scripts</param>
        public async Task SetDatabaseToVersionAsync(int version=-1)
        {
            var lastVersionScript = (await _collection.Find(FilterDefinition<ScriptEntity>.Empty)
                .SortByDescending(x => x.SequenceVersion).FirstOrDefaultAsync())?.SequenceVersion ?? 0;
            
            Console.WriteLine($"Beginning MongoDB script migrations");
            if (version > -1 && version < lastVersionScript)
                await RevertToVersionAsync(version, lastVersionScript);
            else
                await UpdateToVersionAsync(version, lastVersionScript);
            Console.WriteLine("All scripts applied with succefull");
        }

        private async Task UpdateToVersionAsync(int version, int lastVersionScript)
        {
            foreach (var script in _scripts.Where(x => version == -1 || x.SequenceVersion <= version))
            {
                Console.WriteLine($"Applying script {script.Name} version {script.SequenceVersion}");
                if (script.SequenceVersion <= lastVersionScript)
                    continue;

                if (script.SequenceVersion > lastVersionScript + 1)
                    throw new ArgumentException($"The last applied script was {lastVersionScript} and the newest script is {script.SequenceVersion} the versions should be sequential");

                var result = await script.ApplyAsync(_mongoDatabase);
                if (!result)
                    throw new Exception($"Fail on execute script {script.Name} aborting application");

                var scriptEntity = new ScriptEntity(script.SequenceVersion, script.Name);
                await _collection.InsertOneAsync(scriptEntity);
                lastVersionScript++;
                Console.WriteLine($"Script {script.Name} applied with succefull");
            }
        }
        
        private async Task RevertToVersionAsync(int version, int lastVersionScript)
        {
            Console.WriteLine($"Beginning revert MongoDB scripts migrations\n Total scripts to revert: {lastVersionScript - version}");
            while (version < lastVersionScript)
            {
                var script = _scripts.First(x => x.SequenceVersion == lastVersionScript);
                Console.WriteLine($"Reverting script {script.Name} version {script.SequenceVersion}");
                var result = await script.RevertAsync(_mongoDatabase);
                if (!result)
                    throw new Exception($"Fail on revert script {script.Name} aborting application");

                await _collection.DeleteOneAsync(s => s.SequenceVersion == lastVersionScript);
                lastVersionScript--;
                Console.WriteLine($"Script {script.Name} reverted with succefull");
            }
            Console.WriteLine($"All scripts reverted with succefull, current dataBase version {version}");
        }
    }
}