using System;
using System.Threading;

namespace qs
{
    /// <summary>
    /// This is an asynchronous event handler. It facilitates raising event asynchronously in a safe manner.
    /// </summary>
    [Serializable]
    public class AsyncEventHandler
    {
        #region Methods

        /// <summary>
        /// Attempt to raise the specified event asynchronously in a recognized safe manner.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventHandler">The method that will handle an event.</param>
        public static void TryRaise(object sender, EventHandler eventHandler)
        {
            TryRaise(sender, eventHandler, EventArgs.Empty);
        }

        /// <summary>
        /// Attempt to raise the specified event asynchronously in a recognized safe manner.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventHandler">The method that will handle an event.</param>
        /// <param name="eventArgs">An instance of type EventArgs.</param>
        public static void TryRaise(object sender, EventHandler eventHandler, EventArgs eventArgs)
        {
            EventHandler t = eventHandler;
            if (t != null)
            {
                InternalEventAction internalEventAction = new InternalEventAction
                {
                    EventArgs = eventArgs,
                    EventHandler = t,
                    Sender = sender
                };
                AsyncCallback callback = new AsyncCallback(InternalCallback);
                AsyncResult asyncResult = new AsyncResult(internalEventAction);
                callback.BeginInvoke(asyncResult, null, null);
            }
        }

        private static void InternalCallback(object o)
        {
            InternalEventAction iea = (o as IAsyncResult).AsyncState as InternalEventAction;
            EventHandler eventHandler = iea.EventHandler;
            var sender = iea.Sender;
            EventArgs eventArgs = iea.EventArgs;

            eventHandler?.Invoke(sender, eventArgs);
        }

        #endregion Methods

        #region Nested Types

        internal class AsyncResult : IAsyncResult
        {
            #region Constructors

            public AsyncResult(object asyncState)
            {
                AsyncState = asyncState;
            }

            #endregion Constructors

            #region Properties

            public object AsyncState
            {
                get;
                private set;
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool CompletedSynchronously
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsCompleted
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            #endregion Properties
        }

        internal class InternalEventAction<E> : InternalEventAction
            where E : EventArgs
        {
            #region Properties

            internal new E EventArgs
            {
                get;
                set;
            }

            internal new EventHandler<E> EventHandler
            {
                get;
                set;
            }

            #endregion Properties
        }

        internal class InternalEventAction
        {
            #region Properties

            internal EventArgs EventArgs
            {
                get;
                set;
            }

            internal EventHandler EventHandler
            {
                get;
                set;
            }

            internal object Sender
            {
                get;
                set;
            }

            #endregion Properties
        }

        #endregion Nested Types
    }

    /// <summary>
    /// This is a generic asynchronous event handler. It facilitates raising event asynchronously in a safe manner.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public class AsyncEventHandler<E> : AsyncEventHandler
        where E : EventArgs
    {
        #region Methods

        /// <summary>
        /// Attempt to raise the specified event asynchronously in a recognized safe manner.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventHandler">The method that will handle an event.</param>
        /// <param name="eventArgs">An instance of type &lt;E&gt; where E is a subclass of EventArgs.</param>
        public static void TryRaise(object sender, EventHandler<E> eventHandler, E eventArgs)
        {
            EventHandler<E> t = eventHandler;
            if (t != null)
            {
                InternalEventAction<E> internalEventAction = new InternalEventAction<E>
                {
                    EventArgs = eventArgs,
                    EventHandler = t,
                    Sender = sender
                };
                AsyncCallback callback = new AsyncCallback(InternalCallback);
                AsyncResult asyncResult = new AsyncResult(internalEventAction);
                callback.BeginInvoke(asyncResult, null, null);
            }
        }

        private static void InternalCallback(object o)
        {
            InternalEventAction<E> iea = (o as IAsyncResult).AsyncState as InternalEventAction<E>;
            EventHandler<E> eventHandler = iea.EventHandler;
            var sender = iea.Sender;
            E eventArgs = iea.EventArgs;

            eventHandler?.Invoke(sender, eventArgs);
        }

        #endregion Methods
    }
}