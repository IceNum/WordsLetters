using System;
using System.Collections.Generic;
using UnityEngine;

namespace KaboomStudio
{
    ///<summary>
    /// You can get access to extensions from object property like "transfor.position.GetVector2();" or "myList.Shuffle();"
    ///</summary>
    public static class Extensions
    {
        public static Vector2 GetVector2(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static string ReverseString(this string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}