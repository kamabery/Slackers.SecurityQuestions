using Microsoft.Extensions.Logging;
using Slackers.Repository;

namespace Slackers.SecurityQuestions.ConsoleScreen.State;

public class StateService : IStateService<SecurityScreenState>{
    private readonly SecurityScreenState _state;

    public StateService(IRepository repository, ILogger<SecurityQuestionScreen> logger)
    {
        _state = new SecurityScreenState(repository, logger);

    }

    public SecurityScreenState GetState()
    {
        return _state;
    }
}