using Agents;
using Items;

namespace AgentBody
{
    public interface ITool : IItem { }

    public interface ICarrier : IAgent
    {
        Carrier Carrier { get; }
    }
}
