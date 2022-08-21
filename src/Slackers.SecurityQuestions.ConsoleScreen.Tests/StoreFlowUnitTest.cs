using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using Slackers.HostedConsole;
using Slackers.HostedConsole.TestHelper;
using Slackers.SecurityQuestions.ConsoleScreen.Flows;
using Slackers.SecurityQuestions.ConsoleScreen.State;

namespace Slackers.SecurityQuestions.ConsoleScreen.Tests;

[TestClass]
public class StoreFlowUnitTest
{
    [TestMethod]
    public void IfUserSaysYesThanNextFlowIsSelectQuestionsFlow()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var mockIoService = new MockFlowIoService();
        var stateService = fixture.Freeze<IStateService<SecurityScreenState>>();
        stateService.GetState().Returns(fixture.Create<SecurityScreenState>());
        fixture.Register<IFlowIoService>(() => mockIoService);
        var sut = fixture.Create<StoreFlow>();
        mockIoService.UserInputMenuSelection.Push(0); // yes

        // Act
        sut.Run();

        // Assert
        Assert.AreEqual(SecurityQuestionFlows.SelectQuestionFlow, sut.NextFlow);
    }

    [TestMethod]
    public void IfUserSaysNoThanNextFlowIsMainFlow()
    {
        // Arrange
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var mockIoService = new MockFlowIoService();
        var stateService = fixture.Freeze<IStateService<SecurityScreenState>>();
        stateService.GetState().Returns(fixture.Create<SecurityScreenState>());
        fixture.Register<IFlowIoService>(() => mockIoService);
        var sut = fixture.Create<StoreFlow>();
        mockIoService.UserInputMenuSelection.Push(1); // no

        // Act
        sut.Run();

        // Assert
        Assert.AreEqual(SecurityQuestionFlows.MainFlow, sut.NextFlow);
    }

}