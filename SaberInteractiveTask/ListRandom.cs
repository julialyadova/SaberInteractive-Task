namespace SaberInteractiveTask
{
    public class ListRandom
    {
        private const int StartId = 0;
        private const int NullId = -1;

        public ListNode Head;
        public ListNode Tail;
        public int Count;

        public void Append(ListNode node)
        {
            if (Count == 0)
            {
                Head = node;
                Tail = node;
                Count = 1;
                return;
            }

            Tail.Next = node;
            node.Previous = Tail;
            Tail = node;

            Count++;
        }

        public void Insert(ListNode node, int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException(nameof(index));

            if (Count == 0)
            {
                Head = node;
                Tail = node;
                Count = 1;
                return;
            }

            var current = GetNodeAt(index);

            node.Next = current;
            node.Previous = current.Previous;
            current.Previous = node;

            if (node.Previous != null)
                node.Previous.Next = node;

            Count++;
        }

        public ListNode GetNodeAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException(nameof(index));

            var node = Head;
            for (int i = 0; i < Count; i++)
            {
                if (i == index)
                    return node;

                node = node.Next;
            }

            return node;
        }

        public void LinkToRandom(ListNode node)
        {
            int randomIndex = new Random().Next(0, Count);
            node.Random = GetNodeAt(randomIndex);
        }

        public void Serialize(Stream s)
        {
            var nodeIds = IndexNodes();
            var serializer = new ListNodeBinSerializer();

            s.Write(BitConverter.GetBytes(Count));

            ListNode node;
            foreach (var nodeId in nodeIds)
            {
                node = nodeId.Key;
                var nodeMeta = new ListNodeMeta()
                {
                    Id = nodeId.Value,
                    PreviousId = GetId(node.Previous, nodeIds),
                    NextId = GetId(node.Next, nodeIds),
                    RandomId = GetId(node.Random, nodeIds),
                    Data = node.Data
                };

                serializer.Serialize(nodeMeta, s);
            }
        }

        public void Deserialize(Stream s)
        {
            var serializer = new ListNodeBinSerializer();
            var nodeIds = new Dictionary<int, ListNode>();
            var randomIds = new Dictionary<ListNode, int>();

            ListNode node;
            foreach (var nodeMeta in serializer.Deserialize(s))
            {
                node = new ListNode() { Data = nodeMeta.Data };
                Append(node);
                nodeIds[nodeMeta.Id] = node;
                randomIds[node] = nodeMeta.RandomId;
            }

            node = Head;
            while (node != null)
            { 
                var randomId = randomIds[node];

                if (randomId != NullId)
                    node.Random = nodeIds[randomId];

                node = node.Next;
            }
        }

        private int GetId(ListNode node, Dictionary<ListNode, int> nodeIds)
        {
            if (node == null)
                return NullId;
            else
                return nodeIds[node];
        }

        private Dictionary<ListNode, int> IndexNodes()
        {
            var id = StartId;
            var node = Head;
            var nodeIds = new Dictionary<ListNode, int>();

            while (node != null)
            {
                nodeIds[node] = id;
                node = node.Next;
                id++;
            }

            return nodeIds;
        }
    }
}
