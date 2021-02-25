using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;


namespace AutoMoqCore.Tests
{

  public class AutoMoqListTests
  {
    private AutoMoqer mocker;

    public AutoMoqListTests()
    {
      mocker = new AutoMoqer();
    }


    [Fact]
    public void CanRegisterAndResolveAListUsingTheDeclaredType()
    {
      var dependency = new Dependency();
      IList<Dependency> list = new List<Dependency>();
      list.Add(dependency);

      mocker.SetInstance(list);
      var thing = mocker.Create<ThingThatHasDependencies>();

      thing.FindOne().Should().BeSameAs(dependency);
    }

    [Fact]
    public void CanRegisterAndResolveAListWithAnExplicitTypeProvidedToSetInstance()
    {
      var dependency = new Dependency();
      var list = new List<Dependency>();
      list.Add(dependency);

      mocker.SetInstance<IList<Dependency>>(list);
      var thing = mocker.Create<ThingThatHasDependencies>();

      thing.FindOne().Should().BeSameAs(dependency);
    }
  }

  public class Dependency
  {
  }

  public class ThingThatHasDependencies
  {
    private IList<Dependency> innerList;

    public ThingThatHasDependencies(IList<Dependency> dependencies)
    {
      innerList = dependencies;
    }

    public Dependency FindOne()
    {
      if (innerList == null || innerList.Any() == false)
      {
        return null;
      }

      return innerList.FirstOrDefault();
    }
  }
}
