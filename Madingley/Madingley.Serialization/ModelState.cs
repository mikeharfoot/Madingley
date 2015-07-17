
namespace Madingley.Serialization
{
    public static class ModelState
    {
        public static string Serialize(Madingley.Common.ModelState configuration)
        {
            return "";
        }

        public static Madingley.Common.ModelState Deserialize(string json)
        {
            return new Madingley.Common.ModelState();
        }
    }
}
