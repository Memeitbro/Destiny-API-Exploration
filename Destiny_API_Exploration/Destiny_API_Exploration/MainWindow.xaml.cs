using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Destiny_API_Exploration.Objects;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Destiny_API_Exploration;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    private string AuthReq =
        "https://www.bungie.net/en/oauth/authorize?client_id=46798&response_type=code&state=6i0mkLx79Hp91nzWVeHrzHG4";

    private string TokenUri =
        "https://www.bungie.net/Platform/App/OAuth/token/";

    private Auth? Authorization;
    private DestinyMemberShip MainMemberShip;
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private async void Login(object sender, RoutedEventArgs a)
    {
        LoginButton.IsEnabled = false;
        LoginButton.Visibility = Visibility.Collapsed;
        HttpClient client = new HttpClient();
        await webView.EnsureCoreWebView2Async();
        if (webView?.CoreWebView2 != null)
        {
            webView.CoreWebView2.Navigate(AuthReq);
            webView.Visibility = Visibility.Visible;
        }

        string? code = "";
        webView!.NavigationStarting += (sender2, e) =>
        {
            if (!e.Uri.StartsWith("https://localhost:8888"))
                return;

            var uri = new Uri(e.Uri);
            code = HttpUtility.ParseQueryString(uri.Query).Get("code");
            
            GetToken(code, client);
            e.Cancel = true;
            webView.IsEnabled = false;
            webView.Visibility = Visibility.Collapsed;
        };
    }

    
    private async Task<Auth> GetToken(string code, HttpClient client)
    {
        IEnumerable<KeyValuePair<string, string>> body = new KeyValuePair<string, string>[]
        {
            new ("grant_type", "authorization_code"),
            new ("code", $"{code}"),
            new ("client_id", "46798"),
            new ("client_secret", "PyjVap9a3b4cnBidGg2QhnKDW1vi6.vZ2AkMoIOwI34"),
        };
        
        var content = new FormUrlEncodedContent(body);
        content.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");

        HttpResponseMessage response = await client.PostAsync("https://www.bungie.net/platform/app/oauth/token/", content);
        Authorization = JsonSerializer.Deserialize<Auth>(await response.Content.ReadAsStringAsync());
        await getMemberShips(client);
        return Authorization!;
    }

    private async Task<DestinyMemberShip> getMemberShips(HttpClient client)
    {
        Console.WriteLine($"https://www.bungie.net/platform/User/GetBungieAccount/{Authorization?.membership_id}/254");
        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get,
            $"https://www.bungie.net/platform/User/GetBungieAccount/{Authorization?.membership_id}/254");
        req.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
        HttpResponseMessage res = await client.SendAsync(req);
        var responseToBungieAccount = JsonSerializer.Deserialize<ResponseToBungieAccount>(await res.Content.ReadAsStringAsync());
        MainMemberShip = responseToBungieAccount!.Response.destinyMemberships.First(member => member.membershipType == 3);
        return MainMemberShip;
    }
    
}