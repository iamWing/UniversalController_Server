using AlphaOwl.UniversalController.Utilities;

namespace AlphaOwl.UniversalController
{
    public class UCServer : NetworkUtilities.IMessageReceiver,
    NetworkUtilities.IMessageSender
    {
        /* Override methods from NetworkUtilities.IMessageReceiver */

        public void OnReceiveComplete(string msg)
        {

        }

        public void OnReceiveFail(string err)
        {

        }

        /* Override methods from NetworkUtilities.IMessageSender */

        public void OnSendComplete()
        {

        }

        public void OnSendFail(string err)
        {

        }
    }
}
