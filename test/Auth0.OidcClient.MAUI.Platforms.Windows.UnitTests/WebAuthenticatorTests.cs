using Auth0.OidcClient.Platforms.Windows;
using Microsoft.Windows.AppLifecycle;
using Moq;
using System.Text.Json.Nodes;
using Windows.ApplicationModel.Activation;
using Activator = Auth0.OidcClient.Platforms.Windows.Activator;
using AppActivationArguments = Auth0.OidcClient.Platforms.Windows.AppActivationArguments;

namespace Auth0.OidcClient.MAUI.Platforms.Windows.UnitTests;

public class WebAuthenticatorTests {

    [Fact]
    public async void Should_Throw_With_Default()
    {
        await Assert.ThrowsAsync<TypeInitializationException>(() =>
             WebAuthenticator.Default.AuthenticateAsync(new Uri("http://www.idp.com"), new Uri("nyapp://callback")));

    }

    [Fact]
    public async void Should_Throw_When_Not_Checked_For_Redirection_Activation()
    {
        var mockAppInstance = new Mock<IAppInstanceProxy>();
        var mockHelpers = new Mock<IHelpers>();
        var mockTasksManager = new Mock<ITasksManager>();

        Activator.RedirectActivationCheck = false;

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new WebAuthenticator(mockAppInstance.Object, mockHelpers.Object, mockTasksManager.Object).AuthenticateAsync(new Uri("http://www.idp.com"), new Uri("nyapp://callback")));

        Assert.Equal("The redirection check on app activation was not detected. Please make sure a call to Activator.CheckRedirectionActivation was made during App creation.", exception.Message);
    }

    [Fact]
    public async void Should_Not_Throw_When_Checked_For_Redirection_Activation()
    {
        var mockAppInstance = new Mock<IAppInstanceProxy>();
        var mockHelpers = new Mock<IHelpers>();
        var mockTasksManager = new Mock<ITasksManager>();

        Activator.RedirectActivationCheck = true;

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new WebAuthenticator(mockAppInstance.Object, mockHelpers.Object, mockTasksManager.Object).AuthenticateAsync(new Uri("http://www.idp.com"), new Uri("nyapp://callback")));

        Assert.NotEqual("The redirection check on app activation was not detected. Please make sure a call to Activator.CheckRedirectionActivation was made during App creation.", exception.Message);
    }

    [Fact]
    public async void Should_Throw_When_App_Not_Packaged()
    {
        var mockAppInstance = new Mock<IAppInstanceProxy>();
        var mockHelpers = new Mock<IHelpers>();
        var mockTasksManager = new Mock<ITasksManager>();

        Activator.RedirectActivationCheck = true;

        mockHelpers.SetupGet(h => h.IsAppPackaged).Returns(false);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new WebAuthenticator(mockAppInstance.Object, mockHelpers.Object, mockTasksManager.Object).AuthenticateAsync(new Uri("http://www.idp.com"), new Uri("nyapp://callback")));

        Assert.Equal("The WebAuthenticator requires a packaged app with an AppxManifest.", exception.Message);
    }

    [Fact]
    public async void Should_Throw_When_Uri_Protocol_Not_Declared()
    {
        var mockAppInstance = new Mock<IAppInstanceProxy>();
        var mockHelpers = new Mock<IHelpers>();
        var mockTasksManager = new Mock<ITasksManager>();

        Activator.RedirectActivationCheck = true;
        mockHelpers.SetupGet(h => h.IsAppPackaged).Returns(true);
        mockHelpers.Setup(h => h.IsUriProtocolDeclared("myapp")).Returns(false);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new WebAuthenticator(mockAppInstance.Object, mockHelpers.Object, mockTasksManager.Object).AuthenticateAsync(new Uri("http://www.idp.com"), new Uri("myapp://callback")));

        Assert.Equal($"The URI Scheme myapp is not declared in AppxManifest.xml.", exception.Message);
    }

    [Fact]
    public void Should_Open_Browser_With_State()
    {
        var mockAppInstance = new Mock<IAppInstanceProxy>();
        var mockHelpers = new Mock<IHelpers>();
        var mockTasksManager = new Mock<ITasksManager>();

        Activator.RedirectActivationCheck = true;
        mockHelpers.SetupGet(h => h.IsAppPackaged).Returns(true);
        mockHelpers.Setup(h => h.IsUriProtocolDeclared("myapp")).Returns(true);
        mockHelpers.Setup(h => h.OpenBrowser(It.IsAny<Uri>()));
        mockAppInstance.Setup(a => a.GetCurrentAppKey()).Returns("test");

        var webAuthenticator = new WebAuthenticator(mockAppInstance.Object, mockHelpers.Object, mockTasksManager.Object);

        // Do no await so we can leave the method again
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        webAuthenticator.AuthenticateAsync(new Uri("http://www.idp.com"), new Uri("myapp://callback"));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed


        mockHelpers.Verify(d => d.OpenBrowser(It.Is<Uri>(uri => HasStateParam(uri, "appInstanceKey") && HasStateParam(uri, "taskId") )));
    }

    [Fact]
    public async void Should_return_WebAuthenticatorResult_With_Original_State()
    {
        var mockAppInstance = new Mock<IAppInstanceProxy>();
        var mockHelpers = new Mock<IHelpers>();
        var mockTasksManager = new Mock<ITasksManager>();
        Uri authorizeUri = new Uri("https://www.idp.com");

        Activator.RedirectActivationCheck = true;
        mockHelpers.SetupGet(h => h.IsAppPackaged).Returns(true);
        mockHelpers.Setup(h => h.IsUriProtocolDeclared("myapp")).Returns(true);
        mockHelpers.Setup(h => h.OpenBrowser(It.IsAny<Uri>())).Callback((Uri uri) =>
        {
            authorizeUri = uri;
        });
        mockAppInstance.Setup(a => a.GetCurrentAppKey()).Returns("test");

        mockTasksManager.Setup(t => t.Add(It.IsAny<string>(), It.IsAny<TaskCompletionSource<Uri>>())).Callback((string taskId, TaskCompletionSource<Uri> task) =>
        {
            var query = System.Web.HttpUtility.ParseQueryString(authorizeUri.Query);
            var state = query["state"];
            task.TrySetResult(new Uri($"myapp://callback?state={state}"));
        });

        var webAuthenticator = new WebAuthenticator(mockAppInstance.Object, mockHelpers.Object, mockTasksManager.Object);
        var result = await webAuthenticator.AuthenticateAsync(new Uri("http://www.idp.com/?state=abc"), new Uri("myapp://callback"));

        Assert.NotNull(result);
        Assert.Equal("myapp://callback/?state=abc", result.CallbackUri.ToString());
    }

    [Fact]
    public void Should_Remove_Task_On_Cancel()
    {
        var mockAppInstance = new Mock<IAppInstanceProxy>();
        var mockHelpers = new Mock<IHelpers>();
        var mockTasksManager = new Mock<ITasksManager>();

        Activator.RedirectActivationCheck = true;
        mockHelpers.SetupGet(h => h.IsAppPackaged).Returns(true);
        mockHelpers.Setup(h => h.IsUriProtocolDeclared("myapp")).Returns(true);
        mockHelpers.Setup(h => h.OpenBrowser(It.IsAny<Uri>()));
        mockAppInstance.Setup(a => a.GetCurrentAppKey()).Returns("test");

        var cancellationTokenSource = new CancellationTokenSource();
        var webAuthenticator = new WebAuthenticator(mockAppInstance.Object, mockHelpers.Object, mockTasksManager.Object);

        webAuthenticator.AuthenticateAsync(new Uri("http://www.idp.com"), new Uri("myapp://callback"), cancellationTokenSource.Token);

        cancellationTokenSource.Cancel();

        mockTasksManager.Verify(d => d.Remove(It.IsAny<string>()));
    }

    [Fact]
    public async void Should_Throw_If_Cancelled_Before()
    {
        var mockAppInstance = new Mock<IAppInstanceProxy>();
        var mockHelpers = new Mock<IHelpers>();
        var mockTasksManager = new Mock<ITasksManager>();

        Activator.RedirectActivationCheck = true;
        mockHelpers.SetupGet(h => h.IsAppPackaged).Returns(true);
        mockHelpers.Setup(h => h.IsUriProtocolDeclared("myapp")).Returns(true);
        mockHelpers.Setup(h => h.OpenBrowser(It.IsAny<Uri>()));
        mockAppInstance.Setup(a => a.GetCurrentAppKey()).Returns("test");

        var cancellationTokenSource = new CancellationTokenSource();
        var webAuthenticator = new WebAuthenticator(mockAppInstance.Object, mockHelpers.Object, mockTasksManager.Object);

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            webAuthenticator.AuthenticateAsync(new Uri("http://www.idp.com"), new Uri("myapp://callback"),
                cancellationTokenSource.Token));
    }

    [Fact]
    public void Should_Resume_On_Activated()
    {
        var mockAppInstance = new MockAppInstanceProxy();
        var mockHelpers = new Mock<IHelpers>();
        var mockTasksManager = new Mock<ITasksManager>();

        Activator.RedirectActivationCheck = true;
        mockHelpers.SetupGet(h => h.IsAppPackaged).Returns(true);
        mockHelpers.Setup(h => h.IsUriProtocolDeclared("myapp")).Returns(true);
        mockHelpers.Setup(h => h.OpenBrowser(It.IsAny<Uri>()));

        new WebAuthenticator(mockAppInstance, mockHelpers.Object, mockTasksManager.Object);

        var jsonObject = new JsonObject
        {
            { "appInstanceKey", "abc" },
            { "taskId", "def" }
        };

        var query = System.Web.HttpUtility.ParseQueryString("");

        query["state"] = jsonObject.ToJsonString();

        UriBuilder uriBuilder = new UriBuilder("myapp://callback")
        {
            Query = query.ToString()
        };

        mockAppInstance.OnActivated(null, new AppActivationArguments()
        {
            Data = new MockProtocolActivatedEventArgs()
            {
                Uri = uriBuilder.Uri,
            },
            Kind = ExtendedActivationKind.Protocol
        });

        mockTasksManager.Verify(d => d.ResumeTask(It.IsAny<Uri>(), "def"));
    }

    private bool HasStateParam(Uri uri, string paramName)
    {
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        var state = Helpers.Decode(query["state"]);
        var jsonObject = JsonNode.Parse(state) as JsonObject;

        return jsonObject[paramName] != null;

    }
}

internal class MockAppInstanceProxy : IAppInstanceProxy
{
    public MockAppInstanceProxy()
    {
    }

    public event EventHandler<IAppActivationArguments> Activated;

    public void OnActivated(object? sender, AppActivationArguments e)
    {
        Activated?.Invoke(this, e);
    }

    public virtual string GetCurrentAppKey()
    {
        return AppInstance.GetCurrent().Key;
    }

}

public class MockProtocolActivatedEventArgs : IProtocolActivatedEventArgs
{
    public ActivationKind Kind { get; }
    public ApplicationExecutionState PreviousExecutionState { get; }
    public SplashScreen SplashScreen { get; }
    public Uri Uri { get; set; }
}