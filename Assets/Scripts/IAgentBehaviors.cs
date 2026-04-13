using Active.Core;

namespace Projeto1IA
{
    public interface IWorkable
    {
        status Work(string task);
        status Idle();
    }

    public interface IRestable
    {
        status Sleep();
        status Restock();
    }

    public interface IRechargeable
    {
        status Recharge();
    }

    public interface IEmergencyResponder
    {
        status RespondToIncident();
        status Evacuate();
    }
}
