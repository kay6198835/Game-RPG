public class PriorityQueue
{
    private readonly SearchNode[] items;
    private int count;

    public int Count => count;

    public PriorityQueue(int maxSize)
    {
        items = new SearchNode[maxSize];
    }

    public void Clear() => count = 0;

    public void Enqueue(SearchNode node)
    {
        node.HeapIndex = count;
        items[count] = node;
        SortUp(node);
        count++;
    }

    public SearchNode Dequeue()
    {
        SearchNode first = items[0];
        count--;
        items[0] = items[count];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return first;
    }

    // Gọi khi G của node giảm → ưu tiên tăng → đẩy lên
    public void UpdatePriority(SearchNode node) => SortUp(node);

    public bool Contains(SearchNode node)
        => node.HeapIndex < count && items[node.HeapIndex] == node;

    private void SortUp(SearchNode node)
    {
        int parentIndex = (node.HeapIndex - 1) / 2;
        while (node.HeapIndex > 0 && Compare(node, items[parentIndex]) > 0)
        {
            Swap(node, items[parentIndex]);
            parentIndex = (node.HeapIndex - 1) / 2;
        }
    }

    private void SortDown(SearchNode node)
    {
        while (true)
        {
            int left  = node.HeapIndex * 2 + 1;
            int right = node.HeapIndex * 2 + 2;
            if (left >= count) return;

            int swapIndex = left;
            if (right < count && Compare(items[right], items[left]) > 0)
                swapIndex = right;

            if (Compare(items[swapIndex], node) > 0)
                Swap(node, items[swapIndex]);
            else
                return;
        }
    }

    private void Swap(SearchNode a, SearchNode b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;
        (a.HeapIndex, b.HeapIndex) = (b.HeapIndex, a.HeapIndex);
    }

    // >0 nghĩa là a ưu tiên cao hơn b: F nhỏ hơn thắng, hoà F thì H nhỏ hơn thắng
    private int Compare(SearchNode a, SearchNode b)
    {
        int c = a.FCost.CompareTo(b.FCost);
        if (c == 0) c = a.HCost.CompareTo(b.HCost);
        return -c;
    }
}