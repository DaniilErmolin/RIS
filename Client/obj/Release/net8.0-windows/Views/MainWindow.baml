   M S B A M L   `   `   `  ���� 
  Client    Client.Views.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Chat Client��$	��450��$	��746�� � 
ResizeMode$ CanMinimize=�.��� $��10q�.+�� -    �Name$ ConnectionPanel��$y�Visible=�.+���  �� Orientation$ Vertical=�$��Center=�$��283,0,283,0q�$��96��.+��� $��
Server IP:��$��Center=�$��0,0,0,0q��� -    ��Name$ ServerIP��$	��109��$��	127.0.0.1��$
��Left=��� $��Port:��$��Center=�$��0,0,0,0q��� -   $ 
ServerPort��$��50��$
��8080��$
��Left=��� -   $��Подключиться��$��0,0,0,0q�� -   $ 	AuthPanel��$y�	Collapsed=�.+��� $ Vertical=�$��Center=�$��Center=�.+��� $��Логин:��$��0,0,0,5q��� -   $ AuthUsername��$	��200��$��0,0,0,10q��� $��Пароль:��$��0,0,0,5q�G� -    G�Name$ AuthPassword��$	��200��$��0,0,0,10q��� $ 
Horizontal=�$��Center=�.+��� -   $��Вход��$��5q��� -	   $��Регистрация��$��5q��� -
   $8��2Переподключиться к серверу��$��10,0,0,0q�� -   $ 	ChatPanel��$y�	Collapsed=�.+��� $ Vertical=�$��Right=�$��0,0,0,10q�$	��Top=�.+��� -   $+��%Просмотреть историю��$	��140���� -   $&�� Выйти из аккаунта��$	��140���� -    ��Name$ ChatBox�� �� 
IsReadOnly$
 True��" �� VerticalScrollBarVisibility$
 Auto=�$	 �� HorizontalScrollBarVisibility$
	 Auto=�$��	0,50,0,35q��� -   
 ��Name$
 MessagePanel��$ 
Horizontal=�$��Bottom=�$��Stretch=�.+��� -   $ MessageInput��$	��596��$��30��$��0,0,10,0q� �� 	MaxLength$
 2000���� -   $��Отправить��$	��100��$��30��� -   $ HistoryPanel��$y�	Collapsed=�.+��� $��Center=�$��Center=�$ 
Horizontal=�.+��� $ Vertical=�.+��� $��Логин:��$��75���� -   $ HistoryUsername��$��72��$��19���� -   $"��Вернуться в чат��$��0,10,0,0q��� $ Vertical=�.+��� -   $>��8Просмотреть историю сообщений��$��
10,0,10,10q��� -   $ 
HistoryBox��$
 True��$
 Auto=�$
	 Auto=� �� TextWrapping$
 Wrap=�$	��200��$	��200��