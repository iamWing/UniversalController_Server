using System;
using UnityEngine;

namespace AlphaOwl.UniversalController.Utilities
{

    /// <summary>
    /// Type of the log message.
    /// </summary>
    enum LogType
    {
        Normal,
        Warning,
        Error
    }

    /// <summary>
    /// Customised utilities for debug usage across the library.
    /// </summary>
    static class DebugUtilities
    {
        public static bool Enable;

        /// <summary>
        /// Write log message to console.
        /// </summary>
        /// <param name="msg">Log message.</param>
        /// <param name="type">Type of the log. 
        /// Default == LogType.Normal</param>
        public static void Log(string msg, LogType type = LogType.Normal)
        {
            if (!Enable)
                return;
            
            switch (type)
            {
                case LogType.Normal:
                    Debug.Log(msg);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(msg);
                    break;
                case LogType.Error:
                    Debug.LogError(msg);
                    break;
            }
        }
    }
}
