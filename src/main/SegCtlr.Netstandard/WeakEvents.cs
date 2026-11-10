using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Plugin.Segmented;

/// <summary>
/// Subscribes to <see cref="INotifyPropertyChanged"/> and <see cref="INotifyCollectionChanged"/> without the source
/// keeping the subscriber alive. View-model items and collections usually outlive the controls bound to them; a
/// normal subscription would keep every control (and its renderer) that ever displayed them in memory.
/// </summary>
/// <remarks>
/// Pass a <c>static</c> lambda as the handler: it receives the target, so it must not capture it.
/// </remarks>
internal static class WeakEvents
{
    public static IDisposable SubscribePropertyChanged<TTarget>(
        INotifyPropertyChanged source,
        TTarget target,
        Action<TTarget, PropertyChangedEventArgs> handler)
        where TTarget : class
    {
        var listener = new Listener<TTarget, PropertyChangedEventArgs>(target, handler);
        PropertyChangedEventHandler forward = listener.OnEvent;
        source.PropertyChanged += forward;
        listener.Unsubscribe = () => source.PropertyChanged -= forward;
        return listener;
    }

    public static IDisposable SubscribeCollectionChanged<TTarget>(
        INotifyCollectionChanged source,
        TTarget target,
        Action<TTarget, NotifyCollectionChangedEventArgs> handler)
        where TTarget : class
    {
        var listener = new Listener<TTarget, NotifyCollectionChangedEventArgs>(target, handler);
        NotifyCollectionChangedEventHandler forward = listener.OnEvent;
        source.CollectionChanged += forward;
        listener.Unsubscribe = () => source.CollectionChanged -= forward;
        return listener;
    }

    private sealed class Listener<TTarget, TArgs>(TTarget target, Action<TTarget, TArgs> handler) : IDisposable
        where TTarget : class
    {
        private readonly WeakReference<TTarget> _target = new(target);

        public Action Unsubscribe { get; set; }

        public void OnEvent(object _, TArgs e)
        {
            if (_target.TryGetTarget(out var target))
            {
                handler(target, e);
            }
            else
            {
                // The subscriber has been collected; drop the subscription the next time the source raises.
                Dispose();
            }
        }

        public void Dispose()
        {
            Unsubscribe?.Invoke();
            Unsubscribe = null;
        }
    }
}
