using UnityEngine;

namespace Debugger
{
    [DebugGroup(nameof(DebuggerSample))] // Avoid repetitions like "Sample.Health" and "Sample.SetValue"
    [DebugInstance]
    public class DebuggerSample : MonoBehaviour
    {
        [DebugMethod("Sample.Temp")]
        public static void Temp(System.DateTime value) => Debug.Log(value.DayOfWeek);

        [DebugField("Sample.Health", isReadOnly: true)]
        public int health;

        [DebugMethod("Sample.SetValue")]
        private static void SetValue(float a, string b, bool c) => print($"{a},{b},{c}");

        [DebugMethod("Sample.SetValue")]
        private static void SetValue(float a, bool c, string b = "Hello") => print($"{a},{b},{c}");

        [DebugMethod("Sample.SetValue")]
        private static void SetValue(RuntimePlatform platform) => print(platform.ToString());
    }
}