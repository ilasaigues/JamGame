using UnityEngine;

namespace AstralCore
{
    public class LogCategory
    {
        public string Name;
        public Color Color;
        public LogCategory(string name, Color color)
        {
            this.Name = name;
            this.Color = color;
        }

        public string Text => $"<b><color=#{Color.ToHex()}>{Name}:</color></b>";

        public static LogCategory Message = new LogCategory("Message", Color.white);
        public static LogCategory Animation = new LogCategory("Animation", new Color(0, 128, 128));
        public static LogCategory Localization = new LogCategory("Localization", Color.red);

    }

    public static class Logger
    {


        public static void Log(LogCategory logCategory, object message)
        {
            Debug.Log($"{logCategory.Text} {message.ToString()}");
        }
        public static void Log(object message)
        {
            Log(LogCategory.Message, message);
        }

        public static void LogWarning(LogCategory logCategory, object message)
        {
            Debug.LogWarning($"<color={logCategory.Color}>{logCategory.Name}:</color> {message.ToString()}");
        }

        public static void LogWarning(object message)
        {
            LogWarning(LogCategory.Message, message);
        }

        public static void LogError(LogCategory logCategory, object message)
        {
            Debug.LogError($"<color={logCategory.Color}>{logCategory.Name}:</color> {message.ToString()}");
        }
        public static void LogError(object message)
        {
            LogError(LogCategory.Message, message);
        }
    }
}
