using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base
{
    public enum LoginType
    {
        H264 = 0,
        RAW = 1,
    }

    public enum StreamType
    {
        PRI = 0,
        SUB = 1,
    }

    public enum SnapType
    {
        ISP = 0,
        OSD = 1,
        T = 2,
    }

    public enum UserFormType
    {
        Add,
        Update,
    }
}
