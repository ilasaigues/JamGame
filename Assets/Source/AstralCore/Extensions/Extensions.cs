using UnityEngine;

namespace AstralCore
{
    public static class Extensions
    {
        /// <summary>
        /// Convert color to hex string.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToHex(this Color color)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}", (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }

        /// <summary>
        /// Log an object of type <typeparamref name="T"/> in-line and return it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object to be logged.</param>
        /// <param name="message">Optional message to include in the log. If provided, it should contain a {0} anywhere it wants the object added as a value.</param>
        /// <returns></returns>
        public static T LogInPlace<T>(this T obj, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.Log(obj);
            }
            else
            {
                Debug.Log(string.Format(message, obj));
            }
            return obj;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject == null)
            {
                return null;
            }
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }
    }
}
