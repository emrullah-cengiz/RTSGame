using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class CommandableActorBase : TransformObject, ICommandableActor
{
    private Queue<ICommand> _activeCommands = new();

    public void SetCommand(ICommand command)
    {
        // _activeCommands.Enqueue(command);

        command.ExecuteAsync();

        //execute vs..
    }
}