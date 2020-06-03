using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yaclip
{
    internal sealed class CommandCollection : IList<Command>
    {
        private readonly IList<Command> InnerList;

        public CommandCollection(IList<Command> innerList)
        {
            this.InnerList = innerList ?? throw new ArgumentNullException(nameof(innerList));
        }

        public Command this[int index] { get => this.InnerList[index]; set => this.InnerList[index] = value; }

        public int Count => this.InnerList.Count;

        public bool IsReadOnly => this.InnerList.IsReadOnly;

        public void Add(Command item)
        {
            this.InnerList.Add(item);
        }

        public bool TryGet([MaybeNullWhen(false)] out Command command, params string[] name)
        {
            foreach (var cmd in InnerList)
            {
                if (name.SequenceEqual(cmd.Name))
                {
                    command = cmd;
                    return true;
                }
            }

            command = null;
            return false;
        }

        public void Clear()
        {
            this.InnerList.Clear();
        }

        public bool Contains(Command item)
        {
            return this.InnerList.Contains(item);
        }

        public void CopyTo(Command[] array, int arrayIndex)
        {
            this.InnerList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Command> GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }

        public int IndexOf(Command item)
        {
            return this.InnerList.IndexOf(item);
        }

        public void Insert(int index, Command item)
        {
            this.InnerList.Insert(index, item);
        }

        public bool Remove(Command item)
        {
            return this.InnerList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.InnerList.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InnerList.GetEnumerator();
        }
    }
}
