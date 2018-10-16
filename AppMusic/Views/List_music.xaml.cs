using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
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
    public sealed partial class List_music : Page
    {
        private ObservableCollection<Song> listSong;
        public static string tokenKey = null;
        private int _currentIndex;
        internal ObservableCollection<Song> ListSong { get => listSong; set => listSong = value; }
        private Song currentSong;
        private bool _isPlaying = false;
        public List_music()
        {
            ReadToken();
            this.InitializeComponent();
            this.currentSong = new Song();
            this.ListSong = new ObservableCollection<Song>();
            this.GetSong();

        }
        public static async void ReadToken()
        {
            if (tokenKey == null)
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.GetFileAsync("token.txt");
                string content = await FileIO.ReadTextAsync(file);
                TokenResponse member_token = JsonConvert.DeserializeObject<TokenResponse>(content);
                Debug.WriteLine("token la: " + member_token.Token);
                tokenKey = member_token.Token;
            }
        }

        public async void GetSong()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIHandle.GET_SONG);
            //String encoded = ReadToken().ToString();
            request.Headers.Add("Authorization", "Basic " + tokenKey);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            StreamReader resStreamReader = new StreamReader(response.GetResponseStream());
            string result = resStreamReader.ReadToEnd();
            ObservableCollection<Song> listSongs = JsonConvert.DeserializeObject<ObservableCollection<Song>>(result);

            foreach (var songs in listSongs)
            {
                ListSong.Add(songs);
            }


            Debug.WriteLine(result);
        }





        private async void btn_add(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            this.currentSong.name = this.txt_name.Text;
            this.currentSong.description = this.txt_description.Text;
            this.currentSong.singer = this.txt_singer.Text;
            this.currentSong.author = this.txt_author.Text;
            this.currentSong.thumbnail = this.txt_thumbnail.Text;
            this.currentSong.link = this.txt_link.Text;


            var jsonSong = JsonConvert.SerializeObject(this.currentSong);
            StringContent content = new StringContent(jsonSong, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + tokenKey);
            var response = client.PostAsync(APIHandle.REGISTER_SONG, content);
            var contents = await response.Result.Content.ReadAsStringAsync();
            if (response.Result.StatusCode == HttpStatusCode.Created)
            {
                Debug.WriteLine("Success");
            }
            else
            {
                ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(contents);
                if (errorResponse.error.Count > 0)
                {
                    foreach (var key in errorResponse.error.Keys)
                    {
                        var objBykey = this.FindName(key);
                        var value = errorResponse.error[key];
                        if (objBykey != null)
                        {
                            TextBlock textBlock = objBykey as TextBlock;
                            textBlock.Text = "* " + value;
                        }
                    }
                }
            }
            this.txt_name.Text = String.Empty;
            this.txt_description.Text = String.Empty;
            this.txt_singer.Text = String.Empty;
            this.txt_author.Text = String.Empty;
            this.txt_thumbnail.Text = String.Empty;
            this.txt_link.Text = String.Empty;
        }
        
        private void currentSongs(object sender, TappedRoutedEventArgs e)
        {
            StackPanel panel = sender as StackPanel;
            Song chooseSong = panel.Tag as Song;
            Debug.WriteLine(chooseSong.link);
            _currentIndex = this.MyListSong.SelectedIndex;
            Uri mp3Link = new Uri(chooseSong.link);
            this.mediaElement.Source = mp3Link;
            this.name_song.Text= this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
            Do_play();
        }

        private void Do_play()
        {
            _isPlaying = true;
            this.status_song.Text = "Now Playing :";
            this.mediaElement.Play();
            PlayButton.Icon = new SymbolIcon(Symbol.Pause);


        }
        private void Do_pause()
        {
            _isPlaying = false;
            this.status_song.Text = "pause Playing :";
            this.mediaElement.Pause();
            PlayButton.Icon = new SymbolIcon(Symbol.Play);
        }



        private void Player_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                Do_pause();
            }
            else
            {
                Do_play();
            }
        }

        private void btn_Previous(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            if (_currentIndex >= 0)
            {
                _currentIndex -= 1;
            }
            else
            {
                _currentIndex = listSong.Count - 1;
            }
            Uri mp3Link = new Uri(ListSong[_currentIndex].link);
            this.name_song.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
            this.mediaElement.Source = mp3Link;
            Debug.WriteLine(mp3Link);
            Do_play();

        }

        private void btn_Next(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            if (_currentIndex < ListSong.Count - 1)
            {
                _currentIndex += 1;
            }
            else
            {
                _currentIndex = 0;
            }
            Uri mp3Link = new Uri(ListSong[_currentIndex].link);
            this.name_song.Text = this.ListSong[_currentIndex].name + " - " + this.ListSong[_currentIndex].singer;
            Debug.WriteLine(mp3Link);
            this.mediaElement.Source = mp3Link;
            Do_play();
        }
    }
}
