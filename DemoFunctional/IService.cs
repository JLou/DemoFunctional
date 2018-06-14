using CSharpFunctionalExtensions;

namespace DemoFunctional
{
    public interface IService
    {
        Result<bool> IsAllowed(string user);

        Maybe<string> GetCurrentUser();

        Result DoStuff();
    }
}