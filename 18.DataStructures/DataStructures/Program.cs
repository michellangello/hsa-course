using System.Diagnostics;
using MoreLinq.Extensions;

namespace DataStructures;

public static class BinaryTreeBenchmark
{
	private static readonly Random Random = new();
	private const int DataSetCount = 100;
	private const int BinaryTreeDataSetCount = 500;

	public static void Main()
	{
		// List<int> inputArray = [4, 3, 12, 1, 5, 5, 3, 9];
		//
		// var outputArray = CountSortAlgorithm.Process(inputArray);
		// for (int i = 0; i < inputArray.Count; i++)
		// 	Console.Write(outputArray[i] + " ");
		// Console.WriteLine();

		// CountSortMeasurement();
		// CountSortWithScatteredArrayMeasurement();
		// CountSortWithSingleBigElementMeasurement();

		InsertMeasurement();
		DeleteMeasurement();
		FindMeasurement();
	}


	private static void CountSortMeasurement()
	{
		const string csvFilePath = "count-sort-measures.csv";

		Console.WriteLine("Count sort measurement");

		var directory = AppDomain.CurrentDomain.BaseDirectory;
		var filePath = Path.Combine(directory, csvFilePath);
		if (File.Exists(filePath))
			File.Delete(filePath);

		using var writer = new StreamWriter(filePath);
		writer.WriteLine("array_size,time_ns");

		for (var count = 1; count < DataSetCount; count++)
		{
			var inputArray = new List<int>();
			var arraySize = count * 10;
			for (var i = 1; i < arraySize; i++)
				inputArray.Add(Random.Next(1, arraySize));

			var timeNs = MeasureExecutionTime(() => CountSortAlgorithm.Process(inputArray));

			writer.WriteLine($"{arraySize},{timeNs}");
		}
	}

	private static void CountSortWithScatteredArrayMeasurement()
	{
		const string csvFilePath = "count-sort-with-widely-scattered-array-measures.csv";

		Console.WriteLine("Count sort measurement with max int");

		var directory = AppDomain.CurrentDomain.BaseDirectory;
		var filePath = Path.Combine(directory, csvFilePath);
		if (File.Exists(filePath))
			File.Delete(filePath);

		using var writer = new StreamWriter(filePath);
		writer.WriteLine("array_size,time_ns");

		for (var count = 1; count < DataSetCount; count++)
		{
			var arraySize = count * 10;

			var inputArray = Enumerable.Range(0, arraySize)
				.Select(c => c * 10_000)
				.Shuffle()
				.ToList();

			var timeNs = MeasureExecutionTime(() => CountSortAlgorithm.Process(inputArray));

			writer.WriteLine($"{arraySize},{timeNs}");
		}
	}

	private static void CountSortWithSingleBigElementMeasurement()
	{
		const string csvFilePath = "count-sort-with-single-big-value-measures.csv";

		Console.WriteLine("Count sort measurement with max int");

		var directory = AppDomain.CurrentDomain.BaseDirectory;
		var filePath = Path.Combine(directory, csvFilePath);
		if (File.Exists(filePath))
			File.Delete(filePath);

		using var writer = new StreamWriter(filePath);
		writer.WriteLine("array_size,time_ns");

		for (var count = 1; count < DataSetCount; count++)
		{
			var inputArray = new List<int>();
			var arraySize = count * 100;
			for (var i = 1; i < arraySize; i++)
				inputArray.Add(1);

			inputArray.Add(10_000_000);

			var timeNs = MeasureExecutionTime(() => CountSortAlgorithm.Process(inputArray));

			writer.WriteLine($"{arraySize},{timeNs}");
		}
	}


	private static void FindMeasurement()
	{
		const string csvFilePath = "find-measures.csv";

		Console.WriteLine("Find measurement");

		var directory = AppDomain.CurrentDomain.BaseDirectory;
		var filePath = Path.Combine(directory, csvFilePath);
		if (File.Exists(filePath))
			File.Delete(filePath);

		using var writer = new StreamWriter(filePath);
		writer.WriteLine("array_size,time_ns");

		for (var count = 1; count < BinaryTreeDataSetCount; count++)
		{
			var tree = new BinaryTree.BinaryTree();

			var size = count * 100;
			for (int i = 0; i < size; i++)
				tree.Insert(Random.Next(1, size));

			var valueToFind = Random.Next(1, size);
			var timeNs = MeasureExecutionTime(() => tree.Find(valueToFind));

			writer.WriteLine($"{size},{timeNs}");
		}
	}

	private static void DeleteMeasurement()
	{
		const string csvFilePath = "delete-measures.csv";

		Console.WriteLine("Delete measurement");

		var directory = AppDomain.CurrentDomain.BaseDirectory;
		var filePath = Path.Combine(directory, csvFilePath);
		if (File.Exists(filePath))
			File.Delete(filePath);

		using var writer = new StreamWriter(filePath);
		writer.WriteLine("array_size,time_ns");

		for (var count = 1; count < BinaryTreeDataSetCount; count++)
		{
			var tree = new BinaryTree.BinaryTree();

			var size = count * 100;
			for (int i = 0; i < size; i++)
				tree.Insert(Random.Next(1, size));

			var valueToDelete = Random.Next(1, size);
			var timeNs = MeasureExecutionTime(() => tree.Insert(valueToDelete));

			writer.WriteLine($"{size},{timeNs}");
		}
	}

	private static void InsertMeasurement()
	{
		const string csvFilePath = "insert-measures.csv";

		Console.WriteLine("Insert measurement");

		var directory = AppDomain.CurrentDomain.BaseDirectory;
		var filePath = Path.Combine(directory, csvFilePath);

		if (File.Exists(filePath))
			File.Delete(filePath);

		using var writer = new StreamWriter(filePath);
		writer.WriteLine("array_size,time_ns");

		for (var count = 1; count < BinaryTreeDataSetCount; count++)
		{
			var tree = new BinaryTree.BinaryTree();

			var size = count * 100;
			for (int i = 0; i < size; i++)
				tree.Insert(Random.Next(1, size));

			var valueToInsert = Random.Next(1, size);
			var timeNs = MeasureExecutionTime(() => tree.Insert(valueToInsert));

			writer.WriteLine($"{size},{timeNs}");
		}
	}


	private static double MeasureExecutionTime(Action action)
	{
		var stopwatch = new Stopwatch();
		stopwatch.Start();
		action.Invoke();
		stopwatch.Stop();
		return stopwatch.Elapsed.TotalNanoseconds;
	}
}
