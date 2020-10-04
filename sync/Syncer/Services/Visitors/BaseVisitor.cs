﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Syncer.Configuration;
using Syncer.Contracts;

namespace Syncer.Services.Visitors
{
    public class PreProcessInformation
    {
        public TableConfiguration TableConfiguration { get; set; }

        public bool CanContinue { get; set; }

        public static PreProcessInformation CannotContinue => new PreProcessInformation{ CanContinue = false };
    }

    public abstract class BaseVisitor<T> where T: IBinLogEventVisitor
    {
        protected List<string> HandledTables { get; }

        protected ILogger<T> Logger { get; }

        protected BaseVisitor(IOptions<DatabaseConfiguration> databaseConfiguration, ILogger<T> logger)
        {
            HandledTables = databaseConfiguration.Value.HandleTables;
            Logger = logger;

            if (!HandledTables.Any())
            {
                Logger.LogDebug("Handled tables configuration is empty so handling all tables");
            }
        }

        protected PreProcessInformation PreProcess(long tableId, ExecutionContext executionContext)
        {
            if (!HandledTables.Any())
            {
                Logger.LogDebug("Handled tables configuration is empty so handling all tables");
                return PreProcessInformation.CannotContinue;
            }

            var tableConfiguration =
                executionContext.OrdinalConfiguration.Tables.SingleOrDefault(table => table.Id == tableId);

            if (tableConfiguration == null)
            {
                Logger.LogError($"Cannot handle table with id {tableId} -- not contained in the configuration ");
                return PreProcessInformation.CannotContinue;
            }

            return new PreProcessInformation
            {
                CanContinue = true,
                TableConfiguration = tableConfiguration
            };
        }
    }
}
