using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AppMusic.Entity;
using AppMusic.Services;
using Newtonsoft.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppMusic.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Get_info_user : Page
    {
        
        public Get_info_user()
        {
            this.InitializeComponent();
            this.test();
            Debug.WriteLine(ReadToken());
            //this.ReadToken();

        }

        public async Task<String> ReadToken()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("token.txt");
            string content = await FileIO.ReadTextAsync(file);
            TokenResponse member_token = JsonConvert.DeserializeObject<TokenResponse>(content);
            Debug.WriteLine(member_token.Token);
            return  member_token.Token;


        }

        public async void test()
        {
            HttpWebRequest request =(HttpWebRequest)WebRequest.Create("https://2-dot-backup-server-002.appspot.com/_api/v2/songs/");
            //String encoded = ReadToken().ToString();
            request.Headers.Add("Authorization", "Basic " + ReadToken().Result );
            request.Method = "GET";
            
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            StreamReader resStreamReader = new StreamReader(response.GetResponseStream());
            string result = resStreamReader.ReadToEnd();

            
            Debug.WriteLine(result);
        }
    }
}
