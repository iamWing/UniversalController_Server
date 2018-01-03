using UnityEngine;

namespace AlphaOwl.UniversalController.Utilities
{

    /// <summary>
    /// Type of the log message.
    /// </summary>
    public enum LogType
    {
        Normal,
        Warning,
        Error
    }

    /// <summary>
    /// Customised utilities for debug usage across the library.
    /// </summary>
    public static class DebugUtilities
    {
        public static bool Enable;

        /// <summary>
        /// Write log message to console.
        /// </summary>
        /// <param name="msg">Log message.</param>
        /// <param name="obj">Object to which the message applies.</param>
        /// <param name="type">Type of the log. 
        /// Default == LogType.Normal</param>
        public static void Log(string msg, Object obj = null, 
        LogType type = LogType.Normal)
        {
            if (!Enable)
                return;

            switch (type)
            {
                case LogType.Normal:
                    if (obj != null)
                        Debug.Log(msg);
                    else
                        Debug.Log(msg, obj);
                    break;
                case LogType.Warning:
                    if (obj != null)
                        Debug.LogWarning(msg);
                    else
                        Debug.LogWarning(msg, obj);
                    break;
                case LogType.Error:
                    if (obj != null)
                        Debug.LogError(msg);
                    else
                        Debug.LogError(msg, obj);
                    break;
            }
        }
    }
}
