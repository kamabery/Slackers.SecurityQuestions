using ConsoleTools;

namespace Slackers.SecurityQuestions.ConsoleScreen;

public static class MenuManager
{
    public static ConsoleMenu YesNo (Action yes, Action no, string prompt)
    {
        var menu = new ConsoleMenu();
        menu.Add("Yes", () =>
        {
            menu.CloseMenu();
            yes();
        });
        menu.Add("No", () =>
        { 
            menu.CloseMenu();
            no();
        }).Configure(config => config.WriteHeaderAction = () => Console.WriteLine(prompt));

        return menu;
    }

    public static ConsoleMenu AnswerQuestion(
        List<string> questions, 
        Action<ConsoleMenu, string> answerAction, 
        string prompt)
    {
        var menu = new ConsoleMenu();
        foreach (var question in questions)
        {
            menu.Add(question, () => answerAction(menu, question));
        }
        menu.Configure(config => config.WriteHeaderAction = () => Console.WriteLine(prompt));
        return menu;
    }
}