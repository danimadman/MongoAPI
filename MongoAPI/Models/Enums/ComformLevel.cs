using System.ComponentModel;

namespace MongoAPI.Models.Enums
{
    public enum ComformLevelEnum
    {
        [Description("Обычный")]
        Обычный = 1,
        [Description("Полулюкс")]
        Полулюкс,
        [Description("Люкс")]
        Люкс
    }
}