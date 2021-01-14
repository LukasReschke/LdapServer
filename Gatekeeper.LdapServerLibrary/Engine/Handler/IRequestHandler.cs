using System.Threading.Tasks;
using Gatekeeper.LdapServerLibrary.PacketParser.Models.Operations;

namespace Gatekeeper.LdapServerLibrary.Engine.Handler
{
    internal interface IRequestHandler<T> where T : IProtocolOp
    {
        internal Task<HandlerReply> Handle(ClientContext context, LdapEvents eventListener, T operation);
    }
}
