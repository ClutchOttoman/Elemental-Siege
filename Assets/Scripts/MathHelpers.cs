using System;
using System.Collections;
using System.Collections.Generic;

public class MathHelpers
{
    // Function to filter array elements based on a boolean criterion
    public static T[] FilteredArray<T>(T[] inputArray, Func<T, bool> criterion)
    {
        List<T> filteredList = new List<T>();

        // Loop through each element in the input array
        foreach (T element in inputArray)
        {
            // Add element to the filtered list if it meets the criterion
            if (criterion(element))
            {
                filteredList.Add(element);
            }
        }

        // Convert the filtered list back to an array and return
        return filteredList.ToArray();
    }
}
