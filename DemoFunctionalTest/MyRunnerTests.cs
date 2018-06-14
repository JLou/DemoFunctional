using CSharpFunctionalExtensions;
using DemoFunctional;
using Moq;
using System;
using Xunit;

namespace DemoFunctionalTest
{
    public class MyRunnerTests
    {
        private MyRunner _myRunner;
        private Mock<IService> _serviceMoq;

        public MyRunnerTests()
        {
            _serviceMoq = new Mock<IService>();
            _myRunner = new MyRunner(_serviceMoq.Object);

        }

        [Fact]
        public void ShouldFailIfUserNotFound()
        {
            //Given
            _serviceMoq.Setup(s => s.GetCurrentUser())
                .Returns(() => null);

            //When
            var result = _myRunner.Run();

            //Then
            Assert.True(result.IsFailure);
            Assert.Equal("anonymous_user", result.Error);
        }

        [Fact]
        public void ShouldFailIfCantRetrieveAuthorizations()
        {
            //Given
            var user = "georges";
            _serviceMoq.Setup(s => s.GetCurrentUser())
                .Returns(() => user);
            _serviceMoq.Setup(s => s.IsAllowed(user))
                .Returns(Result.Fail<bool>("ad_unavailable"));

            //When
            var result = _myRunner.Run();

            //Then
            Assert.True(result.IsFailure);
            Assert.Equal("ad_unavailable", result.Error);
        }

        [Fact]
        public void ShouldFailIfNotAuthorized()
        {
            //Given
            var user = "georges";
            _serviceMoq.Setup(s => s.GetCurrentUser())
                .Returns(() => user);
            _serviceMoq.Setup(s => s.IsAllowed(user))
                .Returns(Result.Ok(false));

            //When
            var result = _myRunner.Run();

            //Then
            Assert.True(result.IsFailure);
            Assert.Equal("not_allowed", result.Error);
        }

        [Fact]
        public void ShouldFailIfCouldntDoStuff()
        {
            //Given
            var user = "georges";
            _serviceMoq.Setup(s => s.GetCurrentUser())
                .Returns(() => user);
            _serviceMoq.Setup(s => s.IsAllowed(user))
                .Returns(Result.Ok(true));
            _serviceMoq.Setup(s => s.DoStuff())
                .Returns(Result.Fail("file_unavailable"));

            //When
            var result = _myRunner.Run();

            //Then
            Assert.True(result.IsFailure);
            Assert.Equal("file_unavailable", result.Error);
        }

        [Fact]
        public void ShouldSucceedIfAllGood()
        {
            //Given
            var user = "georges";
            _serviceMoq.Setup(s => s.GetCurrentUser())
                .Returns(() => user);
            _serviceMoq.Setup(s => s.IsAllowed(user))
                .Returns(Result.Ok(true));
            _serviceMoq.Setup(s => s.DoStuff())
                .Returns(Result.Ok());

            //When
            var result = _myRunner.Run();

            //Then
            Assert.True(result.IsSuccess);
        }
    }
}
