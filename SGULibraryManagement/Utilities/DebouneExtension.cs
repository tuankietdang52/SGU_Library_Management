using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Utilities
{
    public static class DebounceExtension
    {
        public static Action Debounce(this Action callback, int milisecond)
        {
            // init the cancel token source
            CancellationTokenSource? cancellationTokenSource = null;

            return () =>
            {
                /*
                 * Cancel the previous delay task and create new
                 * value are capture by Closure, so it will keep the value every call
                 * (Closure in C# for more information) 
                 */
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new();

                // if the task is complete (without cancel by debounce), then call the callback
                Task.Delay(milisecond, cancellationTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully) callback();
                    });
            };
        }

        public static Action<T> Debounce<T>(this Action<T> callback, int milisecond)
        {
            CancellationTokenSource? cancellationTokenSource = null;

            return (arg) =>
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new();

                Task.Delay(milisecond, cancellationTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully) callback(arg);
                    });
            };
        }

        public static Action<T1, T2> Debounce<T1, T2>(this Action<T1, T2> callback, int milisecond)
        {
            CancellationTokenSource? cancellationTokenSource = null;

            return (arg1, arg2) =>
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new();

                Task.Delay(milisecond, cancellationTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully) callback(arg1, arg2);
                    });
            };
        }

        public static Action<T1, T2, T3> Debounce<T1, T2, T3>(this Action<T1, T2, T3> callback, int milisecond)
        {
            CancellationTokenSource? cancellationTokenSource = null;

            return (arg1, arg2, arg3) =>
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new();

                Task.Delay(milisecond, cancellationTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully) callback(arg1, arg2, arg3);
                    });
            };
        }

        public static Action<T1, T2, T3, T4> Debounce<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> callback, int milisecond)
        {
            CancellationTokenSource? cancellationTokenSource = null;

            return (arg1, arg2, arg3, arg4) =>
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new();

                Task.Delay(milisecond, cancellationTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully) callback(arg1, arg2, arg3, arg4);
                    });
            };
        }
    }
}
