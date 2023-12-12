using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection.Emit;
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
        string local_board;
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
                                Source = new BitmapImage(new Uri($"Chess_pieces/{btn.Content.ToString().ToCharArray()[0]}w.png", UriKind.Relative)),
                                Name = btn.Content.ToString().ToCharArray()[0]+"w"
                            };
                        else
                            btn.Content = new Image
                            {
                                Source = new BitmapImage(new Uri($"Chess_pieces/{btn.Content.ToString().ToCharArray()[0]}b.png", UriKind.Relative)),
                                Name = btn.Content.ToString().ToCharArray()[0]+"b"
                            };
                        if (turn)
                        {
                            if (player_pos == 0)
                            {
                                if ((btn.Content as Image).Name[1] == 'w')
                                    btn.Click += ChessGrid_Click;
                            }
                            else
                            {
                                if ((btn.Content as Image).Name[1] == 'b')
                                    btn.Click += ChessGrid_Click;
                            }
                        }
                    }
                    else
                    {
                        btn.Content = "";
                    }
                    Grid.SetColumn(btn, i);
                    Grid.SetRow(btn, j);                    
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
            button.Background = Brushes.Gold;
            move+=button.Uid;
            if (move.Length == 4)
            {
                if (move[0] == move[2] && move[1] == move[3])
                {
                    try
                    { 
                        CreateBoard(local_board);
                    }
                    catch
                    {
                        CreateBoard(default_board);
                    }
                    move = "";
                }
                else
                {
                    network.snd("/move " + move);
                    move = "";
                    string res = network.rcv();
                    if (!res.Contains("win"))
                    {
                        turn = false;
                        local_board = res;
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
            else
            {
                ShowPossibleMove(button);
            }
        }

        private void ShowPossibleMove(Button cords)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (IntChar(i) + (j + 1).ToString() != cords.Uid)
                    {
                        string uid = IntChar(i) + (j+1).ToString();
                        var el = GetByUid(Chess_Grid, uid) as Button;
                        el.Click -= ChessGrid_Click;
                    }
                    else
                    {

                    }
                }
            }
            var content = cords.Content as Image;
            switch (content.Name[0])
            {
                case 'r':
                case 'R':
                    {
                        int a = 1;
                        int b = 0;
                        int c = 0;
                        bool end = false;
                        while (!end)
                        {
                            while (true)
                            {
                                string uid = (char)(cords.Uid[0] + 1 * a) + ((int)Char.GetNumericValue(cords.Uid[1]) + 1 * b).ToString();
                                var el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if (content.Name[1] == 'w')
                                        {
                                            if ((el.Content as Image).Name[1] == 'b')
                                            {
                                                el.Click += ChessGrid_Click;
                                                el.Background = Brushes.Red;
                                                c++;
                                                break;
                                            }
                                            else
                                            {
                                                c++;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if ((el.Content as Image).Name[1] == 'w')
                                            {
                                                el.Click += ChessGrid_Click;
                                                el.Background = Brushes.Red;
                                                c++;
                                                break;
                                            }
                                            else
                                            {
                                                c++;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        el.Click += ChessGrid_Click;
                                        el.Background = Brushes.Green;
                                    }
                                    if (a > 0)
                                        a++;
                                    else
                                        if (a == 0)
                                        a = 0;
                                    else
                                        a--;
                                    if (b > 0)
                                        b++;
                                    else
                                        if (b == 0)
                                        b = 0;
                                    else
                                        b--;
                                }
                                else
                                {
                                    c++;
                                    break;
                                }
                            }
                            switch (c)
                            {
                                case 1:
                                    a = -1;
                                    b = 0;
                                    break;
                                case 2:
                                    a = 0;
                                    b = 1;
                                    break;
                                case 3:
                                    a = 0;
                                    b = -1;
                                    break;
                                case 4:
                                    end = true;
                                    break;
                            }
                        }
                        break;
                    }
                case 'n':
                case 'N':
                    {
                        int a = 0;
                        int b = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            
                            switch (i)
                            {
                                case 0:
                                    a = 1;
                                    b = 2;
                                    break;
                                case 1:
                                    a = -1;
                                    b = 2;
                                    break;
                                case 2:
                                    a = 2;
                                    b = 1;
                                    break;
                                case 3:
                                    a = 2;
                                    b = -1;
                                    break;
                                case 4:
                                    a = -2;
                                    b = 1;
                                    break;
                                case 5:
                                    a = -2;
                                    b = -1;
                                    break;
                                case 6:
                                    a = 1;
                                    b = -2;
                                    break;
                                case 7:
                                    a = -1;
                                    b = -2;
                                    break;
                            }
                            string uid = (char)(cords.Uid[0] + 1 * a) + ((int)Char.GetNumericValue(cords.Uid[1]) + 1 * b).ToString();
                            var el = GetByUid(Chess_Grid, uid) as Button;
                            if (el != null)
                            {
                                if (el.Content as string != "")
                                {
                                    if (content.Name[1] == 'w')
                                    {
                                        if ((el.Content as Image).Name[1] == 'b')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                    else
                                    {
                                        if ((el.Content as Image).Name[1] == 'w')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                else
                                {
                                    el.Click += ChessGrid_Click;
                                    el.Background = Brushes.Green;
                                }
                            }
                        }
                        break;
                    }
                case 'b':
                case 'B':
                    {
                        int a = 1;
                        int b = 1;
                        int c = 0;
                        bool end = false;
                        while (!end)
                        {
                            while(true)
                            {
                                string uid = (char)(cords.Uid[0] + 1*a) + ((int)Char.GetNumericValue(cords.Uid[1]) + 1*b).ToString();
                                var el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if (content.Name[1] == 'w')
                                        {
                                            if ((el.Content as Image).Name[1] == 'b')
                                            {
                                                el.Click += ChessGrid_Click;
                                                el.Background = Brushes.Red;
                                                c++;
                                                break;
                                            }
                                            else
                                            {
                                                c++;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if ((el.Content as Image).Name[1] == 'w')
                                            {
                                                el.Click += ChessGrid_Click;
                                                el.Background = Brushes.Red;
                                                c++;
                                                break;
                                            }
                                            else
                                            {
                                                c++;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        el.Click += ChessGrid_Click;
                                        el.Background = Brushes.Green;
                                    }
                                    if (a>0)
                                    a++;
                                    else
                                    a--;
                                    if (b>0)
                                    b++;
                                    else
                                    b--;
                                }
                                else
                                {
                                    c++;
                                    break;
                                }
                            }
                            switch (c)
                            {
                                case 1:
                                    a = -1;
                                    b = 1;
                                    break;
                                case 2:
                                    a = -1;
                                    b = -1;
                                    break;
                                case 3:
                                    a = 1;
                                    b = -1;
                                    break;
                                case 4:
                                    end=true;
                                    break;
                            }
                        }
                        break;
                    }
                case 'q':
                case 'Q':
                    {
                        int a = 1;
                        int b = 0;
                        int c = 0;
                        bool end = false;
                        while (!end)
                        {
                            while (true)
                            {
                                string uid = (char)(cords.Uid[0] + 1 * a) + ((int)Char.GetNumericValue(cords.Uid[1]) + 1 * b).ToString();
                                var el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if (content.Name[1] == 'w')
                                        {
                                            if ((el.Content as Image).Name[1] == 'b')
                                            {
                                                el.Click += ChessGrid_Click;
                                                el.Background = Brushes.Red;
                                                c++;
                                                break;
                                            }
                                            else
                                            {
                                                c++;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if ((el.Content as Image).Name[1] == 'w')
                                            {
                                                el.Click += ChessGrid_Click;
                                                el.Background = Brushes.Red;
                                                c++;
                                                break;
                                            }
                                            else
                                            {
                                                c++;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        el.Click += ChessGrid_Click;
                                        el.Background = Brushes.Green;
                                    }
                                    if (a > 0)
                                        a++;
                                    else
                                        if (a == 0 && c<4)
                                        a = 0;
                                    else
                                        a--;
                                    if (b > 0)
                                        b++;
                                    else
                                        if (b == 0 && c < 4)
                                        b = 0;
                                    else
                                        b--;
                                }
                                else
                                {
                                    c++;
                                    break;
                                }
                            }
                            switch (c)
                            {
                                case 1:
                                    a = -1;
                                    b = 0;
                                    break;
                                case 2:
                                    a = 0;
                                    b = 1;
                                    break;
                                case 3:
                                    a = 0;
                                    b = -1;
                                    break;
                                case 4:
                                    a = -1;
                                    b = 1;
                                    break;
                                case 5:
                                    a = -1;
                                    b = -1;
                                    break;
                                case 6:
                                    a = 1;
                                    b = -1;
                                    break;
                                case 7:
                                    a = 1;
                                    b = 1;
                                    break;
                                case 8:
                                    end = true;
                                    break;
                            }
                        }
                        break;
                    }
                case 'k':
                case 'K':
                    {
                        string uid = (char)(cords.Uid[0] - 1) + cords.Uid[1].ToString();
                        var el = GetByUid(Chess_Grid, uid) as Button;
                        if (el != null)
                        {
                            if (el.Content as string != "")
                            {
                                if (content.Name[1] == 'w')
                                {
                                    if ((el.Content as Image).Name[1] == 'b')
                                    {
                                        el.Click += ChessGrid_Click;
                                        el.Background = Brushes.Red;
                                    }
                                }
                                else
                                {
                                    if ((el.Content as Image).Name[1] == 'w')
                                    {
                                        el.Click += ChessGrid_Click;
                                        el.Background = Brushes.Red;
                                    }
                                }
                            }
                            else
                            {
                                el.Click += ChessGrid_Click;
                                el.Background = Brushes.Green;
                            }
                        }
                        uid = (char)(cords.Uid[0] + 1) + cords.Uid[1].ToString();
                        el = GetByUid(Chess_Grid, uid) as Button;
                        if (el != null)
                        {
                            if (el.Content as string != "")
                            {
                                if (content.Name[1] == 'w')
                                {
                                    if ((el.Content as Image).Name[1] == 'b')
                                    {
                                        el.Click += ChessGrid_Click;
                                        el.Background = Brushes.Red;
                                    }
                                }
                                else
                                {
                                    if ((el.Content as Image).Name[1] == 'w')
                                    {
                                        el.Click += ChessGrid_Click;
                                        el.Background = Brushes.Red;
                                    }
                                }
                            }
                            else
                            {
                                el.Click += ChessGrid_Click;
                                el.Background = Brushes.Green;
                            }
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            uid = (char)(cords.Uid[0] - 1 + i) + ((int)Char.GetNumericValue(cords.Uid[1]) + 1).ToString();
                            el = GetByUid(Chess_Grid, uid) as Button;
                            if (el != null)
                            {
                                if (el.Content as string != "")
                                {
                                    if (content.Name[1] == 'w')
                                    {
                                        if ((el.Content as Image).Name[1] == 'b')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                    else
                                    {
                                        if ((el.Content as Image).Name[1] == 'w')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                else
                                {
                                    el.Click += ChessGrid_Click;
                                    el.Background = Brushes.Green;
                                }
                            }
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            uid = (char)(cords.Uid[0] - 1 + i) + ((int)Char.GetNumericValue(cords.Uid[1]) - 1).ToString();
                            el = GetByUid(Chess_Grid, uid) as Button;
                            if (el != null)
                            {
                                if (el.Content as string != "")
                                {
                                    if (content.Name[1] == 'w')
                                    {
                                        if ((el.Content as Image).Name[1] == 'b')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                    else
                                    {
                                        if ((el.Content as Image).Name[1] == 'w')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                else
                                {
                                    el.Click += ChessGrid_Click;
                                    el.Background = Brushes.Green;
                                }
                            }
                        }                            
                    break;
                    }
                case 'p':
                case 'P':
                    {
                        //white
                        if (content.Name[1] == 'w')
                        {
                            if (cords.Uid[1] == '2')
                            {
                                string uid = (char)(cords.Uid[0] + 1) + ((int)Char.GetNumericValue(cords.Uid[1]) + 1).ToString();
                                var el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if ((el.Content as Image).Name[1] == 'b')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                uid = (char)(cords.Uid[0] - 1) + ((int)Char.GetNumericValue(cords.Uid[1]) + 1).ToString();
                                el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if ((el.Content as Image).Name[1] == 'b')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                for (int i = 1; i < 3; i++)
                                {
                                    uid = cords.Uid[0] + ((int)Char.GetNumericValue(cords.Uid[1]) + i).ToString();
                                    el = GetByUid(Chess_Grid, uid) as Button;
                                    if (el != null)
                                    {
                                        if (el.Content as string == "")
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Green;
                                        }
                                    }
                                }                                
                            }
                            else
                            {                                
                                string uid = (char)(cords.Uid[0] + 1) + ((int)Char.GetNumericValue(cords.Uid[1]) + 1).ToString();
                                var el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if ((el.Content as Image).Name[1] == 'b')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                uid = (char)(cords.Uid[0] - 1) + ((int)Char.GetNumericValue(cords.Uid[1]) + 1).ToString();
                                el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if ((el.Content as Image).Name[1] == 'b')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                uid = cords.Uid[0] + ((int)Char.GetNumericValue(cords.Uid[1]) + 1).ToString();
                                el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string == "")
                                    {
                                        el.Click += ChessGrid_Click;
                                        el.Background = Brushes.Green;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (cords.Uid[1] == '7')
                            {
                                string uid = (char)(cords.Uid[0] + 1) + ((int)Char.GetNumericValue(cords.Uid[1]) - 1).ToString();
                                var el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if ((el.Content as Image).Name[1] == 'w')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                uid = (char)(cords.Uid[0] - 1) + ((int)Char.GetNumericValue(cords.Uid[1]) - 1).ToString();
                                el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if ((el.Content as Image).Name[1] == 'w')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                for (int i = 1; i < 3; i++)
                                {
                                    uid = cords.Uid[0] + ((int)Char.GetNumericValue(cords.Uid[1]) - i).ToString();
                                    el = GetByUid(Chess_Grid, uid) as Button;
                                    if (el != null)
                                    {
                                        if (el.Content as string == "")
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Green;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                string uid = (char)(cords.Uid[0] + 1) + ((int)Char.GetNumericValue(cords.Uid[1]) - 1).ToString();
                                var el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if ((el.Content as Image).Name[1] == 'w')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                uid = (char)(cords.Uid[0] - 1) + ((int)Char.GetNumericValue(cords.Uid[1]) - 1).ToString();
                                el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string != "")
                                    {
                                        if ((el.Content as Image).Name[1] == 'w')
                                        {
                                            el.Click += ChessGrid_Click;
                                            el.Background = Brushes.Red;
                                        }
                                    }
                                }
                                uid = cords.Uid[0] + ((int)Char.GetNumericValue(cords.Uid[1]) - 1).ToString();
                                el = GetByUid(Chess_Grid, uid) as Button;
                                if (el != null)
                                {
                                    if (el.Content as string == "")
                                    {
                                        el.Click += ChessGrid_Click;
                                        el.Background = Brushes.Green;
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }

        public static UIElement GetByUid(DependencyObject rootElement, string uid)
        {
            foreach (UIElement element in LogicalTreeHelper.GetChildren(rootElement).OfType<UIElement>())
            {
                if (element.Uid == uid)
                    return element;
                UIElement resultChildren = GetByUid(element, uid);
                if (resultChildren != null)
                    return resultChildren;
            }
            return null;
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
                                        local_board = res;
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
