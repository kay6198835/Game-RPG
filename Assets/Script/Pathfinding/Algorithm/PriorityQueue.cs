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

    public void Enqueue(SearchNode searchNode)
    {
        searchNode.HeapIndex = count;
        items[count] = searchNode;
        SortUp(searchNode);
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

    // Gọi khi G của searchNode giảm → ưu tiên tăng → đẩy lên
    public void UpdatePriority(SearchNode searchNode) => SortUp(searchNode);

    public bool Contains(SearchNode searchNode)
        => searchNode.HeapIndex < count && items[searchNode.HeapIndex] == searchNode;

    private void SortUp(SearchNode searchNode)
    {
        int parentIndex = (searchNode.HeapIndex - 1) / 2;
        while (searchNode.HeapIndex > 0 && Compare(searchNode, items[parentIndex]) > 0)
        {
            Swap(searchNode, items[parentIndex]);
            parentIndex = (searchNode.HeapIndex - 1) / 2;
        }
    }

    private void SortDown(SearchNode searchNode)
    {
        while (true)
        {
            int left  = searchNode.HeapIndex * 2 + 1;
            int right = searchNode.HeapIndex * 2 + 2;
            if (left >= count) return;

            int swapIndex = left;
            if (right < count && Compare(items[right], items[left]) > 0)
                swapIndex = right;

            if (Compare(items[swapIndex], searchNode) > 0)
                Swap(searchNode, items[swapIndex]);
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