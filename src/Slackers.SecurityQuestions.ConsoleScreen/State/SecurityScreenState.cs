using Microsoft.Extensions.Logging;
using Slackers.Logging;
using Slackers.Repository;
using Slackers.SecurityQuestions.ConsoleScreen.Models;

namespace Slackers.SecurityQuestions.ConsoleScreen.State
{
    public class FlowExceution
    {
        public FlowExceution(string flowName, DateTime flowCreated)
        {
            FlowName = flowName;
            FlowCreated = flowCreated;
        }

        public string FlowName { get;  }
        public DateTime FlowCreated { get;  }
    }
    public class SecurityScreenState
    {
        private readonly IRepository _repository;
        private readonly ILogger<SecurityQuestionScreen> _logger;

        public SecurityScreenState(IRepository repository, ILogger<SecurityQuestionScreen> logger)
        {
            _repository = repository;
            _logger = logger;
            FlowExecutions = new List<FlowExceution>();
        }

        public User? User { get; set; }

        public string CurrentQuestion { get; set; } = default!;

        public int AnswerCount;

        public bool AddingQuestions = false;

        public List<SecurityQuestion> SecurityQuestions { get; set; } = new();

        public List<FlowExceution> FlowExecutions { get; }

        public void ResetState()
        {
            User = null;
            AnswerCount = 0;
            CurrentQuestion = string.Empty;
        }

        public void FlowExecuted(string flowName)
        {
            var execution = new FlowExceution(flowName, DateTime.Now);
            FlowExecutions.Add(execution);
        }

        public RepositoryResponse LoadUserFromRepository(string displayName)
        {
            var name = displayName.Replace(" ", "").ToUpper();
            var response = _repository.Get<User>(u => u.Name == name);
            if (response.Response == RepositoryResponse.Ok)
            {
                if (response.Results == null || response.Results.Count() != 1)
                {
                    _logger.LogEventError(new InvalidOperationException("Invalid repository response multiple users with the same name"), GetLoggingContext());
                    return RepositoryResponse.Error;
                }

                User = response.Results.Single();
            }

            if (response.Response == RepositoryResponse.NotFound)
            {
                User = new User { DisplayName = displayName, Name = name, Id = Guid.NewGuid() };
            }

            return response.Response;
        }

        public RepositoryResponse SaveUserToRepository()
        {
            if (User == null)
            {
                _logger.LogEventError(new InvalidOperationException("No User State invalid"), GetLoggingContext());
                return RepositoryResponse.Error;
            }

            var result = _repository.Post(User);
            return result.Response;
        }

        public EventContext GetLoggingContext()
        {
            // This isn't a true event source but you wouldn't want to log all state.
            return new EventContext
            {
                AddingQuestions = AddingQuestions,
                Name = User?.Name ?? "",
                QuestionsAdded = AddingQuestions ? AnswerCount : 0,
                QuestionsAnswersAttempted = AddingQuestions == false ? AnswerCount : 0
            };
        }
    }
}
