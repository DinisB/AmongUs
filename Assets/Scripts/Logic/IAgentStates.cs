using UnityEngine;
using Active.Core;                
using static Active.Raw;

public abstract class IAgentStates : MonoBehaviour {

    public status state;

    void Update(){
        state = Sleep() || Working("") || Fix("");
        if(!state.running){
            enabled = false;
        }
    }

    protected abstract status Sleep();
    protected abstract status Working(string task);
    private status Fix(string task)
    {
        return new status();
    }
}