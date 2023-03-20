using Xunit;

namespace SaberInteractiveTask.Tests
{
    public class ListRandomTests
    {
        [Fact]
        public void TestAppend()
        {
            // Arrange
            var list = new ListRandom();

            // Act
            for (int i = 0; i < 10; i++)
            {
                list.Append(new ListNode() { Data = "i am " + i });
            }

            // Assert
            Assert.Equal(10, list.Count);
        }

        [Fact]
        public void TestGetAt()
        {
            // Arrange
            var list = new ListRandom();
            string data;

            // Act
            for (int i = 0; i < 10; i++)
            {
                list.Append(new ListNode() { Data = "i am " + i });
            }
            data = list.GetNodeAt(4).Data;

            // Assert
            Assert.Equal("i am 4", data);
        }

        [Fact]
        public void TestSerializeAndDeserialize()
        {
            // Arrange
            var list1 = new ListRandom();
            var list2 = new ListRandom();
            ListNode node1;
            ListNode node2;
            var nodesAreEqual = true;

            // Act
            for (int i = 0; i < 10; i++)
            {
                list1.Append(new ListNode() { Data = "i am " + i });
            }

            for (int i = 0; i < 10; i++)
            {
                node1 = list1.GetNodeAt(i);
                list1.LinkToRandom(node1);
            }

            using (var s = new MemoryStream())
            {
                list1.Serialize(s);
                s.Position = 0;
                list2.Deserialize(s);
            }

            for (int i = 0; i < 10; i++)
            {
                node1 = list1.GetNodeAt(i);
                node2 = list2.GetNodeAt(i);

                nodesAreEqual = nodesAreEqual 
                    && node1.Data == node2.Data
                    && node1.Previous?.Data == node2.Previous?.Data
                    && node1.Next?.Data == node2.Next?.Data
                    && node1.Random?.Data == node2.Random?.Data;
            }

            // Assert
            Assert.True(nodesAreEqual);
        }
    }
}