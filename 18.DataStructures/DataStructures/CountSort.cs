namespace DataStructures;

public class CountSortAlgorithm
{
	public static int[] Process(List<int> inputArray)
	{
		var maxValue = 0;
		foreach (var value in inputArray)
			maxValue = Math.Max(maxValue, value);

		var countArray = new int[maxValue + 1];

		for (var index = 0; index < inputArray.Count; index++)
			countArray[inputArray[index]]++;

		for (int i = 1; i <= maxValue; i++)
			countArray[i] += countArray[i - 1];

		var outputArray = new int[inputArray.Count];
		for (int i = inputArray.Count - 1; i >= 0; i--)
		{
			outputArray[countArray[inputArray[i]] - 1] = inputArray[i];
			countArray[inputArray[i]]--;
		}
		return outputArray;
	}

}
