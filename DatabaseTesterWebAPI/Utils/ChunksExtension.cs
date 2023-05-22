using System.Collections;

namespace DatabaseTesterWebAPI.Utils
{
    public class ChunksExtension
    {
        public record Chunks : IEnumerable<Chunks.Chunk>
        {
            public Chunks(int totalCount, int chunkSize)
            {
                Items = Enumerable
                    .Range(0, totalCount)
                    .Chunk(chunkSize)
                    .Select((value, index) => new Chunk(index, value.Length))
                    .ToList();
            }

            private List<Chunk> Items { get; }
            public int MaxSize => Items.Max(x => x.Count);
            public int MinSize => Items.Min(x => x.Count);
            public int TotalChunks => Items.Count;

            public IEnumerator<Chunk> GetEnumerator()
                => Items.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();

            public record Chunk(int Index, int Count);
        }
    }
}
