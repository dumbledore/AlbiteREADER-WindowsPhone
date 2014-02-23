namespace Albite.Reader.Core.Serialization
{
    public interface IClientNotifier
    {
        /// <summary>
        /// Send a message to the client
        /// </summary>
        /// <param name="message">Serialized message</param>
        /// <returns></returns>
        string NotifyClient(string message);
    }
}
