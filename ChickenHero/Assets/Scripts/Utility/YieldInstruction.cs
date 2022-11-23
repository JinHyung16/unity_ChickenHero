using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HughUtility.Cashing
{
    internal static class YieldInstruction
    {
        class FloatCompare : IEqualityComparer<float>
        {
            bool IEqualityComparer<float>.Equals(float a, float b)
            {
                return a == b;
            }
            int IEqualityComparer<float>.GetHashCode(float a)
            {
                return a.GetHashCode();
            }
        }

        private static readonly Dictionary<float, WaitForSeconds> waitForSec = new Dictionary<float, WaitForSeconds>(new FloatCompare());
        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            WaitForSeconds w;
            if (!waitForSec.TryGetValue(seconds, out w))
            {
                waitForSec.Add(seconds, w = new WaitForSeconds(seconds));
            }
            return w;
        }
    }
}