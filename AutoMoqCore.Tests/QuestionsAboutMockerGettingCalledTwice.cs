
using FluentAssertions;
using Xunit;

namespace AutoMoqCore.Tests
{

    public class QuestionsAboutMockerGettingCalledTwice
    {
        [Fact]
        public void Can_a_mock_generated_by_automoq_be_called_twice()
        {
            var mocker = new AutoMoqer();

            var id = "the id";
            var profile = new Profile();
            var profiles = mocker.Create<SomeController>();

            mocker.GetMock<IProfilerGetter>().Setup(p => p.Get(id)).Returns(profile);
            var p1 = profiles.Get(id);
            var p2 = profiles.Get(id);
            p1.Should().NotBeNull();
            p2.Should().NotBeNull();
        }

        public class SomeController
        {
            private readonly IProfilerGetter profilerGetter;

            public SomeController(IProfilerGetter profilerGetter)
            {
                this.profilerGetter = profilerGetter;
            }

            public Profile Get(string id)
            {
                return profilerGetter.Get(id);
            }
        }

        public interface IProfilerGetter
        {
            Profile Get(string id);
        }

        public class Profile
        {
        }
    }
}