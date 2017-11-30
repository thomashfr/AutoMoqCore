using FluentAssertions;
using Moq;
using Unity;
using Xunit;


namespace AutoMoqCore.Tests
{

    public class ConstructorTests
    {
        [Fact]
        public void I_can_instantiate_a_working_automoqer_with_no_dependencies()
        {
            var mocker = new AutoMoqer();

            var bar = mocker.Create<Bar>();
            bar.Foo.Should().BeSameAs(mocker.GetMock<IFoo>().Object);
        }

        [Fact]
        public void I_can_replace_the_unity_container_with_my_own()
        {
            var container = new UnityContainer();

            var foo = new Mock<IFoo>().Object;
            container.RegisterInstance(foo);

            var mocker = new AutoMoqer(container);

            var bar = mocker.Create<Bar>();
            bar.Foo.Should().BeSameAs(foo);
        }

        [Fact]
        public void I_can_replace_the_unity_container_with_my_own_through_config()
        {
            var container = new UnityContainer();
            var config = new Config() {Container = container};

            var foo = new Mock<IFoo>().Object;
            container.RegisterInstance(foo);

            var mocker = new AutoMoqer(config);

            var bar = mocker.Create<Bar>();
            bar.Foo.Should().BeSameAs(foo);
        }

        public interface IFoo
        {
        }

        public class Bar
        {
            public IFoo Foo { get; set; }

            public Bar(IFoo foo)
            {
                Foo = foo;
            }
        }
    }
}
