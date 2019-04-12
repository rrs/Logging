using System;

namespace Rrs.Logging
{
    public interface IExceptionLogger
    {
        void Log(Exception e, string message);
        void Log(Exception e);
    }
}
