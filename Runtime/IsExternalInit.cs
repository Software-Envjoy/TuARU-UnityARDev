using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Workaround for C# 9 init only setters feature.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit
    {
    }
}