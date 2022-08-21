using Microsoft.Extensions.Logging;
using Slackers.HostedConsole;
using Slackers.Logging;
using Slackers.SecurityQuestions.ConsoleScreen.State;
using Slackers.SecurityQuestions.Security;

namespace Slackers.SecurityQuestions.ConsoleScreen.Flows;

public class AnswerAttemptFlow : IFlow<SecurityScreenState>
    {
        private readonly IFlowIoService _flowIoService;
        private readonly ILogger<SecurityQuestionScreen> _logger;
        private readonly SecurityScreenState _state;

        
        public AnswerAttemptFlow(IStateService<SecurityScreenState> stateService, IFlowIoService flowIoService, ILogger<SecurityQuestionScreen> logger)
        {
            _flowIoService = flowIoService;
            _logger = logger;
            _state = stateService.GetState();
            NextFlow = string.Empty;
    }

        public string FlowName => SecurityQuestionFlows.AnswerAttemptFlow.ToString();

        public string NextFlow { get; set; }
        public void Run()
        {
            _flowIoService.Clear();
            _flowIoService.WriteLine(_state.CurrentQuestion);
            var answer = _flowIoService.ReadLine();

            if (string.IsNullOrEmpty(answer))
            {
                // AnswerAttemptFlow(menu, question, user, tryCount);
                return;
            }

            // Correctly answered question
            string? storedAnswer = _state.User?.Answers[_state.CurrentQuestion];
            if (storedAnswer == null)
            {
                var exception = new InvalidOperationException("Invalid stateService");
                _logger.LogEventError(exception, _state.GetLoggingContext());
                return;
            }

            if (answer.VerifyHash(storedAnswer))
            {
                _flowIoService.WriteLine("Congratulation! You answered Correctly!");
                _flowIoService.WriteLine("Press any key to continue");
                _flowIoService.ReadKey();
                _flowIoService.Clear();
                NextFlow = SecurityQuestionFlows.MainFlow.ToString();
                return;
            }

            // Failed
            _state.AnswerCount++;

            if (_state.AnswerCount == 3)
            {
                _state.ResetState();

                // Failed too often
                _flowIoService.WriteLine("You have exceeded the number of attempts");
                _flowIoService.WriteLine("Press any key to continue");
                _flowIoService.ReadKey();
                _flowIoService.Clear();
                NextFlow = SecurityQuestionFlows.MainFlow.ToString();
                return;
            }

            // Failed, but can retry
            _flowIoService.WriteLine("Incorrect, please try again");
            _flowIoService.WriteLine("Press any key to continue");
            _flowIoService.ReadKey();
            _flowIoService.Clear();

            NextFlow = SecurityQuestionFlows.AnswerFlow.ToString();;

        }
    }