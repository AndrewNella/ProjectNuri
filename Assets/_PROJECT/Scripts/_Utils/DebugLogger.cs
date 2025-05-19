using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kisei.Utility
{
    //Credit: https://youtu.be/lRqR4YF8iQs
    public static class DebugLogger
    {
        private static void DoLog(Action<string> LogFunction, string prefix, Object obj, object msg)
        {
#if UNITY_EDITOR
            LogFunction($"{prefix}[<color=lightblue>{obj.name}</color>]: {msg}");
#endif
        }
        public static void Log(this Object obj, object msg)
        {
            DoLog(Debug.Log, "[LOG]", obj, msg);
        }

        public static void LogError(this Object obj, object msg)
        {
            DoLog(Debug.LogError, "<color=red><!>[ERROR]</color>", obj, msg);
        }        
        public static void LogWarning(this Object obj, object msg)
        {
            DoLog(Debug.LogWarning, "<color=yellow>[WARNING]</color>", obj,  msg);
        }

        public static void LogSuccess(this Object obj, object msg)
        {
            DoLog(Debug.Log,"<color=green>[SUCCESS]</color>", obj, msg);
        }

        public static void LogHighlight(this Object ong, object msg)
        {
            DoLog(Debug.Log, "<color=cyan>++[HIGHLIGHT]++</color>", ong, msg);
        }
        
        public static void LogCaution(this Object obj, object msg)
        {
            DoLog(Debug.LogWarning, "<color=orange><-->[CAUTION]<--></color>", obj, msg);
        }

        public static void LogTODO(this Object obj, object msg)
        {
            DoLog(Debug.Log, "<color=green> ++[TODO]++ </color>", obj, msg);
        }

        public static void LogData(this Object obj, object msg)
        {
            DoLog(Debug.Log, "<color=magenta> [?][DATA][?] </color>", obj, msg);
        }
    }
}