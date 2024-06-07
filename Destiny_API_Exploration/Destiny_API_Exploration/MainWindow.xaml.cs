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
using Destiny_API_Exploration.ErrorHandling;
using Destiny_API_Exploration.Manifest;
using Destiny_API_Exploration.Objects;
using Destiny_API_Exploration.Rotators;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Button = System.Windows.Forms.Button;
using ListView = System.Windows.Forms.ListView;
using MessageBox = System.Windows.MessageBox;

namespace Destiny_API_Exploration;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string? authCode = null;
    private string AuthReq =
        "https://www.bungie.net/en/oauth/authorize?client_id=46798&response_type=code&state=6i0mkLx79Hp91nzWVeHrzHG4";

    private string TokenUri =
        "https://www.bungie.net/Platform/App/OAuth/token/";

    private Auth? Authorization;
    private DestinyMemberShip MainMemberShip;
    private CharacterIds CharIds = new CharacterIds();
    private HttpClient client = new HttpClient();
    private Dictionary<string, List<Item>> Inventories = [];
    private Dictionary<string, ItemProperties> ItemManifest;

    private void navigationEventHandler(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (!e.Uri.StartsWith("https://localhost:8888"))
            return;

        var uri = new Uri(e.Uri);
        authCode = HttpUtility.ParseQueryString(uri.Query).Get("code");

        GetToken();
        webView.IsEnabled = false;
        webView.Visibility = Visibility.Collapsed;
    }
    public MainWindow()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// starts the login process, navigating the webview to the login page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="a"></param>
    private async void Login(object sender, RoutedEventArgs a)
    {
        try
        {
            
            LoginButton.IsEnabled = false;
            LoginButton.Visibility = Visibility.Collapsed;
            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.CookieManager.DeleteAllCookies();
            webView.CoreWebView2.Reload();
            if (webView?.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate(AuthReq);
                webView.Visibility = Visibility.Visible;
            }

            webView!.NavigationStarting += (sender2, e) =>
            {
                if (!e.Uri.StartsWith("https://localhost:8888"))
                    return;

                var uri = new Uri(e.Uri);
                var code = HttpUtility.ParseQueryString(uri.Query).Get("code");
                if (authCode != code)
                {
                    authCode = code;
                    GetToken();
                    webView.IsEnabled = false;
                    webView.Visibility = Visibility.Collapsed;
                }
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// after the user has successfully logged in at the webview, this catches authorization tokens needed
    /// for the app to have access to their account.
    /// </summary>
    /// <returns></returns>
    private async Task<Auth> GetToken()
    {
        IEnumerable<KeyValuePair<string, string>> body = new KeyValuePair<string, string>[]
        {
            new ("grant_type", "authorization_code"),
            new ("code", $"{authCode}"),
            new ("client_id", "46798"),
            new ("client_secret", "PyjVap9a3b4cnBidGg2QhnKDW1vi6.vZ2AkMoIOwI34"),
        };
        
        var content = new FormUrlEncodedContent(body);
        content.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
        
        HttpResponseMessage response = await client.PostAsync("https://www.bungie.net/platform/app/oauth/token/", content);
        Authorization = JsonSerializer.Deserialize<Auth>(await response.Content.ReadAsStringAsync());
        ItemManifest = await ManifestGetter.GetItemManifest(client);
        await GetMemberShips();
        return Authorization!;
    }
    /// <summary>
    /// Takes a look at the user's account and catches any membership that is valid for the user.
    /// These represent the different platforms the user plays Destiny 2 on. Since cross save is a thing
    /// we do not care about which membership is used for our purposes, it does it's thing on every platform available to the user
    /// no matter which membership we decide to interact with.
    /// </summary>
    /// <returns></returns>
    private async Task<DestinyMemberShip> GetMemberShips()
    {
        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get,
            $"https://www.bungie.net/platform/User/GetBungieAccount/{Authorization?.membership_id}/254");
        req.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
        HttpResponseMessage res = await client.SendAsync(req);
        var responseToBungieAccount = JsonSerializer.Deserialize<ResponseToBungieAccount>(await res.Content.ReadAsStringAsync());
        MainMemberShip = responseToBungieAccount!.Response.destinyMemberships.First(member => true);
        await GetCharacterIds();
        return MainMemberShip;
    }

    /// <summary>
    /// Fetches the characters the user has on their account, inventory is shared between these.
    /// </summary>
    /// <returns></returns>
    private async Task<CharacterIds> GetCharacterIds()
    {
        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get,
            $"https://www.bungie.net/Platform/Destiny2/{MainMemberShip.membershipType}" +
                                 $"/Profile/{MainMemberShip.membershipId}/?components=100");
        req.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
        req.Headers.Add("Authorization", Authorization.token_type + " " + Authorization?.access_token);
        HttpResponseMessage res = await client.SendAsync(req);
        var response = JsonSerializer.Deserialize<ResponseToProfileGet>(await res.Content.ReadAsStringAsync());
        CharIds.characterIds = response!.Response.profile.data.characterIds;
        try
        {
            await getInventories();
        }
        catch (Exception e)
        {
            Console.WriteLine("chracterIds " + e.Message);
        }

        return CharIds;
    }
    /// <summary>
    /// Fetches the inventories of characters and the vault of the user.
    /// </summary>
    /// <returns></returns>
    private async Task<Dictionary<string, List<Item>>> getInventories()
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"https://www.bungie.net/Platform/Destiny2/{MainMemberShip.membershipType}" +
            $"/Profile/{MainMemberShip.membershipId}/?components=201");
        req.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
        req.Headers.Add("Authorization", Authorization.token_type + " " + Authorization?.access_token);
        var res = await client.SendAsync(req);
        try
        {
            var characters =
                JsonSerializer.Deserialize<GetCharacterInventoriesResponse>(await res.Content.ReadAsStringAsync());
            foreach (var character in characters.Response.characterInventories.data.Keys)
            {
                Inventories.TryAdd(character, []);
                Inventories[character].Clear();
                foreach (var item in characters.Response.characterInventories.data[character].items)
                {
                    if (item.transferStatus == 0 || item.transferStatus == 1) // is transferable
                    {
                        ItemProperties props;
                        if (ItemManifest.TryGetValue(item.itemHash.ToString(), out props))
                        {
                            item.name = props.displayProperties.name;
                        }
                        Inventories[character].Add(item);
                        item.currentlyIn = character;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new Dictionary<string, List<Item>>();
        }
        req = new HttpRequestMessage(HttpMethod.Get,
            $"https://www.bungie.net/Platform/Destiny2/{MainMemberShip.membershipType}" +
            $"/Profile/{MainMemberShip.membershipId}/?components=102");
        req.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
        req.Headers.Add("Authorization", Authorization.token_type + " " + Authorization?.access_token);
        res = await client.SendAsync(req);
        try
        {
            var vault =
                JsonSerializer.Deserialize<GetVaultInventoryResponse>(await res.Content.ReadAsStringAsync());
            Inventories.TryAdd("vault", []);
            Inventories["vault"].Clear();
            foreach (var item in vault.Response.profileInventory.data.items)
            {
                var alreadySomewhere = false;
                if ((item.transferStatus == 0 || item.transferStatus == 1)) // is transferable
                {
                    foreach (var inventory in Inventories.Values)
                    {
                        if (inventory.Contains(item))
                        {
                            alreadySomewhere = true;
                            break;
                        }

                    }

                    if (alreadySomewhere)
                    {
                        continue;
                    }
                    ItemProperties props;
                    if (ItemManifest.TryGetValue(item.itemHash.ToString(), out props))
                    {
                        item.name = props.displayProperties.name;
                    }
                    Inventories["vault"].Add(item);
                    item.currentlyIn = "vault";
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        if (CharIds.characterIds.Length > 0)
        {
            character1.Items.Clear();
            character1.Visibility = Visibility.Visible;
            charName1.Visibility = Visibility.Visible;
            charName1.Content = CharIds.characterIds[0];
            foreach (var item in Inventories[CharIds.characterIds[0]])
            {
                character1.Items.Add(item);
            }
            char1Transfer.Content = "Transfer\n to\n" + CharIds.characterIds[0];
            char1Equip.Content = "Equip\n on\n" + CharIds.characterIds[0];
        }

        if (CharIds.characterIds.Length > 1)
        {
            character2.Items.Clear();
            character2.Visibility = Visibility.Visible;
            charName2.Visibility = Visibility.Visible;
            charName2.Content = CharIds.characterIds[1];
            foreach (var item in Inventories[CharIds.characterIds[1]])
            {
                character2.Items.Add(item);
            }
            char2Transfer.Content = "Transfer\n to\n" + CharIds.characterIds[1];
            char2Equip.Content = "Equip\n on\n" + CharIds.characterIds[1];
        }

        if (CharIds.characterIds.Length > 2)
        {
            character3.Items.Clear();
            character3.Visibility = Visibility.Visible;
            charName3.Visibility = Visibility.Visible;
            charName3.Content = CharIds.characterIds[2];
            foreach (var item in Inventories[CharIds.characterIds[2]])
            {
                character3.Items.Add(item);
            }
            char3Transfer.Content = "Transfer\n to\n" + CharIds.characterIds[2];
            char3Equip.Content = "Equip\n on\n" + CharIds.characterIds[2];
        }

        vault.Visibility = Visibility.Visible;
        vaultName.Visibility = Visibility.Visible;
        vault.Items.Clear();
        foreach (var item in Inventories["vault"])
        {
            vault.Items.Add(item);
        }

        SelectedItem.Visibility = Visibility.Visible;
        LogOut.Visibility = Visibility.Visible;
        return Inventories;
    }

    /// <summary>
    /// Equips and item on a character, if Item is not present on chosen character, it transfers it there first.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Equip(object sender, RoutedEventArgs e)
    {
        var endpoint = "https://www.bungie.net/platform/Destiny2/Actions/Items/EquipItem/";
        var item = SelectedItem.Content as Item;
        var splitter = sender.ToString().Split("\n");
        string who = splitter[2];
        if (item.currentlyIn != who)
        {
            Transfer(sender, e);
        }
        StringContent jsonContent = new(
            JsonSerializer.Serialize(new
            {
                itemId = item.itemInstanceId,
                characterId = item.currentlyIn,
                membershipType = 3
            }),
            Encoding.UTF8,
            "application/json");
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, endpoint);
        message.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
        message.Headers.Add("Authorization", Authorization.token_type + " " + Authorization?.access_token);
        message.Content = jsonContent;
        var result = await client.SendAsync(message);
        if (result.StatusCode != HttpStatusCode.OK)
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(await result.Content.ReadAsStringAsync());
            MessageBox.Show(error.Message, "could not equip", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None);
        }
        try
        {
            await getInventories();
        }
        catch (Exception x)
        {
            Console.WriteLine("equip " + x.Message);
        }
        char1Transfer.Visibility = Visibility.Hidden;
        char1Equip.Visibility = Visibility.Hidden;
        char2Transfer.Visibility = Visibility.Hidden;
        char2Equip.Visibility = Visibility.Hidden;
        char3Transfer.Visibility = Visibility.Hidden;
        char3Equip.Visibility = Visibility.Hidden;
        ToVault.Visibility = Visibility.Hidden;
        SelectedItem.Visibility = Visibility.Hidden;
    }
    
    /// <summary>
    /// Transfers an item, from anywhere (whether it be a character or vault) to anywhere (character or vault)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Transfer(object sender, RoutedEventArgs e)
    {
        var endpoint = "https://www.bungie.net/platform/Destiny2/Actions/Items/TransferItem/";
        var item = SelectedItem.Content as Item;
        var splitter = sender.ToString().Split("\n");
        string who = "vault";
        if (splitter.Length == 3)
        {
            who = splitter[2];
        }
        var vault = false;
        
        StringContent jsonContent;
        if (item.currentlyIn != "vault")
        { 
            jsonContent = new(
                JsonSerializer.Serialize(new
                {
                    itemReferenceHash = item.itemHash,
                    stackSize = item.quantity,
                    transferToVault = true,
                    itemId = item.itemInstanceId,
                    characterId = item.currentlyIn,
                    membershipType = MainMemberShip.membershipType
                }),
                Encoding.UTF8,
                "application/json");
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, endpoint);
            message.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
            message.Headers.Add("Authorization", Authorization.token_type + " " + Authorization?.access_token);
            message.Content = jsonContent;
            
            var result = await client.SendAsync(message);
            if (result.StatusCode != HttpStatusCode.OK)
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(await result.Content.ReadAsStringAsync());
                MessageBox.Show(error.Message, "cannot transfer to vault, maybe it's full", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None);
                return;
            }

            item.currentlyIn = "vault";
            if (who == "vault")
            {
                try
                {
                    await getInventories();
                }
                catch (Exception x)
                {
                    Console.WriteLine("transfer " + x.Message);
                }
                return;
            }
        }
        jsonContent = new(
            JsonSerializer.Serialize(new
            {
                itemReferenceHash = item.itemHash,
                stackSize = item.quantity,
                transferToVault = false,
                itemId = item.itemInstanceId,
                characterId = who,
                membershipType = MainMemberShip.membershipType
            }),
            Encoding.UTF8,
            "application/json");
        HttpRequestMessage message2 = new HttpRequestMessage(HttpMethod.Post, endpoint);
        message2.Headers.Add("X-API-Key", "f12e32517f1a4b72aa46e39c42e944a7");
        message2.Headers.Add("Authorization", Authorization.token_type + " " + Authorization?.access_token);
        message2.Content = jsonContent;
        
        var res = await client.SendAsync(message2);
        if (res.StatusCode != HttpStatusCode.OK)
        {
            var error = JsonSerializer.Deserialize<ErrorResponse>(await res.Content.ReadAsStringAsync());
            MessageBox.Show(error.Message, "oops couldn't transfer", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None);
            try
            {
                await getInventories();
            }
            catch (Exception x)
            {
                Console.WriteLine("transfer 2 " + x.Message);
            }
        }
        else
        {
            item.currentlyIn = who;
            await getInventories();
        }
        char1Transfer.Visibility = Visibility.Hidden;
        char1Equip.Visibility = Visibility.Hidden;
        char2Transfer.Visibility = Visibility.Hidden;
        char2Equip.Visibility = Visibility.Hidden;
        char3Transfer.Visibility = Visibility.Hidden;
        char3Equip.Visibility = Visibility.Hidden;
        ToVault.Visibility = Visibility.Hidden;
        SelectedItem.Visibility = Visibility.Hidden;
    }
    
    /// <summary>
    /// When an item is selected in the list of items for the first character, it shows all relevant buttons for
    /// transferring/equipping and also which item is currently selected. Same goes for the next 3 methods.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Character1_OnSelected(object sender, RoutedEventArgs e)
    {
        SelectedItem.Content = character1.SelectedItem;

        if (CharIds.characterIds.Length > 0)
        {
            char1Transfer.Visibility = Visibility.Hidden;
            char1Equip.Visibility = Visibility.Visible;
        }

        if (CharIds.characterIds.Length > 1)
        {
            char2Transfer.Visibility = Visibility.Visible;
            char2Equip.Visibility = Visibility.Visible;
        }

        if (CharIds.characterIds.Length > 2)
        {
            char3Transfer.Visibility = Visibility.Visible;
            char3Equip.Visibility = Visibility.Visible;
        }
        ToVault.Visibility = Visibility.Visible;
    }
    
    private void Character2_OnSelected(object sender, RoutedEventArgs e)
    {
        SelectedItem.Content = character2.SelectedItem;
        
        if (CharIds.characterIds.Length > 0)
        {
            char1Transfer.Visibility = Visibility.Visible;
            char1Equip.Visibility = Visibility.Visible;
        }

        if (CharIds.characterIds.Length > 1)
        {
            char2Transfer.Visibility = Visibility.Hidden;
            char2Equip.Visibility = Visibility.Visible;
        }

        if (CharIds.characterIds.Length > 2)
        {
            char3Transfer.Visibility = Visibility.Visible;
            char3Equip.Visibility = Visibility.Visible;
        }
        ToVault.Visibility = Visibility.Visible;
    }
    
    private void Character3_OnSelected(object sender, RoutedEventArgs e)
    {
        SelectedItem.Content = character3.SelectedItem;
        
        if (CharIds.characterIds.Length > 0)
        {
            char1Transfer.Visibility = Visibility.Visible;
            char1Equip.Visibility = Visibility.Visible;
        }

        if (CharIds.characterIds.Length > 1)
        {
            char2Transfer.Visibility = Visibility.Visible;
            char2Equip.Visibility = Visibility.Visible;
        }

        if (CharIds.characterIds.Length > 2)
        {
            char3Transfer.Visibility = Visibility.Hidden;
            char3Equip.Visibility = Visibility.Visible;
        }
        ToVault.Visibility = Visibility.Visible;
    }
    
    private void Vault_OnSelected(object sender, RoutedEventArgs e)
    {
        SelectedItem.Content = vault.SelectedItem;
        
        if (CharIds.characterIds.Length > 0)
        {
            char1Transfer.Visibility = Visibility.Visible;
            char1Equip.Visibility = Visibility.Visible;
        }

        if (CharIds.characterIds.Length > 1)
        {
            char2Transfer.Visibility = Visibility.Visible;
            char2Equip.Visibility = Visibility.Visible;
        }

        if (CharIds.characterIds.Length > 2)
        {
            char3Transfer.Visibility = Visibility.Visible;
            char3Equip.Visibility = Visibility.Visible;
        }
        ToVault.Visibility = Visibility.Hidden;
    }
    
    /// <summary>
    /// On logout we clear everything including authorization and navigate the webview back to bungie.net,
    /// we also make the login button visible again.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void LogOut_OnClick(object sender, RoutedEventArgs e)
    {
        HideInventories();
        character1.Items.Clear();
        character2.Items.Clear();
        character3.Items.Clear();
        vault.Items.Clear();
        Authorization = null;
        
        webView.CoreWebView2.Navigate("https://www.bungie.net/");
        webView.IsEnabled = true;
        LoginButton.IsEnabled = true;
        LoginButton.Visibility = Visibility.Visible;
    }
    
    /// <summary>
    /// We hide inventories, if possible and the login button, also if possible, we then show raid and dungeon rotations.
    /// Apologies for this being hard-coded, originally, it was intended to pull from the API, however,
    /// The Final Shape (DLC that released on the 4th of june 2024) seems to have... broken it.
    /// I currently have no idea where to fetch this data from in the API.
    /// Originally, there were also meant to be nightfalls (weekly) and lost sectors (daily), however, since it's a new season
    /// with a new rotation, I do not know these rotations yet, and thus they have been left out, for now. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void ShowRotations(object sender, RoutedEventArgs e)
    {
        HideInventories();
        LoginButton.Visibility = Visibility.Hidden;
        await Rotations();

        FeaturedRaid.Visibility = Visibility.Visible;
        FeaturedDungeon.Visibility = Visibility.Visible;
        RaidLabel.Visibility = Visibility.Visible;
        DungeonLabel.Visibility = Visibility.Visible;
        HideRotators.Visibility = Visibility.Visible;
        ShowRotators.Visibility = Visibility.Hidden;
    }
    /// <summary>
    /// Hides rotators, goes back to inventory if user is authorized, back to login page if not.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void HideRotations(object sender, RoutedEventArgs e)
    {
        if (Authorization != null)
        {
            SelectedItem.Visibility = Visibility.Visible;
            SelectedItem.Content = "";
            character1.Visibility = Visibility.Visible;
            character2.Visibility = Visibility.Visible;
            character3.Visibility = Visibility.Visible;
        
            charName1.Visibility = Visibility.Visible;
            charName2.Visibility = Visibility.Visible;
            charName3.Visibility = Visibility.Visible;
            vault.Visibility = Visibility.Visible;
            vaultName.Visibility = Visibility.Visible;
            LogOut.Visibility = Visibility.Visible;
        }
        else
        {
            LoginButton.Visibility = Visibility.Visible;
        }
        FeaturedRaid.Visibility = Visibility.Hidden;
        FeaturedDungeon.Visibility = Visibility.Hidden;
        RaidLabel.Visibility = Visibility.Hidden;
        DungeonLabel.Visibility = Visibility.Hidden;
        HideRotators.Visibility = Visibility.Hidden;
        ShowRotators.Visibility = Visibility.Visible;
    }
    /// <summary>
    /// helper function to reduce code duplication, hides inventories.
    /// </summary>
    public async void HideInventories()
    {
        char1Transfer.Visibility = Visibility.Collapsed;
        char1Equip.Visibility = Visibility.Collapsed;
        char2Transfer.Visibility = Visibility.Collapsed;
        char2Equip.Visibility = Visibility.Collapsed;
        char3Transfer.Visibility = Visibility.Collapsed;
        char3Equip.Visibility = Visibility.Collapsed;
        ToVault.Visibility = Visibility.Collapsed;
        SelectedItem.Visibility = Visibility.Hidden;
        SelectedItem.Content = "";
        character1.Visibility = Visibility.Hidden;
        character2.Visibility = Visibility.Hidden;
        character3.Visibility = Visibility.Hidden;
        
        charName1.Visibility = Visibility.Hidden;
        charName2.Visibility = Visibility.Hidden;
        charName3.Visibility = Visibility.Hidden;
        vault.Visibility = Visibility.Hidden;
        vaultName.Visibility = Visibility.Hidden;
        LogOut.Visibility = Visibility.Hidden;
    }
    
    /// <summary>
    /// fetches raid rotations and puts them into their respective labels.
    /// this is called every time "ShowRotations" is called, thus, if we click show rotations after reset, it should correctly show the new rotation every time!
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Rotations()
    {
        FeaturedRaid.Content = await RotatorWorker.RaidRotation();
        FeaturedDungeon.Content = await RotatorWorker.DungeonRotation();
        return true;
    }
}