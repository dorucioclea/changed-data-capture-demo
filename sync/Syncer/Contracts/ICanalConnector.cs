using System;
using CanalSharp.Protocol;

namespace Syncer.Contracts
{
    public interface ICanalConnector : IDisposable
    {
        public void Connect();

        public Message Get(int batchSize = 1024);
    }
}
