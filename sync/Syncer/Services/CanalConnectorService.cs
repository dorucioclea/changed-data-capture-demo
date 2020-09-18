using System;
using System.Diagnostics.CodeAnalysis;
using CanalSharp.Client.Impl;
using CanalSharp.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Syncer.Configuration;
using Syncer.Contracts;

namespace Syncer.Services
{
    public class CanalConnectorService : ICanalConnector
    {
        private readonly ILogger<CanalConnectorService> _logger;
        private readonly IOptions<CanalConfiguration> _canalConfiguration;
        private readonly CanalSharp.Client.ICanalConnector _canalConnector;

        public CanalConnectorService(ILogger<CanalConnectorService> logger, IOptions<CanalConfiguration> canalConfiguration)
        {
            _logger = logger;
            _canalConfiguration = canalConfiguration;
            _canalConnector = ProvideCanalClient(_canalConfiguration.Value);
        }

        public void Connect()
        {
            _canalConnector.Connect();
            
            //
            // MySQL parses the table and Perl supports regex.
            //     Use comma(,) for multiple regex, double backslash(\\) for escape characters.
            //     Examples:
            // 1.All the tables: .*or.*\\..*
            //     2.All the tables in canal scheme: canal\\..*
            //     3.Tables starting with canal in canal scheme: canal\\.canal.*
            //     4.Access a table in canal scheme: canal.test1
            // 5.A combination of multiple rules: canal\\..*, mysql.test1, mysql.test2(with comma as delimeter)

            // All the tables for now
            //
            _canalConnector.Subscribe(".*\\..*");
        }

        public Message Get(int batchSize = 1024)
        {
            // Batch size of 1024
            //
            var message = _canalConnector.Get(batchSize);

            return message;
        }


        private static CanalSharp.Client.ICanalConnector ProvideCanalClient(CanalConfiguration configuration)
        {
            return CanalConnectors.NewSingleConnector(configuration.ServerAddress, configuration.ServerPort,
                configuration.DatabaseName, configuration.Credentials.UserName,
                configuration.Credentials.Password);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [SuppressMessage("ReSharper", "InvertIf")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
                _canalConnector.UnSubscribe();
                _canalConnector.StopRunning();
            }

            // get rid of unmanaged resources
        }

         // only if you use unmanaged resources directly in B
        ~CanalConnectorService()
        {
            Dispose(false);
        }
    }
}
