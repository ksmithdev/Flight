﻿using Flight.Auditing;
using Flight.Database;
using Flight.Providers;
using Flight.Stages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Flight
{
    public class MigrationBuilder
    {
        private readonly CompositeScriptProvider initializationScriptProvider;
        private readonly IList<IStage> stages;
        private IAuditLog? auditor;
        private IBatchManager? batchManager;
        private IConnectionFactory? connectionFactory;

        public MigrationBuilder()
        {
            initializationScriptProvider = new CompositeScriptProvider();
            stages = new List<IStage>();
        }

        public MigrationBuilder AddStage(IStage migrationStage)
        {
            stages.Add(migrationStage);

            return this;
        }

        public MigrationBuilder AddStage(Func<IStage> stageFactory)
        {
            if (stageFactory == null)
                throw new ArgumentNullException(nameof(stageFactory));

            stages.Add(stageFactory());

            return this;
        }

        public IMigration Build(ILoggerFactory loggerFactory)
        {
            if (connectionFactory == null)
                throw new InvalidOperationException("cannot build migration without setting connection factory");
            if (batchManager == null)
                throw new InvalidOperationException("cannot build migration without setting batch manager");
            if (auditor == null)
                throw new InvalidOperationException("cannot build migration without setting auditor");

            var migrationStages = new List<IStage>() { new InitializationStage(initializationScriptProvider) };
            foreach (var stage in stages)
                migrationStages.Add(stage);

            foreach (var stage in migrationStages)
                stage.Initialize(loggerFactory);

            return new Migration(
                connectionFactory,
                batchManager,
                auditor,
                migrationStages,
                loggerFactory);
        }

        public MigrationBuilder InitializeDatabase(IScriptProvider scriptProvider)
        {
            initializationScriptProvider.AddScriptProvider(scriptProvider);

            return this;
        }

        public void SetAuditor(IAuditLog auditor) => this.auditor = auditor ?? throw new ArgumentNullException(nameof(auditor));

        public void SetBatchManager(IBatchManager batchManager) => this.batchManager = batchManager ?? throw new ArgumentNullException(nameof(batchManager));

        public void SetConnectionFactory(IConnectionFactory connectionFactory) => this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }
}