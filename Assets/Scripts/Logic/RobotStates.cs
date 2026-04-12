using UnityEngine;
using Active.Core;
using static Active.Raw;

namespace Projeto1IA
{
    public class RobotStates : AgentStateMachine
{
    public override status Sleep() => new status();
    public override status Working(string task) => new status();
    public override status Fix(string task) => new status();
}
}