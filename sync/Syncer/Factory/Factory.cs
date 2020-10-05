using System;
using Syncer.Fa;

namespace Syncer.Factory
{
    public class Factory<T> : IFactory<T>
    {
        private readonly Func<T> _initFunc;

        public Factory(Func<T> initFunc)
        {
            _initFunc = initFunc;
        }

        public T Create(string name)
        {
            return _initFunc();
        }
    }
}