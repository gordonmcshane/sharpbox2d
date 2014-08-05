using System.Collections.Generic;
using System.Diagnostics;

namespace SharpBox2D.Pooling.Arrays
{
    public class GeneratorArray
    {
        private Dictionary<int, Generator[]> map = new Dictionary<int, Generator[]>();

        public Generator[] get(int length)
        {
            Debug.Assert(length > 0);

            if (!map.ContainsKey(length))
            {
                map.Add(length, getInitializedArray(length));
            }

            Debug.Assert(map[length].Length == length, "Array not built of correct length");
            return map[length];
        }

        internal Generator[] getInitializedArray(int length)
        {
            Generator[] ray = new Generator[length];
            for (int i = 0; i < ray.Length; i++)
            {
                ray[i] = new Generator();
            }
            return ray;
        }
    }
}