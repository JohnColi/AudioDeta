using UnityEngine;

/// <summary>
/// Android 記憶體空間
/// </summary>
public static class DiskUtils
{
    static AndroidJavaClass androidJavaClass
    {
        get
        {
            if (jc == null)
                init();
            return jc;
        }
    }
    static AndroidJavaClass jc = null;

    public static void init()
    {
        Debug.Log("DiskUtils.init()");
        if (jc != null)
        {
            return;
        }
        jc = new AndroidJavaClass("com.u2a.sdk.DiskUtils");
    }

    public static int TotalSpace(bool external)
    {
        int i = androidJavaClass.CallStatic<int>("totalSpace", external);
        string str = external ? "external" : "internal" ;
        Debug.Log(string.Format("TotalSpace:{0} , size:{1}", str, i));
        return i;
    }

    public static int FreeSpace(bool external)
    {
        int i = androidJavaClass.CallStatic<int>("freeSpace", external);
        string str = external ? "external" : "internal" ;
        Debug.Log(string.Format("FreeSpace:{0} , size:{1}", str, i));
        return i;
    }

    public static int BusySpace(bool external)
    {
        int i = androidJavaClass.CallStatic<int>("busySpace", external);
        string str = external ? "external" : "internal";
        Debug.Log(string.Format("BusySpace:{0} , size:{1}", str, i));
        return i;
    }
}
