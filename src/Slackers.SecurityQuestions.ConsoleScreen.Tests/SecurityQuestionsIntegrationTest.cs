using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Slackers.HostedConsole;
using Slackers.HostedConsole.TestHelper;
using Slackers.Repository;
using Slackers.SecurityQuestions.ConsoleScreen.Flows;
using Slackers.SecurityQuestions.ConsoleScreen.Models;
using Slackers.SecurityQuestions.ConsoleScreen.QuestionService;
using Slackers.SecurityQuestions.ConsoleScreen.State;
using System.Linq.Expressions;
using Slackers.SecurityQuestions.Security;

namespace Slackers.SecurityQuestions.ConsoleScreen.Tests;

/// <summary>
/// WARNING: Integration tests are by there very nature brittle.
/// </summary>
[TestClass]
public class SecurityQuestionsIntegrationTests
{
    // https://blog.ploeh.dk/2010/08/19/AutoFixtureasanauto-mockingcontainer/
    private IFixture? _fixture;
    private IRepository? _repository;
    private MockFlowIoService _flowIoService;
    private IEnumerable<IFlow<SecurityScreenState>> _flows;
    private SecurityScreenState _state;

    [TestInitialize]
    public void Setup()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _repository = _fixture.Freeze<IRepository>();
        var logger = _fixture.Create<ILogger<SecurityQuestionScreen>>();
        var stateService = _fixture.Freeze<IStateService<SecurityScreenState>>();
        _state = new SecurityScreenState(_repository, logger)
        {
            SecurityQuestions = _fixture.Create<List<SecurityQuestion>>()
        };
        stateService.GetState().Returns(_state);
   
        
        var questionService = _fixture.Freeze<IQuestionsService>();
        questionService.LoadSecurityQuestions().Returns(true);
        _flowIoService = new MockFlowIoService();
        _fixture.Register<IFlowIoService>(()=> _flowIoService);
    }

    [TestMethod]
    public void CanAddUser()
    {
        // Arrange
     
        _flowIoService.UserInputMenuSelection.Push( 0); // Question
        _flowIoService.UserInputMenuSelection.Push(0); // Question
        _flowIoService.UserInputMenuSelection.Push(0); // Question
        _flowIoService.UserInputMenuSelection.Push(0); // Yes

        _flowIoService.UserInput.Push(SecurityQuestionFlows.Quit);
        _flowIoService.UserInput.Push(_fixture.Create<string>()); // Answer
        _flowIoService.UserInput.Push(_fixture.Create<string>()); // Answer
        _flowIoService.UserInput.Push(_fixture.Create<string>()); // Answer
        _flowIoService.UserInput.Push(_fixture.Create<string>()); // Name


        //Happy path for the repository
        _repository.Get(Arg.Any<Expression<Func<User, bool>>>()).Returns(new RepositoryMessage<User> { Response = RepositoryResponse.NotFound });
        _repository.Post<User>(Arg.Any<User>()).Returns(new RepositoryMessage<User>
            { Response = RepositoryResponse.Created });
        var sut = _fixture.Create<SecurityQuestionScreen>();

        // Act
        sut.Show();

        // Assert
        Assert.AreEqual(2,_state.FlowExecutions.Where(e => e.FlowName == SecurityQuestionFlows.MainFlow).Count());
        Assert.AreEqual(3, _state.FlowExecutions.Where(e => e.FlowName == SecurityQuestionFlows.SelectQuestionFlow).Count());
    }

    [TestMethod]
    public void CanAnswerQuestions()
    {
        // Arrange
        var user = new User
        {
            Name = "BOB",
            DisplayName = "BOB", 
            Answers = new Dictionary<string, string>
            {
                { "Question1", "AnswerOne".Hash() },
                { "Question2", "AnswerTwo".Hash() },
                { "Question3", "AnswerThree".Hash() },
            }
        };

        _repository.Get(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(new RepositoryMessage<User> { Response = RepositoryResponse.Ok, Results = new List<User>{user}});
        // Menu
        _flowIoService.UserInputMenuSelection.Push(2); // Question 3
        _flowIoService.UserInputMenuSelection.Push(1); // Question 2
        _flowIoService.UserInputMenuSelection.Push(0); // Question 1
  
        //Input
        _flowIoService.UserInput.Push(SecurityQuestionFlows.Quit);
        _flowIoService.UserInput.Push("Question3");
        _flowIoService.UserInput.Push("Question2");
        _flowIoService.UserInput.Push("Question1");
        _flowIoService.UserInput.Push("BOB");

        // Act
        var sut = _fixture.Create<SecurityQuestionScreen>();

        // Act
        sut.Show();

        // Assert
        Assert.IsTrue(_state.FlowExecutions.Where(f=> f.FlowName == SecurityQuestionFlows.MainFlow).Count() > 1);

    }


}