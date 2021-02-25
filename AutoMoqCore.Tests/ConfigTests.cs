using FluentAssertions;
using Moq;
using Xunit;

namespace AutoMoqCore.Tests
{
  public class ConfigTests
  {

    public class MockBehaviorTests
    {
      [Fact]
      public void ItShouldDefaultToLoose()
      {
        var config = new Config();
        config.MockBehavior.Should().BeEquivalentTo(MockBehavior.Loose);
      }
    }
  }
}