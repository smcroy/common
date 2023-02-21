namespace qs.Extensions.EventHandlerExtensions
{
    using System;

    public static class EventHandlerExtensions
    {
        #region Methods

        /// <summary>
        /// Attempt to raise the specified event in a recognized safe manner.
        /// </summary>
        /// <param name="a">Represents the method that will handle an event that has no event data.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event data.</param>
        public static void TryRaise(this EventHandler a, object sender, EventArgs args)
        {
            EventHandler temp = a;
            a?.Invoke(sender, args);
        }

        public static void TryRaise(this EventHandler a, object sender)
        {
            a.TryRaise(sender, new EventArgs());
        }

        /// <summary>
        /// Attemp to raise the specified event in a recognized safe manner.
        /// </summary>
        /// <param name="a">Represents the method that will handle an event.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">Event data.</param>
        public static void TryRaise<T>(this EventHandler<T> a, object sender, T args)
            where T : EventArgs
        {
            EventHandler<T> temp = a;
            a?.Invoke(sender, args);
        }

        public static void TryRaise<T>(this EventHandler<T> a, object sender)
            where T : EventArgs
        {
            a.TryRaise(sender, default);
        }

        /// <summary>
        /// Attempt to raise the specified event asynchronously in a recognized safe manner.
        /// </summary>
        /// <param name="a">Represents the method that will handle an event that has no event data.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventHandler">The method that will handle an event.</param>
        public static void TryRaiseAsync(this EventHandler a, object sender)
        {
            AsyncEventHandler.TryRaise(sender, a);
        }

        /// <summary>
        /// Attempt to raise the specified event asynchronously in a recognized safe manner.
        /// </summary>
        /// <param name="a">Represents the method that will handle an event.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventHandler">The method that will handle an event.</param>
        /// <param name="eventArgs">An instance of type EventArgs.</param>
        public static void TryRaiseAsync(this EventHandler a, object sender, EventArgs args)
        {
            AsyncEventHandler.TryRaise(sender, a, args);
        }

        /// <summary>
        /// Attempt to raise the specified event asynchronously in a recognized safe manner.
        /// </summary>
        /// <param name="a">Represents the method that will handle an event.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventHandler">The method that will handle an event.</param>
        /// <param name="eventArgs">An instance of type &lt;E&gt; where E is a subclass of EventArgs.</param>
        public static void TryRaiseAsync<E>(this EventHandler<E> a, object sender, E args)
            where E : EventArgs
        {
            AsyncEventHandler<E>.TryRaise(sender, a, args);
        }

        #endregion Methods
    }
}