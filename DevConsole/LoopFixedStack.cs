using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityUtil.DevConsole
{
    public class LoopFixedCircle<T> : IEnumerable<T>
    {
        private int pointerEnd = 0;
        private int pointerCurrent = 0;
        private readonly T[] values;

        public LoopFixedCircle(int capacity)
        {
            if (capacity <= 0) throw new ArgumentException("capacity must bigger than 0");

            values = new T[capacity];
        }

        public void Push(T item)
        {
            if (pointerEnd != values.Length - 1)
            {
                values[pointerEnd] = item;
                pointerEnd++;
            }
            else
            {
                // 移除第一个元素并前移后面的元素
                for (int i = 0; i < values.Length - 1; i++)
                {
                    values[i] = values[i + 1];
                }
                values[pointerEnd] = item;
            }
            pointerCurrent = pointerEnd;
        }

        public T Next()
        {
            pointerCurrent++;
            pointerCurrent %= pointerEnd;
            T value = values[pointerCurrent];
            return value;
        }

        public T Last()
        {
            pointerCurrent--;
            if (pointerCurrent < 0) pointerCurrent = pointerEnd - 1;
            T value = values[pointerCurrent];
            return value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return values.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

