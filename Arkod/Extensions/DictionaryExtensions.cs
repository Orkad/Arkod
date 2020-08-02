using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arkod.Extensions
{
    /// <summary>
    /// Define extensions method on dictionary
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Track changes between current dictionary and new version of the dictionary
        /// </summary>
        /// <param name="news">new version of the dictionary</param>
        /// <param name="update">determine if the olds dictionary will be updated by the new one</param>
        /// <param name="changed">items who have been updated</param>
        /// <param name="added">items who have been added</param>
        /// <param name="removed">item who have been removed</param>
        public static void TrackChanges<TKey, TValue>(this IDictionary<TKey, TValue> olds, IDictionary<TKey, TValue> news, bool update,
            out IDictionary<TKey, TValue> changed, out IDictionary<TKey, TValue> added, out IDictionary<TKey, TValue> removed)
        {
            changed = new Dictionary<TKey, TValue>();
            added = new Dictionary<TKey, TValue>();
            removed = new Dictionary<TKey, TValue>();
            foreach (var intersect in news.Keys.Intersect(olds.Keys))
            {
                if (!EqualityComparer<TValue>.Default.Equals(olds[intersect], news[intersect]))
                {
                    changed.Add(intersect, news[intersect]);
                }
            }
            foreach (var add in news.Keys.Except(olds.Keys))
            {
                added.Add(add, news[add]);
            }
            foreach (var remove in olds.Keys.Except(news.Keys))
            {
                removed.Add(remove, olds[remove]);
            }
            if (update)
            {
                foreach (var change in changed)
                {
                    olds[change.Key] = change.Value;
                }
                foreach (var add in added)
                {
                    olds.Add(add.Key, add.Value);
                }
                foreach (var remove in removed)
                {
                    olds.Remove(remove.Key);
                }
            }
        }
    }
}
