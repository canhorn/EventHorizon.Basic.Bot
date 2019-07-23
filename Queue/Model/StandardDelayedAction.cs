using System;
using EventHorizon.Basic.Bot.Queue.Api;

namespace EventHorizon.Basic.Bot.Queue.Model
{
    public class StandardDelayedAction : DelayedActionListener<string>
    {
        readonly Action<string> _send;
        public StandardDelayedAction(
            Action<string> send
        )
        {
            _send = send;
        }

        public void ActionPerformed(
            string item
        )
        {
            _send(item);
        }
    }
}