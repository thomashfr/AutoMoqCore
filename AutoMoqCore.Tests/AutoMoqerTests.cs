using System;
using FluentAssertions;
using Moq;
using Xunit;


namespace AutoMoqCore.Tests
{

  public class AutoMoqerTests
  {
    private AutoMoqer mocker;

    public AutoMoqerTests()
    {
      mocker = new AutoMoqer();
    }


    [Fact]
    public void Can_resolve_a_concrete_class()
    {
      var concreteClass = mocker.Create<ConcreteClass>();
      Assert.NotNull(concreteClass);
    }

    [Fact]
    public void Can_resolve_a_class_with_dependencies()
    {
      var concreteClass = mocker.Create<ClassWithDependencies>();
      Assert.NotNull(concreteClass);
    }

    [Fact]
    public void Can_resolve_a_class_with_func_dependencies()
    {
      var concreteClass = mocker.Create<ClassWithFuncDependencies>();
      Assert.NotNull(concreteClass);
    }

    [Fact]
    public void Can_test_against_a_func_dependency_as_if_it_were_not()
    {
      var concreteClass = mocker.Create<ClassWithFuncDependencies>();
      concreteClass.CallSomething();
      mocker.GetMock<IDependency>()
          .Verify(x => x.Something(), Times.Once());
    }

    [Fact]
    public void Can_test_with_an_abstract_dependency_registered_by_the_create()
    {
      var concreteClass = mocker.Create<ClassWithAbstractDependenciesAndManyConstructors>();
      concreteClass.CallSomething();
      mocker.GetMock<AbstractDependency>()
          .Verify(x => x.Something(), Times.Once());
    }

    [Fact]
    public void Can_test_with_an_abstract_dependency_registered_before_the_create()
    {
      mocker.GetMock<AbstractDependency>(); // here is the register
      var concreteClass = mocker.Create<ClassWithAbstractDependenciesAndManyConstructors>();
      concreteClass.CallSomething();
      mocker.GetMock<AbstractDependency>()
          .Verify(x => x.Something(), Times.Once());
    }

    [Fact]
    public void Can_test_with_a_class_that_has_many_constructors_and_abstract_dependencies()
    {

    }

    [Fact]
    public void Can_resolve_a_class_with_nongeneric_create()
    {
      var concreteClass = mocker.Create(typeof(ClassWithAbstractDependenciesAndManyConstructors)) as ClassWithAbstractDependenciesAndManyConstructors;

      Assert.NotNull(concreteClass);
    }

    [Fact]
    public void Can_resolve_a_interface()
    {
      var errorWasHit = false;
      try
      {

        var mockedInterface = mocker.Create<IDependency>();
        mockedInterface.Should().BeEquivalentTo(typeof(IDependency));

      }
      catch
      {
        errorWasHit = true;
      }

      errorWasHit.Should().BeTrue();
    }

    [Fact]
    public void Can_verifyall()
    {
      var mocked = mocker.Create<ClassWithMultipleDependencies>();
      mocker.GetMock<IDependency>().Setup(m => m.Something());
      mocker.GetMock<IOtherDependency>().Setup(m => m.SomethingElse());
      mocked.ConditionallyUseDepedencies(true, true);
      mocker.VerifyAll();
    }

    [Fact]
    public void Can_verifyall_not_called()
    {
      var mocked = mocker.Create<ClassWithMultipleDependencies>();
      mocker.GetMock<IDependency>().Setup(m => m.Something());
      mocker.GetMock<IOtherDependency>().Setup(m => m.SomethingElse());
      mocked.ConditionallyUseDepedencies(true, false);
      Assert.Throws<MockException>(() => mocker.VerifyAll());
    }

    [Fact]
    public void Can_verify_all()
    {
      var mocked = mocker.Create<ClassWithMultipleDependencies>();
      mocker.GetMock<IDependency>().Setup(m => m.Something()).Verifiable();
      mocker.GetMock<IOtherDependency>().Setup(m => m.SomethingElse()).Verifiable();
      mocked.ConditionallyUseDepedencies(true, true);
      mocker.Verify();
    }

    [Fact]
    public void Can_verify_all_not_called()
    {
      var mocked = mocker.Create<ClassWithMultipleDependencies>();
      mocker.GetMock<IDependency>().Setup(m => m.Something()).Verifiable();
      mocker.GetMock<IOtherDependency>().Setup(m => m.SomethingElse()).Verifiable();
      mocked.ConditionallyUseDepedencies(false, true);
      Assert.Throws<MockException>(() => mocker.Verify());
    }
  }

  public class ConcreteClass
  {
  }
  //default behaviour is strict ? if they do not all come from the factory !
  // perhaps instead GetAllMocks - could then call verify on them
  public class ClassWithMultipleDependencies
  {
    private readonly IDependency dependency;
    private readonly IOtherDependency otherDependency;

    public ClassWithMultipleDependencies(IDependency dependency, IOtherDependency otherDependency)
    {
      this.dependency = dependency;
      this.otherDependency = otherDependency;
    }

    public void ConditionallyUseDepedencies(bool useDependency, bool useOtherDependency)
    {
      if (useDependency)
      {
        dependency.Something();
      }
      if (useOtherDependency)
      {
        otherDependency.SomethingElse();
      }
    }
  }

  public class ClassWithDependencies
  {
    public IDependency Dependency { get; set; }

    public ClassWithDependencies(IDependency dependency)
    {
      Dependency = dependency;
    }
  }

  public class ClassWithFuncDependencies
  {
    private readonly IDependency dependency;

    public ClassWithFuncDependencies(Func<IDependency> dependency)
    {
      this.dependency = dependency();
    }

    public IDependency GetTheDependency { get { return this.dependency; } }

    public void CallSomething()
    {
      dependency.Something();
    }
  }

  public interface IDependency
  {
    void Something();
  }

  public interface IOtherDependency
  {
    void SomethingElse();
  }

  public class ClassWithAbstractDependencies
  {
    private readonly AbstractDependency abstractDependency;

    public ClassWithAbstractDependencies(AbstractDependency abstractDependency)
    {
      this.abstractDependency = abstractDependency;
    }

    public void CallSomething()
    {
      abstractDependency.Something();
    }
  }

  public class ClassWithAbstractDependenciesAndManyConstructors
  {
    private readonly AbstractDependency abstractDependency;

    public ClassWithAbstractDependenciesAndManyConstructors()
    {
    }

    public ClassWithAbstractDependenciesAndManyConstructors(AbstractDependency abstractDependency)
    {
      this.abstractDependency = abstractDependency;
    }

    public ClassWithAbstractDependenciesAndManyConstructors(IDependency dependency, AbstractDependency abstractDependency)
    {
      this.abstractDependency = abstractDependency;
    }

    public void CallSomething()
    {
      abstractDependency.Something();
    }
  }

  public abstract class AbstractDependency
  {
    public virtual void Something()
    {
    }
  }

}