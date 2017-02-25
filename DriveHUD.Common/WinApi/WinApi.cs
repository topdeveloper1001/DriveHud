﻿//-----------------------------------------------------------------------
// <copyright file="WinApi.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace DriveHUD.Common.WinApi
{
    [SuppressUnmanagedCodeSecurity]
    /// <summary>
    /// WinApi methods 
    /// </summary>
    public class WinApi
    {
        #region Modules/Procs

        /// <summary>
        /// Retrieve a module handle for the specified module. The module must have been loaded by the calling process.
        /// </summary>
        /// <param name="lpModuleName">The name of the loaded module.</param>
        /// <returns>If the function succeeds, a handle to the module is returned. Otherwise, <see cref="IntPtr.Zero"/> is returned. Call <see cref="Marshal.GetLastWin32Error"/> on failure to get last Win32 error</returns>
        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true)]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

        /// <summary>
        /// Retrieves the address of an exported function from the specified Dll.
        /// </summary>
        /// <param name="hModule">Handle to the Dll module that contains the exported function</param>
        /// <param name="procName">The function name.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        /// <summary>
        /// Frees the loaded Dll module.
        /// </summary>
        /// <param name="hModule">Handle to the loaded library to free</param>
        /// <returns>True if the function succeeds, otherwise false. Call <see cref="Marshal.GetLastWin32Error"/> on failure to get Win32 error.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// Retrieves the calling thread's last-error code value.
        /// </summary>
        /// <returns>The return value is the calling thread's last-error code value.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetLastError();

        /// <summary>
        /// Enumerates all modules in process
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="lphModule"></param>
        /// <param name="cb"></param>
        /// <param name="lpcbNeeded"></param>
        /// <returns></returns>
        [DllImport("psapi.dll", SetLastError = true)]
        public static extern bool EnumProcessModules(IntPtr hProcess, [Out] IntPtr lphModule, uint cb, [MarshalAs(UnmanagedType.U4)] out uint lpcbNeeded);

        /// <summary>
        /// Get file name of specified module
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="hModule"></param>
        /// <param name="lpBaseName"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        #endregion

        #region Thread

        /// <summary>
        /// Create a thread that runs in the virtual address space of another process
        /// </summary>
        /// <param name="hProcess">A handle to the process in which the thread is to be created</param>
        /// <param name="lpThreadAttributes">A pointer to a SECURITY_ATTRIBUTES structure that specifies a security descriptor for the new thread and determines whether child processes can inherit the returned handle.</param>
        /// <param name="dwStackSize">The initial size of the stack, in bytes. The system rounds this value to the nearest page. If this parameter is 0 (zero), the new thread uses the default size for the executable.</param>
        /// <param name="lpStartAddress">A pointer to the application-defined function of type LPTHREAD_START_ROUTINE to be executed by the thread and represents the starting address of the thread in the remote process. The function must exist in the remote process.</param>
        /// <param name="lpParameter">A pointer to a variable to be passed to the thread function</param>
        /// <param name="dwCreationFlags">The flags that control the creation of the thread</param>
        /// <param name="lpThreadId">A pointer to a variable that receives the thread identifier. If this parameter is <see cref="IntPtr.Zero"/>, the thread identifier is not returned.</param>
        /// <returns>If the function succeeds, the return value is a handle to the new thread. If the function fails, the return value is <see cref="IntPtr.Zero"/>. Call <see cref="Marshal.GetLastWin32Error"/> to get Win32 Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "CreateRemoteThread", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttributes,
            uint dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            [Out] IntPtr lpThreadId);

        /// <summary>
        /// Waits until the specified object is in the signaled state or the time-out interval elapses.
        /// </summary>
        /// <param name="hObject">A handle to the object. For a list of the object types whose handles can be specified, see the following Remarks section.</param>
        /// <param name="dwMilliseconds">The time-out interval, in milliseconds. The function returns if the interval elapses, even if the object's state is nonsignaled. If dwMilliseconds is zero, the function tests the object's state and returns immediately. If dwMilliseconds is INFINITE, the function's time-out interval never elapses.</param>
        /// <returns>If the function succeeds, the return value indicates the event that caused the function to return. If the function fails, the return value is WAIT_FAILED ((DWORD)0xFFFFFFFF).</returns>
        [DllImport("kernel32", EntryPoint = "WaitForSingleObject")]
        public static extern uint WaitForSingleObject(IntPtr hObject, uint dwMilliseconds);

        /// <summary>
        /// Retrieves the termination status of the specified thread.
        /// </summary>
        /// <param name="hThread">Handle to the thread</param>
        /// <param name="lpExitCode">A pointer to a variable to receive the thread termination status. If this works properly, this should be the return value from the thread function of <see cref="CreateRemoteThread"/></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeThread(IntPtr hThread, out IntPtr lpExitCode);

        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// Enumerates all non-child windows associated with a thread by passing the handle to each window 
        /// </summary>
        /// <param name="dwThreadId">The identifier of the thread whose windows are to be enumerated</param>
        /// <param name="lpfn">A pointer to an application-defined callback function</param>
        /// <param name="lParam">An application-defined value to be passed to the callback function</param>
        /// <returns>If the callback function returns TRUE for all windows in the thread specified by dwThreadId, the return value is TRUE. 
        /// If the callback function returns FALSE on any enumerated window, or if there are no windows found in the thread specified by dwThreadId, the return value is FALSE.</returns>
        [DllImport("user32.dll")]
        public static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        #endregion

        #region Memory

        /// <summary>
        /// Reserves or commits a region of memory within the virtual address space of a specified process.
        /// The function initializes the memory it allocates to zero, unless <see cref="AllocationType.Reset"/> is used.
        /// </summary>
        /// <param name="hProcess">The handle to a process. The function allocated memory within the virtual address space of this process.
        /// The process must have the <see cref="ProcessAccessFlags.VMOperation"/> access right.</param>
        /// <param name="lpAddress">Optional desired address to begin allocation from. Use <see cref="IntPtr.Zero"/> to let the function determine the address</param>
        /// <param name="dwSize">The size of the region of memory to allocate, in bytes</param>
        /// <param name="flAllocationType">
        /// <see cref="AllocationType"/> type of allocation. Must contain one of <see cref="AllocationType.Commit"/>, <see cref="AllocationType.Reserve"/> or <see cref="AllocationType.Reset"/>.
        /// Can also specify <see cref="AllocationType.LargePages"/>, <see cref="AllocationType.Physical"/>, <see cref="AllocationType.TopDown"/>.
        /// </param>
        /// <param name="flProtect">One of <see cref="MemoryProtection"/> constants.</param>
        /// <returns>If the function succeeds, the return value is the base address of the allocated region of pages.
        /// If the function fails, the return value is <see cref="IntPtr.Zero"/>. Call <see cref="Marshal.GetLastWin32Error"/> to get Win32 error.</returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            AllocationType flAllocationType,
            MemoryProtection flProtect);

        /// <summary>
        /// Releases, decommits, or releases and decommits a region of memory within the virtual address space of a specified process
        /// </summary>
        /// <param name="hProcess">A handle to a process. The function frees memory within the virtual address space of this process.
        /// The handle must have the <see cref="ProcessAccessFlags.VMOperation"/> access right</param>
        /// <param name="lpAddress">A pointer to the starting address of the region of memory to be freed.
        /// If the <paramref name="dwFreeType"/> parameter is <see cref="AllocationType.Release"/>, this address must be the base address
        /// returned by <see cref="VirtualAllocEx"/>.</param>
        /// <param name="dwSize">The size of the region of memory to free, in bytes.
        /// If the <paramref name="dwFreeType"/> parameter is <see cref="AllocationType.Release"/>, this parameter must be 0. The function
        /// frees the entire region that is reserved in the initial allocation call to <see cref="VirtualAllocEx"/></param>
        /// <param name="dwFreeType">The type of free operation. This parameter can be one of the following values: 
        /// <see cref="AllocationType.Decommit"/> or <see cref="AllocationType.Release"/></param>
        /// <returns>If the function is successful, it returns true. If the function fails, it returns false. Call <see cref="Marshal.GetLastWin32Error"/> to get Win32 error.</returns>
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualFreeEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            uint dwSize,
            AllocationType dwFreeType);

        /// <summary>
        /// Writes data to an area of memory in a specified process.
        /// </summary>
        /// <param name="hProcess">Handle to the process to write memory to.
        /// The handle must have <see cref="ProcessAccessFlags.VMWrite"/> and <see cref="ProcessAccessFlags.VMOperation"/> access to the process</param>
        /// <param name="lpBaseAddress">A pointer to the base address to write to in the specified process</param>
        /// <param name="lpBuffer">A pointer to a buffer that contains the data to be written</param>
        /// <param name="nSize">The number of bytes to write</param>
        /// <param name="lpNumberOfBytesWritten">The number of bytes written.</param>
        /// <returns>If the function succeeds, it returns true. Otherwise false is returned and calling <see cref="Marshal.GetLastWin32Error"/> will retrieve the error.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            uint nSize,
            out int lpNumberOfBytesWritten);

        /// <summary>
        /// Writes data to an area of memory in a specified process.
        /// </summary>
        /// <param name="hProcess">Handle to the process to write memory to.
        /// The handle must have <see cref="ProcessAccessFlags.VMWrite"/> and <see cref="ProcessAccessFlags.VMOperation"/> access to the process</param>
        /// <param name="lpBaseAddress">A pointer to the base address to write to in the specified process</param>
        /// <param name="lpBuffer">A pointer to a buffer that contains the data to be written</param>
        /// <param name="nSize">The number of bytes to write</param>
        /// <param name="lpNumberOfBytesWritten">The number of bytes written.</param>
        /// <returns>If the function succeeds, it returns true. Otherwise false is returned and calling <see cref="Marshal.GetLastWin32Error"/> will retrieve the error.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            IntPtr lpBuffer,
            uint nSize,
            out int lpNumberOfBytesWritten);

        #endregion

        #region Handle

        /// <summary>
        /// Close an open handle
        /// </summary>
        /// <param name="hObject">Object handle to close</param>
        /// <returns>True if success, false if failure. Use <see cref="Marshal.GetLastWin32Error"/> on failure to get Win32 error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        #endregion

        #region Processes

        /// <summary>
        /// Open process and retrieve handle for manipulation
        /// </summary>
        /// <param name="dwDesiredAccess"><see cref="ProcessAccessFlags"/> for external process.</param>
        /// <param name="bInheritHandle">Indicate whether to inherit a handle.</param>
        /// <param name="dwProcessId">Unique process ID of process to open</param>
        /// <returns>Returns a handle to opened process if successful or <see cref="IntPtr.Zero"/> if unsuccessful.
        /// Use <see cref="Marshal.GetLastWin32Error" /> to get Win32 Error on failure</returns>
        [DllImport("kernel32.dll", EntryPoint = "OpenProcess", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            ProcessAccessFlags dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            int dwProcessId);

        /// <summary>
        /// Creates a new process and its primary thread. The new process runs in the security context of the calling process.
        /// </summary>
        /// <param name="lpApplicationName">The name of the module to be executed. The string can specify the full path and file name of hte module to execute
        /// or it can specify a partial name.</param>
        /// <param name="lpCommandLine">The command line to be executed.</param>
        /// <param name="lpProcessAttributes">A pointer to a SECURITY_ATTRIBUTES structure that determines whether the returned handle to the new process object can be inherited by child processes. If lpProcessAttributes is <see cref="IntPtr.Zero"/>, the handle cannot be inherited.</param>
        /// <param name="lpThreadAttributes">A pointer to a SECURITY_ATTRIBUTES structure that determines whether the returned handle to the new thread object can be inherited by child processes. If lpThreadAttributes is <see cref="IntPtr.Zero"/>, the handle cannot be inherited.</param>
        /// <param name="bInheritHandles">If this parameter is true, each inheritable handle in the calling process is inherited by the new process. If the parameter is FALSE, the handles are not inherited. Note that inherited handles have the same value and access rights as the original handles.</param>
        /// <param name="dwCreationFlags">The flags that control the priority class and the creation of the process. See <see cref="ProcessCreationFlags"/></param>
        /// <param name="lpEnvironment">A pointer to the environment block for the new process. If this parameter is <see cref="IntPtr.Zero"/>, the new process uses the environment of the calling process.</param>
        /// <param name="lpCurrentDirectory">The full path to the current directory for the process. The string can also specify a UNC path.</param>
        /// <param name="lpStartupInfo">A pointer to a <see cref="STARTUPINFO"/> structure.</param>
        /// <param name="lpProcessInformation">A pointer to a <see cref="PROCESS_INFORMATION"/> structure that receives identification information about the new process.</param>
        /// <returns>If the function succeeds, the return value is true. If the function fails, the return value is false. Call <see cref="Marshal.GetLastWin32Error"/> to get the Win32 Error.</returns>
        [DllImport("kernel32.dll", EntryPoint = "CreateProcessW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcess(
            [MarshalAs(UnmanagedType.LPWStr)]
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            ProcessCreationFlags dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref StartupInfo lpStartupInfo,
            out ProcessInformation lpProcessInformation);

        #endregion

        #region Security

        /// <summary>
        /// Sets the security descriptor attributes.
        /// </summary>
        /// <param name="sd">Reference to a <see cref="SecurityDescriptor" /> structure.</param>
        /// <param name="bDaclPresent">A flag that indicates the presence of the DACL in the security descriptor.</param>
        /// <param name="Dacl">A pointer to an ACL structure that specifies the DACL for the security descriptor.</param>
        /// <param name="bDaclDefaulted">A flag that indicates the source of the DACL.</param>
        /// <returns>If the function succeeds, the function returns nonzero.</returns>
        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern bool SetSecurityDescriptorDacl(
            ref SecurityDescriptor sd,
            bool bDaclPresent,
            IntPtr Dacl,
            bool bDaclDefaulted);

        /// <summary>
        /// Initializes a SecurityDescriptor structure.
        /// </summary>
        /// <param name="sd">Reference to a <see cref="SecurityDescriptor" /> structure that the function initializes.</param>
        /// <param name="dwRevision">The revision level to assign to the security descriptor. </param>
        /// <returns>If the function succeeds, the function returns nonzero.</returns>
        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern bool InitializeSecurityDescriptor(
            out SecurityDescriptor sd,
            int dwRevision);

        #endregion

        #region Native named pipes

        /// <summary>
        /// Creates an instance of a named pipe and returns a handle for 
        /// subsequent pipe operations.
        /// </summary>
        /// <param name="lpName">Pipe name</param>
        /// <param name="dwOpenMode">Pipe open mode</param>
        /// <param name="dwPipeMode">Pipe-specific modes</param>
        /// <param name="nMaxInstances">Maximum number of instances</param>
        /// <param name="nOutBufferSize">Output buffer size</param>
        /// <param name="nInBufferSize">Input buffer size</param>
        /// <param name="nDefaultTimeOut">Time-out interval</param>
        /// <param name="pipeSecurityAttributes">Security attributes</param>
        /// <returns>If the function succeeds, the return value is a handle 
        /// to the server end of a named pipe instance.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateNamedPipe(
            string lpName,
            PipeOpenMode dwOpenMode,
            PipeMode dwPipeMode,
            uint nMaxInstances,
            uint nOutBufferSize,
            uint nInBufferSize,
            uint nDefaultTimeOut,
            IntPtr pipeSecurityAttributes);

        /// <summary>
        /// Reads data from a file, starting at the position indicated by the
        /// file pointer.
        /// </summary>
        /// <param name="hHandle">Handle to the file to be read.</param>
        /// <param name="lpBuffer">Pointer to the buffer that receives the data
        ///  read from the file.</param>
        /// <param name="nNumberOfBytesToRead">Number of bytes to be read 
        /// from the file.</param>
        /// <param name="lpNumberOfBytesRead">Pointer to the variable that 
        /// receives the number of bytes read.</param>
        /// <param name="lpOverlapped">Pointer to an Overlapped object.
        /// </param>
        /// <returns>The ReadFile function returns when one of the following 
        /// conditions is met: a write operation completes on the write end 
        /// of the pipe, the number of bytes requested has been read, or an
        /// error occurs.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
            IntPtr hHandle,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            out uint lpNumberOfBytesRead,
            IntPtr lpOverlapped);

        /// <summary>
        /// Flushes the buffers of the specified file and causes all buffered
        /// data to be written to the file.
        /// </summary>
        /// <param name="hHandle">Handle to an open file.</param>
        /// <returns>If the function succeeds, the return value is nonzero. </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FlushFileBuffers(IntPtr hHandle);

        /// <summary>
        /// Disconnects the server end of a named pipe instance from a client
        /// process.
        /// </summary>
        /// <param name="hHandle">Handle to an instance of a named pipe.
        /// </param>
        /// <returns>If the function succeeds, the return value is nonzero. </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DisconnectNamedPipe(IntPtr hHandle);

        #endregion

        #region Windows

        /// <summary>
        /// Retrieves a handle to a window that has the specified relationship (Z-Order or owner) to the specified window
        /// </summary>
        /// <param name="hWnd">A handle to a window. The window handle retrieved is relative to this window, based on the value of the uCmd parameter.</param>
        /// <param name="uCmd">The relationship between the specified window and the window whose handle is to be retrieved.</param>
        /// <returns>If the function succeeds, the return value is a window handle. If no window exists with the specified relationship to the specified window, the return value is NULL</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

        /// <summary>
        /// Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a control, the text of the control is copied.         
        /// However, GetWindowText cannot retrieve the text of a control in another application.
        /// </summary>
        /// <param name="hWnd">A handle to the window or control containing the text</param>
        /// <param name="lpString">The buffer that will receive the text. If the string is as long or longer than the buffer, the string is truncated and terminated with a null character</param>
        /// <param name="nMaxCount">The maximum number of characters to copy to the buffer, including the null character. If the text exceeds this limit, it is truncated</param>
        /// <returns>If the function succeeds, the return value is the length, in characters, of the copied string, not including the terminating null character. 
        /// If the window has no title bar or text, if the title bar is empty, or if the window or control handle is invalid, the return value is zero</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Retrieves the length, in characters, of the specified window's title bar text (if the window has a title bar). If the specified window is a control, the function retrieves the length of the text within the control. 
        /// However, GetWindowTextLength cannot retrieve the length of the text of an edit control in another application
        /// </summary>
        /// <param name="hWnd">A handle to the window or control</param>
        /// <returns>If the function succeeds, the return value is the length, in characters, of the text. Under certain conditions, this value may actually be greater than the length of the text</returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        /// <summary>
        /// Get the text of the specified window's title bar. If the specified window is a control, the text of the control is returned.         
        /// However, GetWindowText cannot retrieve the text of a control in another application.
        /// </summary>
        /// <param name="hWnd">A handle to the window or control</param>
        /// <returns>If the function succeeds, the return value is the text if the specified window's title bar. If the window has no title bar or text, if the title bar is empty, or if the window or control handle is invalid, the return value is empty string</returns>
        public static string GetWindowText(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return string.Empty;
            }

            var textLength = GetWindowTextLength(hWnd);
            var sb = new StringBuilder(textLength + 1);

            if (GetWindowText(hWnd, sb, sb.Capacity) == 0)
            {
                return string.Empty;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Retrieves a handle to a window whose class name and window name match the specified strings.
        /// The function searches child windows, beginning with the one following the specified child window.
        /// This function does not perform a case-sensitive search.
        /// </summary>
        /// <param name="hwndParent">A handle to the parent window whose child windows are to be searched.</param>
        /// <param name="hwndChildAfter">A handle to a child window. The search begins with the next child window in the Z order. The child window must be a direct child window of hwndParent, not just a descendant window.</param>
        /// <param name="lpszClass">The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function. The atom must be placed in the low-order word of lpszClass; the high-order word must be zero.</param>
        /// <param name="lpszWindow">The window name (the window's title). If this parameter is NULL, all window names match.</param>
        /// <returns>If the function succeeds, the return value is a handle to the window that has the specified class and window names.
        /// If the function fails, the return value is NULL.To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        /// <summary>
        /// Retrieves the name of the class to which the specified window belongs.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="lpClassName">The class name string.</param>
        /// <param name="nMaxCount">The length of the lpClassName buffer, in characters. The buffer must be large enough to include the terminating null character; otherwise, the class name string is truncated to nMaxCount-1 characters.</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        #endregion
    }
}