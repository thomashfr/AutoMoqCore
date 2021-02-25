using AutoMoqCore.Helpers;
using FluentAssertions;
using Moq;
using Xunit;


namespace AutoMoqCore.Tests
{

  public class WithAutomoqerTests
  {
    private AutoMoqer mocker;

    [Fact]
    public void Setup()
    {
      mocker = new AutoMoqer();
    }

    [Fact]
    public void Instantiating_the_automoqer_sets_the_static_mocker()
    {
      WithAutomoqer.Mocker = null;
      var test = new WithAutomoqer();
      WithAutomoqer.Mocker.Should().NotBeNull();
    }

    [Fact]
    public void A_config_option_can_be_provided_when_setting_up()
    {
      WithAutomoqer.Mocker = null;
      new WithAutomoqer(new Config { MockBehavior = MockBehavior.Loose });

      WithAutomoqer.Create<Test>().DoSomething(); // should not fail

      new WithAutomoqer(new Config { MockBehavior = MockBehavior.Strict });
      var errorHit = false;
      try
      {
        WithAutomoqer.Create<Test>().DoSomething(); // should fail
      }
      catch
      {
        errorHit = true;
      }
      errorHit.Should().BeTrue();
    }

    [Fact]
    public void GetMock_returns_the_mock()
    {
      var test = new WithAutomoqer();
      WithAutomoqer.Mocker = new AutoMoqer();

      WithAutomoqer.GetMock<IDependency>().Should().BeSameAs(WithAutomoqer.Mocker.GetMock<IDependency>());
    }

    [Fact]
    public void Create_returns_the_class_resolved_from_automoqer()
    {
      var test = new WithAutomoqer();
      WithAutomoqer.Mocker = new AutoMoqer();

      WithAutomoqer.Create<Test>()
          .Dependency.Should().BeSameAs(WithAutomoqer.GetMock<IDependency>().Object);
    }

    [Fact]
    public void SetInstance_sets_the_instance()
    {
      WithAutomoqer.Mocker = new AutoMoqer();

      var instance = new Mock<IDependency>().Object;
      WithAutomoqer.SetInstance(instance);

      WithAutomoqer.Create<Test>().Dependency.Should().BeSameAs(instance);
    }

    public interface IDependency
    {
      void DoSomething();
    }

    public class Test
    {
      public readonly IDependency Dependency;

      public Test(IDependency Dependency)
      {
        this.Dependency = Dependency;
      }

      public void DoSomething()
      {
        Dependency.DoSomething();
      }
    }
  }
}