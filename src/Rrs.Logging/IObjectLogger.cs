namespace Rrs.Logging
{
    public interface IObjectLogger
    {
        void Log(object o);
        void Log(object o, string message);
    }
}
