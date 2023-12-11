using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Network network = new Network();
        string player_login;
        string move = "";
        int player_pos=3;
        string default_board = "rnbqkbnr/pppppppp/00000000/00000000/00000000/00000000/PPPPPPPP/RNBQKBNR";
        bool turn;
        bool start_game=false;
        static CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken ct = tokenSource.Token;
        private enum Mode
        {
            Login, Register
        }
        public MainWindow()
        {
            InitializeComponent();
            InitVisibility();
            network.con();
        }

        private void ShowPassword_PreviewMouseDown(object sender, MouseButtonEventArgs e) => ShowPasswordFunction();
        private void ShowPassword_PreviewMouseUp(object sender, MouseButtonEventArgs e) => HidePasswordFunction();
        private void ShowPassword_MouseLeave(object sender, MouseEventArgs e) => HidePasswordFunction();

        private void ShowPasswordFunction()
        {
            ShowPassword.Source = new BitmapImage(new Uri("show_hide/show.png", UriKind.Relative));
            PasswordUnmask.Visibility = Visibility.Visible;
            PasswordHidden.Visibility = Visibility.Hidden;
            PasswordUnmask.Text = PasswordHidden.Password;
        }

        private void HidePasswordFunction()
        {
            ShowPassword.Source = new BitmapImage(new Uri("show_hide/hide.png", UriKind.Relative));
            PasswordUnmask.Visibility = Visibility.Hidden;
            PasswordHidden.Visibility = Visibility.Visible;
        }

        private void InitVisibility()
        {
            StartButtons.Visibility = Visibility.Visible;
            LoginPassword.Visibility = Visibility.Collapsed;
            Lobby.Visibility = Visibility.Collapsed;
            Game.Visibility = Visibility.Collapsed;
            Login.Text = "";
            PasswordHidden.Password = "";
        }
        
        private void MainVisibility()
        {
            StartButtons.Visibility = Visibility.Collapsed;
            LoginPassword.Visibility = Visibility.Collapsed;
            Lobby.Visibility = Visibility.Visible;
            Game.Visibility = Visibility.Collapsed;
            Refresh();
        }

        private void GameVisibility()
        {
            Grid parent = Chess_Grid;
            parent.Children.Clear();
            StartButtons.Visibility = Visibility.Collapsed;
            LoginPassword.Visibility = Visibility.Collapsed;
            Lobby.Visibility = Visibility.Collapsed;
            Game.Visibility = Visibility.Visible;            
        }

        private void CreateBoard(string board)
        {
            Grid parent = Chess_Grid;
            parent.Children.Clear();
            string[] board_rows = board.Split('/');
            for (int i = 0;  i < 8; i++)
            {
                for (int  j = 0; j < 8; j++)
                {
                    Button btn = new Button();
                    if (player_pos == 0)
                    {
                        btn.Uid = IntChar(i) + (8 - j).ToString();
                        btn.Content = board_rows[8 - (j + 1)][i];
                    }
                    else
                    {
                        btn.Uid = IntChar(8 - (i + 1)) + (j + 1).ToString();
                        btn.Content = board_rows[j][8 - (i + 1)];
                    }
                    if (i % 2 == 1)
                        if (j % 2 == 0)
                            btn.Background = Brushes.SandyBrown;
                        else
                            btn.Background = Brushes.Wheat;
                    else
                        if (j % 2 == 1)
                            btn.Background = Brushes.SandyBrown;
                        else
                            btn.Background = Brushes.Wheat;
                    if (btn.Content.ToString() != "0")
                    {
                        if (char.IsLower(btn.Content.ToString().ToCharArray()[0]))
                            btn.Content = new Image
                            {
                                Source = new BitmapImage(new Uri($"Chess_pieces/{btn.Content.ToString().ToCharArray()[0]}w.png", UriKind.Relative))
                            };
                        else
                            btn.Content = new Image
                            {
                                Source = new BitmapImage(new Uri($"Chess_pieces/{btn.Content.ToString().ToCharArray()[0]}b.png", UriKind.Relative))
                            };
                    }
                    else
                    {
                        btn.Content = "";
                    }
                    Grid.SetColumn(btn, i);
                    Grid.SetRow(btn, j);    
                    if (turn)
                    btn.Click += ChessGrid_Click;
                    parent.Children.Add(btn);
                }
            }
        }

        private char IntChar(int i)
        {
            switch (i)
            {
                case 0:
                    return 'a';
                case 1:
                    return 'b';
                case 2:
                    return 'c';
                case 3:
                    return 'd';
                case 4:
                    return 'e';
                case 5:
                    return 'f';
                case 6:
                    return 'g';
                case 7:
                    return 'h';
                case 8:
                    return 'i';
                default:
                    return '0';
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            StartButtons.Visibility = Visibility.Collapsed;
            LoginPassword.Visibility = Visibility.Visible;
            RegisterSend.Visibility = Visibility.Collapsed;
            LoginSend.Visibility = Visibility.Visible;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            StartButtons.Visibility = Visibility.Collapsed;
            LoginPassword.Visibility = Visibility.Visible;
            RegisterSend.Visibility = Visibility.Visible;
            LoginSend.Visibility = Visibility.Collapsed;
        }

        private void LoginSend_Click(object sender, RoutedEventArgs e)
        {
            if (Login.Text != "" && PasswordHidden.Password != "")
            {
                network.snd("/login");
                network.rcv();
                network.snd(Login.Text.ToString());
                network.rcv();
                network.snd(PasswordHidden.Password.ToString());
                string res = network.rcv();
                if (res.Contains("Успешный вход"))
                {
                    player_login = Login.Text.ToString();
                    MainVisibility();
                }
                else
                {
                    MessageBox.Show(res, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                }
            }
            else
            {
                MessageBox.Show("Есть незаполненные поля!", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
        }

        private void RegisterSend_Click(object sender, RoutedEventArgs e)
        {
            if (Login.Text != "" && PasswordHidden.Password != "")
            {
                network.snd("/register");
                network.rcv();
                network.snd(Login.Text.ToString());
                network.rcv();
                network.snd(PasswordHidden.Password.ToString());
                string res = network.rcv();
                if (res.Contains("Регистрация успешна"))
                {
                    InitVisibility();
                }
                else
                {
                    MessageBox.Show(res, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
                }
            }
            else
            {
                MessageBox.Show("Есть незаполненные поля!", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
        }

        private void RefreshSend_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            network.snd("/lobby");
            string res = network.rcv();
            string[] str_lobbies = res.Split('\n');
            StackPanel parent = Lobbies;
            parent.Children.Clear();
            for (int i = 0; i < str_lobbies.Length - 1; i++)
            {
                Button btn = new Button();
                string[] elem = str_lobbies[i].Split(' ');
                btn.Uid = elem[2];
                btn.Content = str_lobbies[i].ToString();
                btn.Click += Connect_Click;
                parent.Children.Add(btn);
            }
        }

        private void ChessGrid_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            move+=button.Uid;
            if (move.Length==4)
            {
                network.snd("/move " + move);
                move = "";
                string res = network.rcv();
                if (!res.Contains("win"))
                {
                    turn = false;
                    CreateBoard(res);
                    Task.Run((Action)CheckTask, ct);
                }
                else
                {
                    Restart.Visibility = Visibility.Visible;
                    Wait.Text = "Вы победили";
                    Grid parent = Chess_Grid;
                    parent.Children.Clear();
                }
            }
        }

        private void CheckTask()
        {
            if (!ct.IsCancellationRequested)
                network.snd("/check");
            while (true)
            {
                if (!ct.IsCancellationRequested)
                {
                    string res = network.rcv();
                    if (res.Contains("wait"))
                    {
                        start_game = false;
                        Dispatcher.Invoke(() => Wait.Text = "Ожидание оппонента");
                        Grid parent = Chess_Grid;
                        Dispatcher.Invoke(() => parent.Children.Clear());
                        network.snd("/check");
                        Dispatcher.Invoke(() => Enemy_info.Visibility = Visibility.Collapsed);
                        continue;
                    }
                    else
                    {
                        if (res.Contains("alive"))
                        {
                            start_game = true;
                            network.snd("/check");
                            continue;
                        }
                        else
                        {                        
                            if (res.Contains("первый игрок"))
                            {
                                string[] res_parts = res.Split(' ');
                                Dispatcher.Invoke(() => Enemy_login.Text = res_parts[3]);
                                Dispatcher.Invoke(() => Enemy_wins.Text = res_parts[6]);
                                Dispatcher.Invoke(() => Enemy_info.Visibility = Visibility.Visible);
                                player_pos = 0;
                                turn = true;
                                move = "";
                                Dispatcher.Invoke(() => CreateBoard(default_board));
                                break;
                            }
                            else
                            {
                                if (res.Contains("второй игрок"))
                                {
                                    string[] res_parts = res.Split(' ');
                                    Dispatcher.Invoke(() => Enemy_login.Text = res_parts[3]);
                                    Dispatcher.Invoke(() => Enemy_wins.Text = res_parts[6]);
                                    Dispatcher.Invoke(() => Enemy_info.Visibility = Visibility.Visible);
                                    player_pos = 1;
                                    turn = false;
                                    move = "";
                                    Dispatcher.Invoke(() => CreateBoard(default_board));
                                    Task.Run((Action)CheckTask, ct);
                                    break;
                                }
                                else
                                {
                                    if (res.Contains("lose"))
                                    {
                                        Dispatcher.Invoke(() => Wait.Text = "Вы проиграли");
                                        Grid parent = Chess_Grid;
                                        Dispatcher.Invoke(() => parent.Children.Clear());
                                        start_game = false;
                                        turn=true;
                                        Dispatcher.Invoke(() => Restart.Visibility=Visibility.Visible);                                        
                                        break;
                                    }   
                                    else
                                    {                                    
                                        turn = true;
                                        Dispatcher.Invoke(() => CreateBoard(res));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    network.rcv();
                    Dispatcher.Invoke(() => Task.Run((Action)Leave));
                    break; 
                }
            }
        }

        private void CreateSend_Click(object sender, RoutedEventArgs e)
        {
            Enemy_info.Visibility = Visibility.Collapsed;
            Restart.Visibility = Visibility.Collapsed;
            network.snd("/create");
            network.rcv();
            GameVisibility();
            Task.Run((Action)CheckTask, ct);
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            Enemy_info.Visibility = Visibility.Collapsed;
            Restart.Visibility = Visibility.Collapsed;
            var button = sender as Button;            
            network.snd("/connect " + button.Uid);
            string res = network.rcv();
            if (res.Contains("Подключен"))
            {
                GameVisibility();
                Task.Run((Action)CheckTask, ct);
            }
            else
            {
                MessageBox.Show(res, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.Yes);
            }
        }

        private void LeaveSend_Click(object sender, RoutedEventArgs e)
        {
            if (!turn)
                tokenSource.Cancel();
            else
                Leave();
        }

        private void Leave()
        {
            network.snd("/leave");
            network.rcv();
            start_game = false;
            player_pos = 3;
            tokenSource = new CancellationTokenSource();
            ct = tokenSource.Token;
            Dispatcher.Invoke(() => MainVisibility());
        }

        private void QuitSend_Click(object sender, RoutedEventArgs e)
        {
            network.snd("/quit");
            network.rcv();
            Application.Current.Shutdown();
        }
        private void RestartSend_Click(object sender, RoutedEventArgs e)
        {
            Restart.Visibility = Visibility.Collapsed;
            Task.Run((Action)CheckTask, ct);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            InitVisibility();
        }

        private void DeleteSend_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы точно уверенны?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                network.snd("/delete");
                network.rcv();
                InitVisibility();
            }            
        }
    }
}
