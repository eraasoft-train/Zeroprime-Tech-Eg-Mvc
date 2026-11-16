using Zeroprime.Domain.Entities;
using Zeroprime.Domain.Enums;

namespace Zeroprime.UnitTests.Domain;

public class CompanyProfileTests
{
    [Fact]
    public void Create_WithValidArguments_ReturnsProfile()
    {
        var profile = CompanyProfile.Create(
            "ZeroPrime Technologies", "Description", "Mission", "Vision",
            "Smart Cities", "Cairo, Egypt", "zeroprime.tech@gmail.com", 2017);

        Assert.Equal("ZeroPrime Technologies", profile.Name);
        Assert.Equal(2017, profile.FoundingYear);
    }

    [Theory]
    [InlineData("", "zeroprime.tech@gmail.com", 2017)]
    [InlineData("   ", "zeroprime.tech@gmail.com", 2017)]
    [InlineData("ZeroPrime", "", 2017)]
    [InlineData("ZeroPrime", "zeroprime.tech@gmail.com", 1899)]
    [InlineData("ZeroPrime", "zeroprime.tech@gmail.com", 3000)]
    public void Create_WithInvalidArguments_Throws(string name, string email, int year)
    {
        Assert.Throws<ArgumentException>(() => CompanyProfile.Create(
            name, "Description", "Mission", "Vision", "Industry", "Cairo", email, year));
    }

    [Fact]
    public void Update_SetsUpdatedAt()
    {
        var profile = CompanyProfile.Create(
            "ZeroPrime Technologies", "Description", "Mission", "Vision",
            "Smart Cities", "Cairo, Egypt", "zeroprime.tech@gmail.com", 2017);

        profile.Update("New desc", "New mission", "New vision", "Industry", "Alexandria, Egypt", "new@example.com");

        Assert.NotNull(profile.UpdatedAt);
        Assert.Equal("New desc", profile.Description);
    }
}

public class ContactInquiryTests
{
    [Fact]
    public void Create_WithValidArguments_SetsNewStatusAndSubmittedAt()
    {
        var inquiry = ContactInquiry.Create("Jane Doe", "jane@example.com", "Deployment", "We want CanBank in our mall.");

        Assert.Equal(InquiryStatus.New, inquiry.Status);
        Assert.True(inquiry.SubmittedAt <= DateTime.UtcNow);
        Assert.Null(inquiry.Company);
        Assert.Null(inquiry.Phone);
    }

    [Theory]
    [InlineData("", "jane@example.com", "Subject", "Message body")]
    [InlineData("Jane", "", "Subject", "Message body")]
    [InlineData("Jane", "jane@example.com", "", "Message body")]
    [InlineData("Jane", "jane@example.com", "Subject", "")]
    public void Create_WithMissingRequiredFields_Throws(string name, string email, string subject, string message)
    {
        Assert.Throws<ArgumentException>(() => ContactInquiry.Create(name, email, subject, message));
    }

    [Fact]
    public void StatusTransitions_WorkAsExpected()
    {
        var inquiry = ContactInquiry.Create("Jane Doe", "jane@example.com", "Subject", "Message body");

        inquiry.MarkAsInProgress();
        Assert.Equal(InquiryStatus.InProgress, inquiry.Status);

        inquiry.MarkAsResolved();
        Assert.Equal(InquiryStatus.Resolved, inquiry.Status);

        inquiry.MarkAsClosed();
        Assert.Equal(InquiryStatus.Closed, inquiry.Status);
    }
}

public class ProductTests
{
    [Fact]
    public void Create_WithValidArguments_IsActive()
    {
        var product = Product.Create("CanBank", "Reverse vending machine", "RVM");

        Assert.True(product.IsActive);
        Assert.Equal("CanBank", product.Name);
    }

    [Theory]
    [InlineData("", "Description")]
    [InlineData("Name", "")]
    public void Create_WithInvalidArguments_Throws(string name, string description)
    {
        Assert.Throws<ArgumentException>(() => Product.Create(name, description, "Category"));
    }

    [Fact]
    public void ActivateDeactivate_TogglesFlag()
    {
        var product = Product.Create("CanBank", "Reverse vending machine", "RVM");

        product.Deactivate();
        Assert.False(product.IsActive);

        product.Activate();
        Assert.True(product.IsActive);
    }
}
