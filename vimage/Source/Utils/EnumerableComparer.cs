﻿/*
    http://www.interact-sw.co.uk/iangblog/2007/12/13/natural-sorting
*/
using System.Collections.Generic;

/// <summary>
/// Compares two sequences.
/// </summary>
/// <typeparam name="T">Type of item in the sequences.</typeparam>
/// <remarks>
/// Compares elements from the two input sequences in turn. If we
/// run out of list before finding unequal elements, then the shorter
/// list is deemed to be the lesser list.
/// </remarks>
public class EnumerableComparer<T> : IComparer<IEnumerable<T>>
{
    /// <summary>
    /// Create a sequence comparer using the default comparer for T.
    /// </summary>
    public EnumerableComparer()
    {
        comp = Comparer<T>.Default;
    }

    /// <summary>
    /// Create a sequence comparer, using the specified item comparer
    /// for T.
    /// </summary>
    /// <param name="comparer">Comparer for comparing each pair of
    /// items from the sequences.</param>
    public EnumerableComparer(IComparer<T> comparer)
    {
        comp = comparer;
    }

    /// <summary>
    /// Object used for comparing each element.
    /// </summary>
    private IComparer<T> comp;


    /// <summary>
    /// Compare two sequences of T.
    /// </summary>
    /// <param name="x">First sequence.</param>
    /// <param name="y">Second sequence.</param>
    public int Compare(IEnumerable<T> x, IEnumerable<T> y)
    {
        using (IEnumerator<T> leftIt = x.GetEnumerator())
        using (IEnumerator<T> rightIt = y.GetEnumerator())
        {
            while (true)
            {
                bool left = leftIt.MoveNext();
                bool right = rightIt.MoveNext();

                if (!(left || right)) return 0;

                if (!left) return -1;
                if (!right) return 1;

                int itemResult = comp.Compare(leftIt.Current, rightIt.Current);
                if (itemResult != 0) return itemResult;
            }
        }
    }
}
