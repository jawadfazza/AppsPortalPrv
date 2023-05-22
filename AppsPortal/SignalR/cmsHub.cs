using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace AppsPortal.SignalR
{
    public class cmsHub : Hub<IClient>
    {

        public void JoinGroup(string userGUID, string groupName)
        {
            Groups.Add(userGUID, groupName);
        }

        public override Task OnConnected()
        {

            //JoinGroup("UserGuidFromSession", "OnlineUsers");
            Guid connectedUserGuid = Guid.Parse(Context.ConnectionId);
            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current JobTitle ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the JobTitle is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current JobTitle id and user name.
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            // Add your own code here.
            // For example: in a chat application, you might have marked the
            // user as offline after a period of inactivity; in that case 
            // mark the user as online again.
            return base.OnReconnected();
        }

        public void operationSettingsReload(Guid OperationGUID, string message, string callback)
        {
            Clients.Others.operationSettingsReload(OperationGUID, message, callback);
        }

        public void Concurrency(string Message)
        {
            Clients.Others.Concurrency("Data has been modified, please reload the page, Parameter Value = " + Message);
        }


        public void testPopUp(string data)
        {
            Clients.Caller.showPopUp("message to user");
        }
        public void SendNotification(string author, string message)
        {
            Clients.All.broadcastNotification(author, message);
        }
    }

    public interface IClient
    {
        void Concurrency(string message);
        void operationSettingsReload(Guid operationGUID, string message, string callBack);

        void broadcastNotification(string author,string message);
        void showPopUp(string messageToUser);
    }

   
}