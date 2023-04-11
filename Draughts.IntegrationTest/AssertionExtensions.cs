using FluentAssertions;
using FluentAssertions.Primitives;
using Flurl.Http;
using Xunit;

namespace Draughts.IntegrationTest;

public static class FlurlExtensions {
    public static FlurlResponseAssertions Should(this IFlurlResponse actualValue) => new(actualValue);
}

public sealed class FlurlResponseAssertions : ObjectAssertions<IFlurlResponse, FlurlResponseAssertions> {
    public FlurlResponseAssertions(IFlurlResponse value) : base(value) { }

    public void HaveStatusCode(int expectedStatusCode) {
        if (Subject.StatusCode == expectedStatusCode) {
            Subject.StatusCode.Should().Be(expectedStatusCode);
            return;
        }

        Assert.Fail($"StatusCode should be {expectedStatusCode}, but was {Subject.StatusCode}.\n"
            + $"Content: {Subject.ResponseMessage.Content}.\n"
            + $"RequestMessage: {Subject.ResponseMessage.RequestMessage}");
    }
}
