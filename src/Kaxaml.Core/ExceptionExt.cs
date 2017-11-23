using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Kaxaml.Core
{
    /// <summary>
    /// This class provides some helper methods for Exceptions. -> PixelLab.Common
    /// https://github.com/thinkpixellab/bot
    /// </summary>
    public static class ExceptionExt
    {
        /// <summary>
        /// Returns true if the provided <see cref="Exception"/> is considered 'critical'
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to evaluate for critical-ness.</param>
        /// <returns>true if the Exception is conisdered critical; otherwise, false.</returns>
        /// <remarks>
        /// These exceptions are consider critical:
        /// <list type="bullets">
        ///     <item><see cref="OutOfMemoryException"/></item>
        ///     <item><see cref="StackOverflowException"/></item>
        ///     <item><see cref="ThreadAbortException"/></item>
        ///     <item><see cref="SEHException"/></item>
        /// </list>
        /// </remarks>
        public static bool IsCriticalException(this Exception exception)
        {
            // Copied with respect from WPF WindowsBase->MS.Internal.CriticalExceptions.IsCriticalException
            // NullReferencException, SecurityException --> not going to consider these critical
            while (exception != null)
            {
                if (exception is OutOfMemoryException ||
                    exception is StackOverflowException ||
                    exception is ThreadAbortException
#if !WP7
                    || exception is System.Runtime.InteropServices.SEHException
#endif
                )
                {
                    return true;
                }
                exception = exception.InnerException;
            }
            return false;
        }
    }
}