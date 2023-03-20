using SaberInteractiveTask;

internal class Program
{
    private static void Main(string[] args)
    {
        var list = new ListRandom();
        var list2 = new ListRandom();

        for (int i = 0; i < 10; i++)
        {
            list.Append(new ListNode() { Data = "my data is " + i });
        }

        for (int i = 0; i < 10; i++)
        {
            var node = list.GetNodeAt(i);
            list.LinkToRandom(node);
        }

        using (var s = new MemoryStream())
        {
            list.Serialize(s);
            s.Position = 0;
            list2.Deserialize(s);
        }

        for (int i = 0; i < list2.Count; i++)
        {
            var node = list.GetNodeAt(i);
            Console.WriteLine("node: " + node.Data);
            Console.WriteLine("  prev -> " + node.Previous?.Data);
            Console.WriteLine("  next -> " + node.Next?.Data);
            Console.WriteLine("  rand -> " + node.Random?.Data);
        }
    }
}