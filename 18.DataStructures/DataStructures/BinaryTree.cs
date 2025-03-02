namespace DataStructures.BinaryTree;

public class BinaryTree
{
	private Node? Root { get; set; }

	public void Insert(int value)
	{
		if (Root == null)
			Root = new Node { Value = value };
		else
			Insert(Root, value);
	}

	public Node? Find(int value) => FindInternal(Root, value);

	public bool Delete(int value)
	{
		var node = DeleteInternal(Root, value);
		return node != null;
	}

	private static Node? DeleteInternal(Node? root, int value)
	{
		if (root == null)
			return root;

		if (root.Value > value)
			root.Left = DeleteInternal(root.Left, value);
		else if (root.Value < value)
			root.Right = DeleteInternal(root.Right, value);
		else
		{
			if (root.Left == null)
				return root.Right;

			if (root.Right == null)
				return root.Left;

			var successor = GetSuccessor(root);
			root.Value = successor.Value;
			root.Right = DeleteInternal(root.Right, successor.Value);
		}

		return root;
	}

	private static Node GetSuccessor(Node current)
	{
		current = current.Right;
		while (current != null && current.Left != null)
		{
			current = current.Left;
		}

		return current;
	}

	private Node Insert(Node? node, int value)
	{
		if (node == null)
			return new Node { Value = value };

		if (value == node.Value)
			return node;

		if (value < node.Value)
			node.Left = Insert(node.Left, value);
		else if (value > node.Value)
			node.Right = Insert(node.Right, value);

		return node;
	}


	private Node? FindInternal(
		Node? node,
		int value) =>
		value switch
		{
			_ when node == null => null,
			_ when value == node.Value => node,
			_ when value < node.Value => FindInternal(node.Left, value),
			_ when value > node.Value => FindInternal(node.Right, value),
			_ => null
		};

	public record Node
	{
		public int Value { get; set; }

		public Node? Left { get; set; }

		public Node? Right { get; set; }
	}
}
