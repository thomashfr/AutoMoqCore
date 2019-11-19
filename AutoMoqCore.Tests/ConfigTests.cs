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
            public void It_should_default_to_loose()
            {
                
                var config = new Config();
                config.MockBehavior.Should().BeEquivalentTo(MockBehavior.Loose);
            }
        }
    }
}