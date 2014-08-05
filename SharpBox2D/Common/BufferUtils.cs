using System;
using System.Diagnostics;

namespace SharpBox2D.Common
{
    public static class BufferUtils
    {
        /** Reallocate a buffer. */

        public static T[] reallocateBuffer<T>(T[] oldBuffer, int oldCapacity, int newCapacity) where T : new()
        {
            Debug.Assert(newCapacity > oldCapacity);

            T[] newBuffer = new T[newCapacity];
            if (oldBuffer != null)
            {
                Array.Copy(oldBuffer, 0, newBuffer, 0, oldCapacity);
            }
            for (int i = oldCapacity; i < newCapacity; i++)
            {
                newBuffer[i] = new T();
            }
            return newBuffer;
        }

        /** Reallocate a buffer. */

        public static int[] reallocateBuffer(int[] oldBuffer, int oldCapacity, int newCapacity)
        {
            Debug.Assert(newCapacity > oldCapacity);
            int[] newBuffer = new int[newCapacity];
            if (oldBuffer != null)
            {
                Array.Copy(oldBuffer, 0, newBuffer, 0, oldCapacity);
            }
            return newBuffer;
        }

        /** Reallocate a buffer. */

        public static float[] reallocateBuffer(float[] oldBuffer, int oldCapacity, int newCapacity)
        {
            Debug.Assert(newCapacity > oldCapacity);
            float[] newBuffer = new float[newCapacity];
            if (oldBuffer != null)
            {
                Array.Copy(oldBuffer, 0, newBuffer, 0, oldCapacity);
            }
            return newBuffer;
        }

        /**
   * Reallocate a buffer. A 'deferred' buffer is reallocated only if it is not NULL. If
   * 'userSuppliedCapacity' is not zero, buffer is user supplied and must be kept.
   */

        public static T[] reallocateBuffer<T>(T[] buffer, int userSuppliedCapacity,
            int oldCapacity, int newCapacity, bool deferred) where T : new()
        {
            Debug.Assert(newCapacity > oldCapacity);
            Debug.Assert(userSuppliedCapacity == 0 || newCapacity <= userSuppliedCapacity);
            if ((!deferred || buffer != null) && userSuppliedCapacity == 0)
            {
                buffer = reallocateBuffer(buffer, oldCapacity, newCapacity);
            }
            return buffer;
        }

        /**
   * Reallocate an int buffer. A 'deferred' buffer is reallocated only if it is not NULL. If
   * 'userSuppliedCapacity' is not zero, buffer is user supplied and must be kept.
   */

        public static int[] reallocateBuffer(int[] buffer, int userSuppliedCapacity, int oldCapacity,
            int newCapacity, bool deferred)
        {
            Debug.Assert(newCapacity > oldCapacity);
            Debug.Assert(userSuppliedCapacity == 0 || newCapacity <= userSuppliedCapacity);
            if ((!deferred || buffer != null) && userSuppliedCapacity == 0)
            {
                buffer = reallocateBuffer(buffer, oldCapacity, newCapacity);
            }
            return buffer;
        }

        /**
   * Reallocate a float buffer. A 'deferred' buffer is reallocated only if it is not NULL. If
   * 'userSuppliedCapacity' is not zero, buffer is user supplied and must be kept.
   */

        public static float[] reallocateBuffer(float[] buffer, int userSuppliedCapacity, int oldCapacity,
            int newCapacity, bool deferred)
        {
            Debug.Assert(newCapacity > oldCapacity);
            Debug.Assert(userSuppliedCapacity == 0 || newCapacity <= userSuppliedCapacity);
            if ((!deferred || buffer != null) && userSuppliedCapacity == 0)
            {
                buffer = reallocateBuffer(buffer, oldCapacity, newCapacity);
            }
            return buffer;
        }

        /** Rotate an array, see std::rotate */

        public static void rotate<T>(T[] ray, int first, int new_first, int last)
        {
            int next = new_first;
            while (next != first)
            {
                T temp = ray[first];
                ray[first] = ray[next];
                ray[next] = temp;
                first++;
                next++;
                if (next == last)
                {
                    next = new_first;
                }
                else if (first == new_first)
                {
                    new_first = next;
                }
            }
        }

        /** Rotate an array, see std::rotate */

        public static void rotate(int[] ray, int first, int new_first, int last)
        {
            int next = new_first;
            while (next != first)
            {
                int temp = ray[first];
                ray[first] = ray[next];
                ray[next] = temp;
                first++;
                next++;
                if (next == last)
                {
                    next = new_first;
                }
                else if (first == new_first)
                {
                    new_first = next;
                }
            }
        }

        /** Rotate an array, see std::rotate */

        public static void rotate(float[] ray, int first, int new_first, int last)
        {
            int next = new_first;
            while (next != first)
            {
                float temp = ray[first];
                ray[first] = ray[next];
                ray[next] = temp;
                first++;
                next++;
                if (next == last)
                {
                    next = new_first;
                }
                else if (first == new_first)
                {
                    new_first = next;
                }
            }
        }
    }
}
