namespace Flight.Stages
{
    using Flight.Database;
    using Flight.Logging;
    using Flight.Providers;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class MigrationStageBase : StageBase
    {
        private readonly IScriptProvider scriptProvider;

        protected MigrationStageBase(IScriptProvider scriptProvider)
        {
            this.scriptProvider = scriptProvider;
        }

        protected abstract Task ApplyAsync(DbConnection connection, IEnumerable<IScript> scripts, IBatchManager batchManager, IAuditor auditor, CancellationToken cancellationToken = default);

        public override async Task MigrateAsync(DbConnection connection, IBatchManager batchManager, IAuditor auditor, CancellationToken cancellationToken = default)
        {
            if (auditor == null)
                throw new System.ArgumentNullException(nameof(auditor));

            var scripts = scriptProvider.GetScripts();
            var changeSet = await auditor.CreateChangeSetAsync(connection, scripts, cancellationToken).ConfigureAwait(false);

            Log.Info($"Change set contains {changeSet.Count()} script(s)");

            if (changeSet.Any())
                await ApplyAsync(connection, changeSet, batchManager, auditor, cancellationToken).ConfigureAwait(false);
        }
    }
}