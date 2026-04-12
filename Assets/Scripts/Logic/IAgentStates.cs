using UnityEngine;
using Active.Core;
using static Active.Raw;

namespace Projeto1IA
{
    public interface IAgentStates
    {
        status Sleep();
        status Working(string task);
        status Fix(string task);
    }
}