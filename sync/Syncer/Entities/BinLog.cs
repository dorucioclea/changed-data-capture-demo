using System;

namespace Syncer.Entities
{
    public class BinLog
    {
        private const string Delimiter = "%%";

        public string FileName { get; }

        public long Position { get; }

        public BinLog(string fileName, long position) => (FileName, Position) = (fileName, position);

        public BinLog(string content)
        {
            var info = content.Split(Delimiter);

            if (info.Length != 2)
            {
                throw new ArgumentException(nameof(content));
            }

            FileName = info[0];
            Position = long.Parse(info[1]);
        }

        public override string ToString()
        {
            return $"{FileName}{Delimiter}{Position}";
        }
    }
}
