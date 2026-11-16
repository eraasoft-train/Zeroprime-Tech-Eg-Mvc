using Microsoft.Extensions.Logging;
using Zeroprime.Application.Abstractions;
using Zeroprime.Application.Features.Contact;
using Zeroprime.Domain.Entities;

namespace Zeroprime.UnitTests.Application;

sealed class StubContactInquiryRepository : IContactInquiryRepository
{
    public List<ContactInquiry> Added { get; } = [];

    public Task AddAsync(ContactInquiry inquiry, CancellationToken ct = default)
    {
        Added.Add(inquiry);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ContactInquiry>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<ContactInquiry>>(Added);
}

sealed class NullLogger<T> : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => false;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
}

public class ContactInquiryServiceTests
{
    private static CreateContactInquiryRequest ValidRequest() => new()
    {
        Name = "Jane Doe",
        Email = "jane@example.com",
        Company = "MallCo",
        Subject = "CanBank deployment",
        Message = "We would like CanBank machines in our shopping mall."
    };

    private static ContactInquiryService CreateService(StubContactInquiryRepository repository) =>
        new(repository,
            new Zeroprime.Application.Features.Contact.Validators.CreateContactInquiryRequestValidator(),
            new NullLogger<ContactInquiryService>());

    [Fact]
    public async Task SubmitAsync_WithValidRequest_StoresInquiryAndSucceeds()
    {
        var repository = new StubContactInquiryRepository();
        var service = CreateService(repository);

        var result = await service.SubmitAsync(ValidRequest());

        Assert.True(result.Succeeded);
        Assert.Single(repository.Added);
        Assert.Equal("Jane Doe", repository.Added[0].Name);
        Assert.Equal(Zeroprime.Domain.Enums.InquiryStatus.New, repository.Added[0].Status);
    }

    [Fact]
    public async Task SubmitAsync_WithInvalidRequest_ReturnsErrorsAndDoesNotStore()
    {
        var repository = new StubContactInquiryRepository();
        var service = CreateService(repository);
        var request = ValidRequest();
        request.Name = "";

        var result = await service.SubmitAsync(request);

        Assert.False(result.Succeeded);
        Assert.Contains("Name", result.Errors.Keys);
        Assert.Empty(repository.Added);
    }
}
