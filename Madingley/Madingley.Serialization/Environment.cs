
namespace Madingley.Serialization
{
    public static class Environment
    {
        public static string Serialize(Madingley.Common.Environment configuration)
        {
            return "";
        }

        public static Madingley.Common.Environment Deserialize(string json)
        {
            return new Madingley.Common.Environment();
        }
    }
}
