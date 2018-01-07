using System;
using System.Collections.Generic;

namespace AlphaOwl.UniversalController.Utilities
{
    public interface IDispatcher
    {
        void Invoke(Action fn);
    }

    public class Dispatcher
    {
        public List<Action> pending = new List<Action>();

        /// <summary>
        /// Schedule code for execution in the thread.
        /// </summary>
        /// <param name="fn">Function that needs to be 
        /// invoked.</param>
        public void Invoke(Action fn)
        {
            lock (pending)
            {
                pending.Add(fn);
            }
        }

        /// <summary>
        /// Execute pending actions.
        /// </summary>
        public void InvokePending()
        {
            lock(pending)
            {
                foreach (var action in pending)
                    action();

                pending.Clear();
            }
        }
    }
}
