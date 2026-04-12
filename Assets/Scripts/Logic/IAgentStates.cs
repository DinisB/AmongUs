using UnityEngine;
using Active.Core;
using static Active.Raw;

namespace Projeto1IA
{
    public interface IAgentStates
    {
        status Idle();
        status Work(string task);
        status Sleep();
        status Restock();
        status Recharge();
        status RespondToIncident();
        status Evacuate();
    }
}