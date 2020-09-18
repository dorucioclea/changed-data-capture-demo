using System;
using CanalSharp.Client.Impl;
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
            throw new NotImplementedException();
        }


        private static CanalSharp.Client.ICanalConnector ProvideCanalClient(CanalConfiguration configuration)
        {
            return CanalConnectors.NewSingleConnector(configuration.ServerAddress, configuration.ServerPort,
                configuration.DatabaseName, configuration.Credentials.UserName,
                configuration.Credentials.Password);
        }
    }
}
