
namespace Projeto1IA
{
    public interface IWorkable
    {
        void Work(string task);
        void Idle();
    }

    public interface IRestable
    {
        void Sleep();
        void Restock();
    }

    public interface IRechargeable
    {
        void Recharge();
    }

    public interface IEmergencyResponder
    {
        void RespondToIncident();
        void Evacuate();
    }
}
