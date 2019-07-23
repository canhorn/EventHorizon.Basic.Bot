using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Basic.Bot.Queue.Api;

namespace EventHorizon.Basic.Bot.Queue.Model
{
    public class DelayedActionQueue<E>
    {
        private DelayedActionListener<E> _listener;
        private int _delay;
        private BlockingCollection<E> _queue = new BlockingCollection<E>();

        /// <summary>
        /// Create a new queue object and start it.
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static DelayedActionQueue<E> Create(
            DelayedActionListener<E> listener, 
            int delay
        )
        {
            var q = new DelayedActionQueue<E>(
                listener, 
                delay
            );
            q.Start();
            return q;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="listener">The listener to send the items to (can't be {@code null})</param>
        /// <param name="delay">The delay between items in milliseconds</param>
        private DelayedActionQueue(
            DelayedActionListener<E> listener, 
            int delay
        )
        {
            this._listener = listener;
            this._delay = delay;
        }

         /// <summary>
         /// Start a new reader thread. This should only be called once per instance.
         /// </summary>
        private void Start()
        {
            Task.Factory.StartNew(
                () =>
                {
                    this.Run();
                }
            );
        }
        public void Run()
        {
            while (true)
            {
                try
                {
                    E item;
                    if (_queue.TryTake(
                        out item
                    ))
                    {
                        _listener.ActionPerformed(
                            item
                        );
                    }
                    Thread.Sleep(
                        _delay
                    );
                }
                catch (
                    Exception ex
                )
                {
                    System.Console.WriteLine(
                        "[ERROR]: Reader Thread interrupted.{0}", 
                        ex
                    );
                }
            }
        }

        /// <summary>
        /// Adds an item to the queue.
        /// </summary>
        /// <param name="item"></param>
        public void Add(
            E item
        )
        {
            _queue.TryAdd(
                item
            );
        }

        /// <summary>
        /// Clears all elements from the queue.
        /// </summary>
        public void Clear()
        {
            _queue.Clear();
        }
    }
}