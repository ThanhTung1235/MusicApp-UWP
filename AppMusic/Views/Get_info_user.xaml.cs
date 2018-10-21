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
using Windows.UI.Xaml.Media.Imaging;
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
        private Member _currentMember;
        public Get_info_user()
        {
            this._currentMember = new Member();
            this.InitializeComponent();
            this.GetInfoUser();

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

        public async void GetInfoUser()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + ReadToken().Result);
            var response =  client.GetAsync(APIHandle.MEMBER_INFORMATION);
            var result = await response.Result.Content.ReadAsStringAsync();
             Member responseJsonMember = JsonConvert.DeserializeObject<Member>(result);
            this.txt_firstName.Text = responseJsonMember.firstName;
            this.txt_lastName.Text = responseJsonMember.lastName;
            this.txt_avatar.ProfilePicture = new BitmapImage(new Uri(responseJsonMember.avatar));
            this.txt_address.Text = responseJsonMember.address;
            this.txt_birthday.Text = responseJsonMember.birthday;
            this.txt_gender.CharacterSpacing = responseJsonMember.gender;
            this.txt_phone.Text = responseJsonMember.phone;
            Debug.WriteLine("Response"+ result);
            
        }
    }
}
                                         