using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Common.DataStructures
{
    /// <summary>
    /// Implementation of a Tree Data Strcuture
    /// </summary>
    /// <remarks>Taken from Sample @ http://stackoverflow.com/questions/66893/tree-data-structure-in-c </remarks>
    public class Tree<T>
    {
        readonly T _data;
        readonly LinkedList<Tree<T>> _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tree&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Tree(T data)
        {
            this._data = data;
            _children = new LinkedList<Tree<T>>();
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="childData">The child data.</param>
        public void AddChild(T childData)
        {
            _children.AddLast(new Tree<T>(childData));
        }

        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public Tree<T> GetChild(int i)
        {
            foreach (Tree<T> n in _children)
                if (--i == 0) return n;
            return null;
        }

        public T Current
        {
            get
            {
                return this._data;    
            }
        }

        public List<Tree<T>> Children
        {
            get { return this._children.ToList(); }
        }

        /// <summary>
        /// Traverses the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="action">The action.</param>
        public static void Traverse(Tree<T> node, Action<Tree<T>> action)
        {
            action(node);
            foreach (Tree<T> kid in node._children)
                Traverse(kid, action);
        }
    }
}
