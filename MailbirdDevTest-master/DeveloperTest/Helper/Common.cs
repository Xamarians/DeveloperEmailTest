using System;
using System.Collections.Generic;

namespace DeveloperTest.Helper
{
    internal static class Common
    {
        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int size)
        {
            for (int i = 0; i < locations.Count; i += size)
            {
                yield return locations.GetRange(i, Math.Min(size, locations.Count - i));
            }
        }
    }
}
