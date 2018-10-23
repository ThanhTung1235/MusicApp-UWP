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
using Windows.Security.Authentication.Web;
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
        public async void GetInfoUser()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync("token.txt");
            string content = await FileIO.ReadTextAsync(file);
            TokenResponse member_token = JsonConvert.DeserializeObject<TokenResponse>(content);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + member_token.Token);
            var response = client.GetAsync(APIHandle.MEMBER_INFORMATION);
            var result = await response.Result.Content.ReadAsStringAsync();
            Member responseJsonMember = JsonConvert.DeserializeObject<Member>(result);
            this.name.Text = responseJsonMember.firstName + " " + responseJsonMember.lastName;
            this.txt_avatar.ProfilePicture = new BitmapImage(new Uri(responseJsonMember.avatar));
            //this.txt_address.Text = responseJsonMember.address;
            this.txt_birthday.Text = responseJsonMember.birthday;
            this.txt_gender.CharacterSpacing = responseJsonMember.gender;
            this.txt_phone.Text = responseJsonMember.phone;
            this.txt_email.Text = responseJsonMember.email;
            int gender_member = responseJsonMember.gender;
            switch (gender_member)
            {
                case 0:
                    this.txt_gender.Text =  "Nữ";
                    break;
                case 1:
                    this.txt_gender.Text = "Nam";
                    break;
                case 2:
                    this.txt_gender.Text = "Giới tính khác";
                    break;
            }

        }
    }
}
                                         