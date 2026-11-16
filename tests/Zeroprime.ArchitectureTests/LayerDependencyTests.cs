using NetArchTest.Rules;
using Xunit;

namespace Zeroprime.ArchitectureTests;

public class LayerDependencyTests
{
    private const string DomainNamespace = "Zeroprime.Domain";
    private const string ApplicationNamespace = "Zeroprime.Application";
    private const string InfrastructureNamespace = "Zeroprime.Infrastructure";
    private const string WebNamespace = "Zeroprime.Web";

    [Fact]
    public void Domain_ShouldNotDependOn_ApplicationInfrastructureOrWeb()
    {
        var result = Types.InAssembly(typeof(Zeroprime.Domain.Entities.CompanyProfile).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, WebNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Domain layer must not depend on outer layers. Violations: " +
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Application_ShouldNotDependOn_InfrastructureOrWeb()
    {
        var result = Types.InAssembly(typeof(Zeroprime.Application.Features.Company.ICompanyProfileService).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, WebNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Application layer must not depend on Infrastructure or Web. Violations: " +
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Web()
    {
        var result = Types.InAssembly(typeof(Zeroprime.Infrastructure.Persistence.Context.ZeroprimeDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOn(WebNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Infrastructure layer must not depend on Web. Violations: " +
            string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void Domain_Entities_ShouldHavePrivateParameterlessConstructor()
    {
        // EF Core needs a usable constructor surface; ensure entities never expose public setters on identity keys.
        var result = Types.InAssembly(typeof(Zeroprime.Domain.Entities.Product).Assembly)
            .That()
            .ResideInNamespace(DomainNamespace + ".Entities")
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        Assert.True(result.IsSuccessful,
            "Domain entities must not reference EF Core directly. Violations: " +
            string.Join(", ", result.FailingTypeNames ?? []));
    }
}
