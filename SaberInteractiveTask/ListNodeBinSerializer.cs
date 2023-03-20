using System.Text;

namespace SaberInteractiveTask
{
    public class ListNodeBinSerializer
    {
        private const int IdOffset = 0;
        private const int PreviousIdOffset = 4;
        private const int NextIdOffset = 8;
        private const int RandomIdOffset = 12;
        private const int DataLengthOffset = 16;

        private readonly StringBuilder _stringBuilder = new();

        public void Serialize(ListNodeMeta nodeMeta, Stream s)
        {
            byte[] buffer;

            buffer = BitConverter.GetBytes(nodeMeta.Id);
            s.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(nodeMeta.PreviousId);
            s.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(nodeMeta.NextId);
            s.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(nodeMeta.RandomId);
            s.Write(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(nodeMeta.Data.Length);
            s.Write(buffer, 0, buffer.Length);

            foreach (var symbol in nodeMeta.Data)
            {
                buffer = BitConverter.GetBytes(symbol);
                s.Write(buffer);
            }
        }

        public IEnumerable<ListNodeMeta> Deserialize(Stream s)
        {
            byte[] buffer = new byte[sizeof(int) * 5];
            byte[] dataBuffer;
            var node = new ListNodeMeta();

            s.Read(buffer, 0, 4);
            var count = BitConverter.ToInt32(buffer, 0);

            while (count != 0)
            {
                s.Read(buffer, 0, buffer.Length);
                DeserializeIds(buffer, node);

                var dataLength = DeserializeDataLength(buffer) * sizeof(char);
                dataBuffer = new byte[dataLength];
                s.Read(dataBuffer, 0, dataLength);
                DeserializeData(dataBuffer, node);

                count--;

                yield return node;
            }
        }

        private void DeserializeIds(byte[] buffer, ListNodeMeta node)
        {
            node.Id = BitConverter.ToInt32(buffer, IdOffset);
            node.PreviousId = BitConverter.ToInt32(buffer, PreviousIdOffset);
            node.NextId = BitConverter.ToInt32(buffer, NextIdOffset);
            node.RandomId = BitConverter.ToInt32(buffer, RandomIdOffset);
        }

        private int DeserializeDataLength(byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, DataLengthOffset);
        }

        private void DeserializeData(byte[] buffer, ListNodeMeta node)
        {
            for (int i = 0; i < buffer.Length; i+= sizeof(char))
            {
                _stringBuilder.Append(BitConverter.ToChar(buffer, i));
            }
            node.Data = _stringBuilder.ToString();
            _stringBuilder.Clear();
        }
    }
}
