using Gatekeeper.LdapServerLibrary.PacketParser.Models.Operations.Request;

namespace Gatekeeper.LdapServerLibrary.Session.Events
{
    public interface ISearchEvent
    {
        SearchRequest SearchRequest { get; set; }
    }
}
