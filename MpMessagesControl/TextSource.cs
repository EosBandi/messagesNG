using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPMessageControlBoxNS
{
    public class TextSource : IList<Line>, IDisposable
    {
        readonly public List<Line> lines = new List<Line>();

        private bool disposedValue;
        public int Count => ((ICollection<Line>)lines).Count;
        public bool IsReadOnly => ((ICollection<Line>)lines).IsReadOnly;
        public Line this[int index] { get => ((IList<Line>)lines)[index]; set => ((IList<Line>)lines)[index] = value; }
        public int IndexOf(Line item)
        {
            return ((IList<Line>)lines).IndexOf(item);
        }
        public void Insert(int index, Line item)
        {
            ((IList<Line>)lines).Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            ((IList<Line>)lines).RemoveAt(index);
        }
        public void Add(Line item)
        {
            ((ICollection<Line>)lines).Add(item);
        }
        public void Clear()
        {
            ((ICollection<Line>)lines).Clear();
        }
        public bool Contains(Line item)
        {
            return ((ICollection<Line>)lines).Contains(item);
        }
        public void CopyTo(Line[] array, int arrayIndex)
        {
            ((ICollection<Line>)lines).CopyTo(array, arrayIndex);
        }
        public bool Remove(Line item)
        {
            return ((ICollection<Line>)lines).Remove(item);
        }
        public IEnumerator<Line> GetEnumerator()
        {
            return ((IEnumerable<Line>)lines).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)lines).GetEnumerator();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }



}

