using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.Services
{
    internal class ListOfRecent<T>
    {
        private readonly List<T> recent;
        private int position;

        public ListOfRecent(T firstElement)
        {
            recent = new List<T>();
            position = 0;
            recent.Add(firstElement);
        }

        public T Previous()
        {
            if (position > 0) position--;
            return recent.ElementAt(position);
        }

        public T Next()
        {
            if (position < recent.Count - 1) position++;
            return recent.ElementAt(position);
        }

        public T Current() => recent.ElementAt(position);

        public void Add(T obj)
        {
            if (recent[position].Equals(obj)) return;

            while (position < recent.Count - 1)
            {
                recent.RemoveRange(position + 1, recent.Count - position - 1);
            }

            recent.Add(obj);
            position++;
        }

        public void Remove(T obj) => recent.Remove(obj);

        public void Clear()
        {
            recent.Clear();
            position = -1;
        }
    }
}
