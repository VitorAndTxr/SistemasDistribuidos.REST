using System.Threading.Channels;

public static class SseMessageChannel
{
    // Canal de string, mas pode ser JSON ou outro formato.
    public static Channel<string> MessageChannel = Channel.CreateUnbounded<string>();
}

