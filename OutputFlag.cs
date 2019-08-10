using System;

namespace LoggerLib
{
    [Flags]
    [Author("HANU")]
    public enum OutputFlag
    {
        File = 0b0001,
        Console = 0b0010,
        Debug = 0b0100,
        Trace = 0b1000
    }
}
