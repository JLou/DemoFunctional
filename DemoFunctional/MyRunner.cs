using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoFunctional
{
    public class MyRunner
    {
        private IService _service;

        public MyRunner(IService service)
        {
            _service = service;
        }

        public Result Run()
        {
            var user = _service.GetCurrentUser();
            return user.ToResult("anonymous_user")
                .OnSuccess(u => _service.IsAllowed(u))
                .Ensure(isAllowed => isAllowed, "not_allowed")
                .OnSuccess(isAllowed => _service.DoStuff());
        }

        public Result Run2()
        {
            var user = _service.GetCurrentUser();
            if (user.HasNoValue)
            {
                return Result.Fail("anonymous_user");
            }

            var auth = _service.IsAllowed(user.Value);
            if (auth.IsFailure)
            {
                return auth;
            }

            if (!auth.Value)
            {
                return Result.Fail("not_allowed");
            }

            var actionResult = _service.DoStuff();
            if (actionResult.IsFailure)
            {
                return Result.Fail(actionResult.Error);
            }

            return Result.Ok();
        }
    }
}
