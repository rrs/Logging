using YamlDotNet.Serialization;

namespace Rrs.Logging.Yaml
{
    public static class YamlConvert
    {
        private static Serializer _yamler = new Serializer();

        public static string SerializeObject(object o) => _yamler.Serialize(o);
    }
}
