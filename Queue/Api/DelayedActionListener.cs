namespace EventHorizon.Basic.Bot.Queue.Api
{

    public interface DelayedActionListener<E>
    {
        void ActionPerformed(E item);
    }
}