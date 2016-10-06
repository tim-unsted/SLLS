using System.Linq;
using System.Net;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace slls.Hubs
{
    public class ProgressHub : Hub
    {
        public string msg = "Initializing and Preparing...";
        public int count = 0;
        
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        
        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;

            _connections.Add(name, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;

            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
        
        // something to start the progress notifications ...
        public static void NotifyStart(string msg, string who)
        {
            string name = HttpContext.Current.User.Identity.Name;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            foreach (var connectionId in _connections.GetConnections(who))
            {
                hubContext.Clients.Client(connectionId).initProgress(string.Format(msg));
            }
        }
        
        // update the progress ...
        public static void NotifyProgress(string msg, int count, string who)
        {
            string name = HttpContext.Current.User.Identity.Name;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            foreach (var connectionId in _connections.GetConnections(who))
            {
                hubContext.Clients.Client(connectionId).updateProgress(string.Format(msg), count);
            }
        }

        // something to send a current value (string) to the client ...
        public static void SendCurrentValue(string currentValue, string who)
        {
            string name = HttpContext.Current.User.Identity.Name;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            foreach (var connectionId in _connections.GetConnections(who))
            {
                hubContext.Clients.Client(connectionId).sendCurrentValue(currentValue);
            }
        }

        // something to end the progress notifications
        public static void NotifyEnd(string msg, string who)
        {
            string name = HttpContext.Current.User.Identity.Name;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            foreach (var connectionId in _connections.GetConnections(who))
            {
                hubContext.Clients.Client(connectionId).clearProgress(string.Format(msg));
            }
        }

        // ad-hoc messages, nothing to do with progress ...
        public static void SendMessage(string msg, string who)
        {
            string name = HttpContext.Current.User.Identity.Name;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            foreach (var connectionId in _connections.GetConnections(who))
            {
                hubContext.Clients.Client(connectionId).sendMessage(msg);
            }
        }
        
        // really just a duplicate of NotifyProgress - probably not needed.
        public static void GetCountAndMessage(string msg, int count, string who)
        {
            string name = HttpContext.Current.User.Identity.Name;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            foreach (var connectionId in _connections.GetConnections(who))
            {
                hubContext.Clients.Client(connectionId).sendMessage(string.Format(msg), count);
            }
        }

       //something to stop the connection ...
        public static void Stop(string msg, int count, int id, string who)
        {
            string name = HttpContext.Current.User.Identity.Name;
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();

            foreach (var connectionId in _connections.GetConnections(who))
            {
                hubContext.Clients.Client(connectionId).stopClient(string.Format(msg), count, id);
            }
        }

    }
}
