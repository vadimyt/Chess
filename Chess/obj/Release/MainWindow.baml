   M S B A M L   `   `   `  ���� @  <Chess, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:ChessChess      Chess.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     %localclr-namespace:Chess       � Title$  
MainWindow��$	��450��$	��800��$	��450��$	��800��.��� .+�� -    �Name$ StartButtons��.+��� -   $��220,0,440,0q�$	��120��$��60��$��Center=�$��Center=�.���� $��Login��$��30(��� -   $��440,0,220,0q�$	��120��$��60��$��Center=�$��Center=�.���� $��Register��$��30(�� -   $ LoginPassword��.+��� $��Login��$��30(�$	��100��$��60��$��Center=�$��Center=�$��0,0,100,250q��� -    ��Name$ Login��$��30(�$	��100��$��60��$��Center=�$��Center=�$��100,0,0,250q��� $��Password��$��30(�$	��140��$��60��$��Center=�$��Center=�$��0,0,140,100q�G� -    G�Name$ PasswordHidden��$��30(�$	��100��$��60��$��Center=�$��Center=� G� PasswordChar$	 •��$��100,0,0,100q��� -   $ PasswordUnmask��$y�Hidden=�$��30(�$	��100��$��60��$��Center=�$��Center=�$��100,0,0,100q��� -    ��Name$ ShowPassword��$��20��$��20��$��show_hide/hide.png�� �� BitmapScalingMode$ NearestNeighbor=� �� EdgeMode$ Aliased=�$��300,0,0,100q��� -	    ��Name$ 	LoginSend��$��100,100,0,0q�$	��120��$��60��$��Center=�$��Center=�.���� $��Login��$��30(��� -
   $ RegisterSend��$��100,100,0,0q�$	��120��$��60��$��Center=�$��Center=�.���� $��Register��$��30(��� -   $
 Back��$��0,100,160,0q�$	��120��$��60��$��Center=�$��Center=�.���� $
��Back��$��30(�� -   $ Lobby��	 � ColumnDefinitions	 �� $	��150 ��� �� $	��150 �.+��� -   
 ��Name$
 Lobbies��$��1���� -   $ RefreshSend��$��
10,100,0,0q�$	��120��$��60��$
��Left=�$	��Top=�.���� $��Refresh��$��30(��� -   $ 
DeleteSend��$��	10,10,0,0q�$	��120��$��60��$
��Left=�$��Center=�.���� $��Delete��$��30(��� -   $ 
CreateSend��$��0��$��	10,10,0,0q�$	��120��$��60��$
��Left=�$	��Top=�.���� $��Create��$��30(��� -   $ QuitSend��$��0��$��
10,350,0,0q�$	��120��$��60��$
��Left=�$	��Top=�.���� $
��Quit��$��30(�� -   $
 Game��	 �� $	��100 ��� �� $	��100 � � RowDefinitions �� $��50 ��� �� $��50 �.+��� -    ��Name$
 Wait��$��1��$��1��$	��200��$��80��$��20(�� -   $ 
Enemy_info��$��0��$��1��.+��� -   $ Enemy_login��$	��200��$��50��$��20(�$��0,10,0,0q��� $��Ваш оппонент:��$	��150��$��50��$��20(�$��
0,10,400,0q��� -   $ 
Enemy_wins��$��60��$��50��$��20(�$��
480,10,0,0q��� $��победы:��$	��150��$��50��$��20(�$��
400,10,0,0q��� -   $��1��$��0��$��50��$��90��.���� $��Leave��$��20(��� -   $ Restart��$��1��$��2��$��50��$��90��.���� $��Restart��$��20(�� -   $ 
Chess_Grid��$��1��$��1��	 �� �� �� �� �� �� �� ��  �� �� �� �� �� �� �� �� 