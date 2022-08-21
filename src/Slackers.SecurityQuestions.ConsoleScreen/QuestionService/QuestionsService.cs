using Microsoft.Extensions.Logging;
using Slackers.HostedConsole;
using Slackers.Logging;
using Slackers.Repository;
using Slackers.SecurityQuestions.ConsoleScreen.Models;
using Slackers.SecurityQuestions.ConsoleScreen.State;

namespace Slackers.SecurityQuestions.ConsoleScreen.QuestionService
{
    public class QuestionsService : IQuestionsService
    {
        private readonly IRepository _repository;
        private readonly ILogger<QuestionsService> _logger;
        private readonly IFlowIoService _flowIoService;
        private readonly SecurityScreenState _state;

        public QuestionsService(
            IStateService<SecurityScreenState> stateService, 
            IRepository repository, 
            ILogger<QuestionsService> logger, 
            IFlowIoService flowIoService)
        {
            _repository = repository;
            _logger = logger;
            _flowIoService = flowIoService;
            _state = stateService.GetState();
        }
        public bool LoadSecurityQuestions()
        { 
            var query = _repository.Get<SecurityQuestion>();
           if (query.Response == RepositoryResponse.Error)
           { 
               _logger.LogError(query.Exception, query.Message);
               _flowIoService.WriteLine(query.Message);
               return false;
           }

           if (query.Response == RepositoryResponse.NotFound)
           {
               LoadSecurityQuestionsFromFile();
               _logger.LogInformation("List Initialized");
               return true;
           }
   
           if (query.Results != null && query.Results.Any())
           {
               _state.SecurityQuestions.AddRange(query.Results.ToList());
               _logger.LogInformation("List Loaded from repository");
               return true;
           }

           // Something went sideways
           _flowIoService.WriteLine("A System error has occurred, please close application");
           return false;
        }

        private void LoadSecurityQuestionsFromFile()
        {
            try
            {
                // First Time Run
                using (StreamReader sr = new StreamReader("Questions.txt"))
                {
                    while (sr.ReadLine() is { } line)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }
                        var question = new SecurityQuestion()
                        {
                            Id = Guid.NewGuid(),
                            Question = line
                        };
                        _repository.Post(question);
                        _state.SecurityQuestions.Add(question);
                    }
                }
            }
            catch (Exception e)
            {
                _flowIoService.WriteLine("An error has occurred");
                _logger.LogEventError(e, _state.GetLoggingContext());
            }
        }
    }
}
