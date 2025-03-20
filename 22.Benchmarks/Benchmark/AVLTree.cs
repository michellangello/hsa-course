namespace Benchmark;

public class AVLTree
{
	private Node? Root { get; set; }

	public void Insert(int value) => Root = Insert(Root, value);

	public bool Delete(int value)
	{
		Root = DeleteInternal(Root, value);
		return Root != null;
	}

	public Node? Find(int value) => FindInternal(Root, value);

	private Node Insert(Node? node, int value)
	{
		if (node == null)
			return new Node { Value = value, Height = 1 };

		if (value < node.Value)
			node.Left = Insert(node.Left, value);
		else if (value > node.Value)
			node.Right = Insert(node.Right, value);
		else
			return node;

		UpdateHeight(node);
		return Balance(node);
	}

	private Node? DeleteInternal(Node? root, int value)
	{
		if (root == null) return null;

		if (value < root.Value)
			root.Left = DeleteInternal(root.Left, value);
		else if (value > root.Value)
			root.Right = DeleteInternal(root.Right, value);
		else
		{
			if (root.Left == null) return root.Right;
			if (root.Right == null) return root.Left;

			var successor = GetSuccessor(root.Right);
			root.Value = successor.Value;
			root.Right = DeleteInternal(root.Right, successor.Value);
		}

		UpdateHeight(root);
		return Balance(root);
	}

	private Node GetSuccessor(Node node)
	{
		while (node.Left != null) node = node.Left;
		return node;
	}

	private Node? FindInternal(Node? node, int value) =>
		node == null || node.Value == value
			? node
			: value < node.Value
				? FindInternal(node.Left, value)
				: FindInternal(node.Right, value);

	private int Height(Node? node) => node?.Height ?? 0;

	private void UpdateHeight(Node node)
	{
		node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
	}

	private int BalanceFactor(Node? node) => node == null ? 0 : Height(node.Left) - Height(node.Right);

	private Node Balance(Node node)
	{
		int balance = BalanceFactor(node);

		if (balance > 1)
		{
			if (BalanceFactor(node.Left) < 0)
				node.Left = LeftRotate(node.Left);
			return RightRotate(node);
		}

		if (balance < -1)
		{
			if (BalanceFactor(node.Right) > 0)
				node.Right = RightRotate(node.Right);
			return LeftRotate(node);
		}

		return node;
	}

	private Node LeftRotate(Node z)
	{
		var y = z.Right!;
		z.Right = y.Left;
		y.Left = z;
		UpdateHeight(z);
		UpdateHeight(y);
		return y;
	}

	private Node RightRotate(Node z)
	{
		var y = z.Left!;
		z.Left = y.Right;
		y.Right = z;
		UpdateHeight(z);
		UpdateHeight(y);
		return y;
	}

	public record Node
	{
		public int Value { get; set; }
		public Node? Left { get; set; }
		public Node? Right { get; set; }
		public int Height { get; set; }
	}
}
