public class PriorityQueue
{
    private readonly Node[] items;
    private int count;

    public PriorityQueue(int maxSize) { items = new Node[maxSize]; }

    public int Count => count;
    public void Clear() { count = 0; }

    public void Enqueue(Node node)
    {
        node.HeapIndex = count;
        items[count] = node;
        SortUp(node);
        count++;
    }

    public Node Dequeue()
    {
        Node first = items[0];
        count--;
        items[0] = items[count];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return first;
    }

    public bool Contains(Node node) => node.HeapIndex < count && items[node.HeapIndex] == node;

    public void UpdatePriority(Node node) => SortUp(node);

    private void SortUp(Node node)
    {
        while (node.HeapIndex > 0)
        {
            int parent = (node.HeapIndex - 1) / 2;
            if (Compare(node, items[parent]) < 0) Swap(node, items[parent]);
            else break;
        }
    }

    private void SortDown(Node node)
    {
        while (true)
        {
            int l = node.HeapIndex * 2 + 1;
            int r = node.HeapIndex * 2 + 2;
            int swapIdx = node.HeapIndex;
            if (l < count && Compare(items[l], items[swapIdx]) < 0) swapIdx = l;
            if (r < count && Compare(items[r], items[swapIdx]) < 0) swapIdx = r;
            if (swapIdx == node.HeapIndex) break;
            Swap(node, items[swapIdx]);
        }
    }

    private int Compare(Node a, Node b)
    {
        int c = a.FCost.CompareTo(b.FCost);
        if (c == 0) c = a.HCost.CompareTo(b.HCost);
        return c;
    }

    private void Swap(Node a, Node b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;
        int tmp = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = tmp;
    }
}
