using System.Runtime.InteropServices;
using System.Text;

namespace Gnomicon;

/// <summary>
/// Manages desktop icon positions using Windows API calls.
/// </summary>
public class DesktopIconManager
{
    private const int LVM_FIRST = 0x1000;
    private const int LVM_GETITEMCOUNT = LVM_FIRST + 4;
    private const int LVM_GETITEMPOSITION = LVM_FIRST + 16;
    private const int LVM_SETITEMPOSITION = LVM_FIRST + 15;
    private const int LVM_GETITEMTEXT = LVM_FIRST + 45;
    
    private const uint WM_GETTEXT = 0x000D;
    private const uint WM_GETTEXTLENGTH = 0x000E;

    private const uint PROCESS_VM_OPERATION = 0x0008;
    private const uint PROCESS_VM_READ = 0x0010;
    private const uint PROCESS_VM_WRITE = 0x0020;
    private const uint MEM_COMMIT = 0x1000;
    private const uint MEM_RELEASE = 0x8000;
    private const uint PAGE_READWRITE = 0x04;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string? lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string? windowName);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, ref POINT lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, out POINT lpBuffer, uint dwSize, out uint lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, uint dwSize, out uint lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, ref POINT lpBuffer, uint dwSize, out uint lpNumberOfBytesWritten);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct LVITEM
    {
        public uint mask;
        public int iItem;
        public int iSubItem;
        public uint state;
        public uint stateMask;
        public IntPtr pszText;
        public int cchTextMax;
        public int iImage;
        public IntPtr lParam;
        public int iIndent;
    }

    private IntPtr _desktopHandle = IntPtr.Zero;
    private uint _desktopProcessId = 0;

    public bool Initialize()
    {
        _desktopHandle = GetDesktopListViewHandle();
        if (_desktopHandle == IntPtr.Zero)
            return false;

        GetWindowThreadProcessId(_desktopHandle, out _desktopProcessId);
        return _desktopProcessId != 0;
    }

    private IntPtr GetDesktopListViewHandle()
    {
        IntPtr progman = FindWindow("Progman", null);
        if (progman != IntPtr.Zero)
        {
            IntPtr shellView = FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (shellView != IntPtr.Zero)
            {
                IntPtr listView = FindWindowEx(shellView, IntPtr.Zero, "SysListView32", null);
                if (listView != IntPtr.Zero)
                    return listView;
            }
        }

        IntPtr workerW = IntPtr.Zero;
        do
        {
            workerW = FindWindowEx(IntPtr.Zero, workerW, "WorkerW", null);
            if (workerW != IntPtr.Zero)
            {
                IntPtr shellView = FindWindowEx(workerW, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellView != IntPtr.Zero)
                {
                    IntPtr listView = FindWindowEx(shellView, IntPtr.Zero, "SysListView32", null);
                    if (listView != IntPtr.Zero)
                        return listView;
                }
            }
        } while (workerW != IntPtr.Zero);

        return IntPtr.Zero;
    }

    public int GetIconCount()
    {
        if (_desktopHandle == IntPtr.Zero)
            return 0;

        return SendMessage(_desktopHandle, LVM_GETITEMCOUNT, 0, 0);
    }

    public IconPosition? GetIconPosition(int index)
    {
        if (_desktopHandle == IntPtr.Zero || _desktopProcessId == 0)
            return null;

        IntPtr hProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, _desktopProcessId);
        if (hProcess == IntPtr.Zero)
            return null;

        try
        {
            IntPtr remotePoint = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)Marshal.SizeOf<POINT>(), MEM_COMMIT, PAGE_READWRITE);
            if (remotePoint == IntPtr.Zero)
                return null;

            try
            {
                int result = SendMessage(_desktopHandle, LVM_GETITEMPOSITION, index, remotePoint);
                if (result == 0)
                    return null;

                POINT point;
                if (!ReadProcessMemory(hProcess, remotePoint, out point, (uint)Marshal.SizeOf<POINT>(), out _))
                    return null;

                return new IconPosition
                {
                    Index = index,
                    X = point.X,
                    Y = point.Y
                };
            }
            finally
            {
                VirtualFreeEx(hProcess, remotePoint, 0, MEM_RELEASE);
            }
        }
        finally
        {
            CloseHandle(hProcess);
        }
    }

    public bool SetIconPosition(int index, int x, int y)
    {
        if (_desktopHandle == IntPtr.Zero)
            return false;

        int lParam = (y << 16) | (x & 0xFFFF);
        int result = SendMessage(_desktopHandle, LVM_SETITEMPOSITION, index, lParam);
        
        return result != 0;
    }

    public List<IconPosition> GetAllIconPositions()
    {
        var positions = new List<IconPosition>();
        int count = GetIconCount();

        for (int i = 0; i < count; i++)
        {
            var pos = GetIconPosition(i);
            if (pos != null)
            {
                positions.Add(pos);
            }
        }

        return positions;
    }

    public void SetIconPositions(List<IconPosition> positions)
    {
        foreach (var pos in positions)
        {
            SetIconPosition(pos.Index, pos.X, pos.Y);
        }
    }

    public void Refresh()
    {
        Initialize();
    }
}
