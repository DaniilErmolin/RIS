   M S B A M L   `   `   `  ���� 
  Client    Client.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Chat Client��$	��450��$	��746�� � 
ResizeMode$ CanMinimize=�.��� $��10q�.+�� -    �Name$ ConnectionPanel��.+���  �� Orientation$ 
Horizontal=�$	��Top=�$��0,0,0,10q�.+��� $��
Server IP:��$��Center=�$��0,0,5,0q��� -    ��Name$ ServerIP��$	��150��$��	127.0.0.1���� $��Port:��$��Center=�$��10,0,5,0q��� -   $ 
ServerPort��$��50��$
��8181���� -   $��Connect��$��10,0,0,0q�� -   $ 	AuthPanel��$y�	Collapsed=�.+��� $ Vertical=�$��Center=�$��Center=�.+��� $��	Username:��$��0,0,0,5q��� -   $ AuthUsername��$	��200��$��0,0,0,10q��� $��	Password:��$��0,0,0,5q�G� -    G�Name$ AuthPassword��$	��200��$��0,0,0,10q��� $ 
Horizontal=�$��Center=�.+��� -   $��Login��$��5q��� -	   $��Register��$��5q��� -
   $��Back to Connect��$��10,0,0,0q�� -   $ 	ChatPanel��$y�	Collapsed=�.+��� $ 
Horizontal=�$��Right=�$��0,0,0,10q�$	��Top=�.+��� -   $��History Viewer��$	��120���� -   $��Logout��$	��120���� -   $ ChatBox�� �� 
IsReadOnly$
 True��" �� VerticalScrollBarVisibility$
 Auto=�$ �� HorizontalScrollBarVisibility$
 Auto=�	 �� TextWrapping$
	 Wrap=�$��	0,50,0,35q��� -   
 ��Name$
 MessagePanel��$ 
Horizontal=�$��Bottom=�$��Stretch=�.+��� -   $ MessageInput��$	��600��$��30��$��0,0,10,0q��� -   $
��Send��$	��100��$��30��� -   $ HistoryPanel��$y�	Collapsed=�.+��� $��Center=�$��Center=�.+��� $��Enter username:��$��0,0,0,5q��� -   $ HistoryUsername��$	��200��$��0,0,0,10q��� -   $��View History��$��0,0,0,10q��� -   $ 
HistoryBox��$
 True��$
 Auto=�$
 Auto=�$
	 Wrap=�$	��200��$	��400���� -   $��Back to Chat��$��10,0,0,0q�