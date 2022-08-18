using ConsoleTools;
using Slackers.Repository;
using Slackers.SecurityQuestions.ConsoleScreen.Models;
using Slackers.SecurityQuestions.Security;

namespace Slackers.SecurityQuestions.ConsoleScreen;

/// <summary>
/// A Console Screen to gather security questions as well as answer them
/// </summary>
public class SecurityQuestionScreen : IConsoleScreen
{
    private readonly IRepository _repository;

    private List<SecurityQuestion> _questions = new();
    
    public SecurityQuestionScreen(IRepository repository)
    {
        _repository = repository;
    }

    public void Show()
    {
        if (!SecurityQuestionListLoaded()) return;
        MainFlow();
    }

    private bool SecurityQuestionListLoaded()
    {
        var query = _repository.Get<SecurityQuestion>();
        if (query.Response == RepositoryResponse.Error)
        {
            Console.WriteLine(query.Message);
            return false;
        }


        if (query.Response == RepositoryResponse.NotFound)
        {
            Initialize();
        }
        else
        {
            if (query.Results != null && query.Results.Any())
            {
                _questions.AddRange(query.Results.ToList());
            }
        }

        if (!_questions.Any())
        {
            Console.WriteLine("A System error has occurred, please close application");
            return false;
        }

        return true;
    }

    private void MainFlow()
    {
        //PO Note: there is no Exit described in the flow, but adding it for UX, I can take it out
        Console.WriteLine("Hi, what is your name?, or type quit to exit");
        string? displayName = string.Empty;
        while (string.IsNullOrEmpty(displayName))
        {
            displayName = Console.ReadLine();

            if (string.IsNullOrEmpty(displayName))
            {
                Console.WriteLine("Please provide a name or type quit");
            }
        }

        if (displayName.ToUpper() == "QUIT")
        {
            return;
        }


        var name = displayName.Replace(" ", "").ToUpper();
        var response = _repository.Get<User>(u => u.Name == name);
        if (response.Response == RepositoryResponse.Error)
        {
            Console.WriteLine(response.Message);
        }

        if (response.Response == RepositoryResponse.NotFound)
        {
            var user = new User { Id = Guid.NewGuid(), Name = name, DisplayName = name };
            StoreFlow(user);
        }

        if (response.Response == RepositoryResponse.Ok)
        {
            var user = response.Results?.FirstOrDefault();
            if (user == null)
            {
                Console.WriteLine("System Failure, please close and try again later");
                return;
            }

            AnswerFlow(user);
        }

    }

    private void StoreFlow(User user)
    {
        var menu = MenuManager.YesNo(
            () => SelectQuestionFlow(user), 
            MainFlow, 
            "Would you like to store answers to security questions?");
        
        menu.Show();
    }

    private void SelectQuestionFlow(User user,  int answerCount = 0)
    {
        int leftToGo = 3 - answerCount;
        var menuQuestions = _questions.Select(s => s.Question).ToList();
        var menu = MenuManager.AnswerQuestion(
            menuQuestions,
            (menu, question) => SelectAnswerFlow(question, user, menu, answerCount),
            $"Please select a question. {leftToGo} left to answer");

        menu.Show();
    }

    private void SelectAnswerFlow(string question, User user, ConsoleMenu menu, int answerCount)
    {
        menu.CloseMenu();
        Console.WriteLine("Please Answer");
        var answer = Console.ReadLine();
        if (string.IsNullOrEmpty(answer))
        {
            SelectAnswerFlow(question, user, menu, answerCount);
            return;
        }

        var hashAnswer = answer.Hash();

        user.Answers.Add(question, hashAnswer);
        answerCount++;
        if (answerCount == 3)
        {
            var result = _repository.Post(user);
            if (result.Response == RepositoryResponse.Created)
            {
                Console.Clear();
                MainFlow();
                return;
            }
        }

        Console.Clear();
        SelectQuestionFlow(user, answerCount);
    }

    private void AnswerFlow(User user, int tryCount = 0)
    {
        //PO Note: the specifications were unclear if the user needed to select only from the 
        // questions they answered or from all potential questions.  I went with only the ones they answered,
        // but could display all questions
        var attemptsLeft = 3- tryCount;
        var questions = user.Answers.Keys.ToList();
        var menu = MenuManager.AnswerQuestion(
            questions, 
            (menu, question) => AnswerAttemptFlow(menu, question, user, tryCount),
            $"Please Select a question to answer. you have {attemptsLeft} attempts left");
        menu.Show();
    }

    private void AnswerAttemptFlow(ConsoleMenu menu, string question, User user, int tryCount)
    {
        menu.CloseMenu();
        Console.Clear();
        Console.WriteLine(question);
        var answer = Console.ReadLine();

        if (string.IsNullOrEmpty(answer))
        {
            AnswerAttemptFlow(menu, question, user, tryCount);
            return;
        }

        // Correctly answered question
        if (answer.VerifyHash(user.Answers[question]))
        {
            Console.WriteLine("Congratulation! You answered Correctly!");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.Clear();
            MainFlow();
            return;
        }
   
        // Failed
        tryCount++;
        if (tryCount == 3)
        {
            // Failed too often
            Console.WriteLine("You have exceeded the number of attempts");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.Clear();
            MainFlow();
            return;
        }

        // Failed, but can retry
        Console.WriteLine("Incorrect, please try again");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        Console.Clear();

        AnswerFlow(user, tryCount);
    }

    private void Initialize()
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
                    _questions.Add(question);
                }
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            throw;
        }
    }
}