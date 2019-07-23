using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using EventHorizon.Basic.Bot.Socket;

namespace EventHorizon.Basic.Bot.Client
{
    public interface IClientSendMessage 
    {
        Task SendMessage(
            string message
        );
    }
}