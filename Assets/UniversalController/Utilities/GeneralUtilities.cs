using System;

namespace AlphaOwl.UniversalController.Utilities
{
    public static class GeneralUtilities
    {

        public static T[] ArrayCopy<T>(Array source, int startIndex, 
		int endIndex)
        {
            int arrayLength = (startIndex == 0) ? 
				endIndex + 1 : endIndex - startIndex + 1;

            T[] result = new T[arrayLength];

            Array.Copy(
                sourceArray: source,
                sourceIndex: startIndex,
                destinationArray: result,
                destinationIndex: 0,
                length: result.Length 
            );

            return result;
        }
    }
}
