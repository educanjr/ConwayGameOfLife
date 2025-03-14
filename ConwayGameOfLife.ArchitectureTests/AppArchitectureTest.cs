namespace ConwayGameOfLife.ArchitectureTests;

using FluentAssertions;
using NetArchTest.Rules;

public class AppArchitectureTest
{
    private const string ApplicationNamespace = nameof(ConwayGameOfLife.Application);
    private const string DataNamespace = nameof(ConwayGameOfLife.Data);
    private const string WebNamespace = nameof(ConwayGameOfLife.Web);
    private const string AppNamespace = nameof(ConwayGameOfLife.App);

    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            ApplicationNamespace,
            WebNamespace,
            AppNamespace
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Data_Should_Not_HaveDependencyOnProjects()
    {
        // Arrange
        var assembly = typeof(Data.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            WebNamespace,
            AppNamespace
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Web_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Web.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            DataNamespace,
            AppNamespace
        };

        // Act
        var testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(otherProjects)
            .GetResult();

        // Assert
        testResult.IsSuccessful.Should().BeTrue();
    }
}