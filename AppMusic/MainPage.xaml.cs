using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppMusic
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string CurrentTag = " ";
        public MainPage()
        {
            this.InitializeComponent();
        }

        //private async void CheckAuthentication()
        //{
        //    bool isLogged = false;
        //    // trường hợp chưa có token key trong hệ thống.
        //    if (Service.ApiHandle.TOKEN_STRING == null)
        //    {
        //        // Lấy token từ trong file.
        //        if (await folder.TryGetItemAsync("token.txt") != null)
        //        {
        //            try
        //            {
        //                Windows.Storage.StorageFile file = await folder.GetFileAsync("token.txt");
        //                string fileContent = await Windows.Storage.FileIO.ReadTextAsync(file);
        //                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(fileContent);
        //                Service.ApiHandle.TOKEN_STRING = token.token;
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.WriteLine(e.Message);
        //            }
        //        }
        //    }
        //    // Check tính hợp lệ của token của api.
        //    if (Service.ApiHandle.TOKEN_STRING != null)
        //    {
        //        if (await Service.ApiHandle.GetInformation())
        //        {
        //            isLogged = true;
        //        }
        //    }
        //    if (!isLogged)
        //    {
        //        Login login = new Login();
        //        await login.ShowAsync();
        //    }
        //}



        private void btn_bar_Click(object sender, RoutedEventArgs e)
        {
            this.My_SplitView.IsPaneOpen = !this.My_SplitView.IsPaneOpen;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (CurrentTag == radio.Tag.ToString())
            {
                return;
            }
            switch (radio.Tag.ToString())
            {
                case "Register":
                    CurrentTag = "Register";
                    this.My_Frame.Navigate(typeof(Views.Sign_Up));
                    break;
                case "Login":
                    CurrentTag = "Login";
                    this.My_Frame.Navigate(typeof(Views.Sign_In));
                    break;
                case "HotSong":
                    CurrentTag = "HotSong";
                    this.My_Frame.Navigate(typeof(Views.List_music));
                    break;
            }
        }
    }
}





