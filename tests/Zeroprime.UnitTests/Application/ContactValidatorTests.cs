using Zeroprime.Application.Features.Contact;
using Zeroprime.Application.Features.Contact.Validators;

namespace Zeroprime.UnitTests.Application;

public class ContactValidatorTests
{
    private readonly CreateContactInquiryRequestValidator _validator = new();

    private static CreateContactInquiryRequest ValidRequest() => new()
    {
        Name = "Jane Doe",
        Email = "jane@example.com",
        Subject = "CanBank deployment",
        Message = "We would like CanBank machines in our shopping mall."
    };

    [Fact]
    public void ValidRequest_Passes()
    {
        var result = _validator.Validate(ValidRequest());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void MissingName_Fails()
    {
        var request = ValidRequest();
        request.Name = "";

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void InvalidEmail_Fails()
    {
        var request = ValidRequest();
        request.Email = "not-an-email";

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Email");
    }

    [Fact]
    public void ShortMessage_Fails()
    {
        var request = ValidRequest();
        request.Message = "short";

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Message");
    }

    [Fact]
    public void TooLongMessage_Fails()
    {
        var request = ValidRequest();
        request.Message = new string('x', 4001);

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void PhoneWithLetters_Fails()
    {
        var request = ValidRequest();
        request.Phone = "call-me-now";

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Phone");
    }

    [Fact]
    public void ValidPhone_Passes()
    {
        var request = ValidRequest();
        request.Phone = "+20 100 123 4567";

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }
}
