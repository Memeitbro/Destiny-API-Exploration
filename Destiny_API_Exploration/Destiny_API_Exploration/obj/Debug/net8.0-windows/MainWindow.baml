   M S B A M L   `   `   `  ����e  aMicrosoft.Web.WebView2.Wpf, Version=1.0.2478.35, Culture=neutral, PublicKeyToken=2a8ab48044d2601ekLclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.WpfMicrosoft.Web.WebView2.Wpf  5   	   R NDestiny_API_Exploration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:Destiny_API_ExplorationDestiny_API_Exploration 5   	   (   "Destiny_API_Exploration.MainWindow   5   	   Twv2Lclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.Wpf  5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   7local%clr-namespace:Destiny_API_Exploration     5   	   -   5        $��	mainWindo��5
   	     � Title$  Evil Destiny Item Manager��5	   	   $
��1920��6+   $
��1080��68   .��5      � -   5        $
��Grid��5       � RowDefinitions 5   
   �� 5      $��75 �6   6*   �� 5      6    � ColumnDefinitions 5   
   �� 5      6!   �� 5      6!   �� 5      6!   �� 5      6!   �� 5      6!   .+�5   
   �� -   5        $��LoginButton��5      $	��100��6$   $��50��60   $��Center=�5      $��Center=�6,   $��2��5      $��5��60   .��6D   	Authorize5      )   #Microsoft.Web.WebView2.Wpf.WebView2  5   
   -   5        $��webView��5         Source$ https://www.bungie.net/?�5      $y�Hidden=�5      $��2��5      $��5��6(   �� 5!   
   -   5        $��	charName1��5!      $��0��6!   $��0��6.   $y�Hidden=�6>   $��Unknown Character��5"      $��Center=�6,   $��Center=�6G   6f   �� 5#   
   -   5        $��	charName2��5#      $��0��6!   $��1��6.   $y�Hidden=�6>   $��Unknown Character��5$      $��Center=�6-   $��Center=�6H   6g   �� 5%   
   -   5        $��	charName3��5%      $��0��6!   $��2��6.   $y�Hidden=�6>   $��Unknown Character��5&      $��Center=�6-   $��Center=�6H   6g   �� 5'   
   -   5        $��	vaultName��5'      $��0��6!   $��3��6.   $y�Hidden=�6>   $��	The Vault��5(      $��Center=�6%   $��Center=�6@   6_   �� 5)   
   -	   5        $��SelectedItem��5)      $��0��6$   $��4��61   $y�Hidden=�6A   $��No Item Selected��5*      $��Center=�6,   $��Center=�6G   6f   �� 5-   
   -
   5        $��
character1��5-      $��1��6%   $��0��62   $y�Hidden=�6B    �� SelectionMode$ Single=�5.      6V   �� 5/   
   -   5        $��
character2��5/      $��1��6%   $��1��62   $y�Hidden=�6B   $ Single=�50      6V   �� 51   
   -   5        $��
character3��51      $��1��6%   $��2��62   $y�Hidden=�6B   $ Single=�52      6V   �� 53   
   -   5        $��vault��53      $��1��6    $��3��6-   $y�Hidden=�6=   $ Single=�54      6Q   �� 56   
   -   5        $��char1Transfer��56      $��1��6&   $��4��63   $��50��6C   $	��150��6O   $	��Top=�57      $
��Left=�6)   $y�Hidden=�6D   $��No Character��58      6;   �� 59   
   -   5        $��
char1Equip��59      $��1��6#   $��4��60   $��50��6@   $	��150��6L   $	��Top=�5:      $��Right=�6)   $y�Hidden=�6E   $��No Character��5;      68   �� 5<   
   -   5        $��char2Transfer��5<      $��1��6&   $��4��63   $��50��6C   $	��150��6O   $��Center=�5=      $
��Left=�6,   $y�Hidden=�6G   $��No Character��5>      6;   �� 5?   
   -   5        $��
char2Equip��5?      $��1��6#   $��4��60   $��50��6@   $	��150��6L   $��Center=�5@      $��Right=�6,   $y�Hidden=�6H   $��No Character��5A      68   �� 5B   
   -   5        $��char3Transfer��5B      $��1��6&   $��4��63   $��50��6C   $	��150��6O   $��Bottom=�5C      $
��Left=�6,   $y�Hidden=�6G   $��No Character��5D      6;   �� 5E   
   -   5        $��
char3Equip��5E      $��1��6#   $��4��60   $��50��6@   $	��150��6L   $��Bottom=�5F      $��Right=�6,   $y�Hidden=�6H   $��No Character��5G      68   �� 5I   
   -   5        $��LogOut��5I      $��1��6   $��4��6,   $��50��6<   $	��150��6H   $��Center=�5J      $��Center=�6,   $y�Hidden=�6I   $��	0,250,0,0q�5K      $��Log Out��6$   5L   *   �� 5M   
   -   5        $��ToVault��5M      $��1��6    $��4��6-   $��50��6=   $	��150��6I   $��Center=�5N      $��Center=�6,   $y�Hidden=�6I   $��	0,0,0,250q�5O      $��Banish to Vault��6$   5P   $   �� 5S   
   -   5        $��ShowRotators��5S      $��1��6%   $��4��62   $��50��6B   $	��150��6N   $��Center=�5T      $��Center=�6,   $y�Visible=�6I   $��	0,500,0,0q�5U      $��Show Rotators��6$   5V   )   �� 5W   
   -   5        $��HideRotators��5W      $��1��6%   $��4��62   $��50��6B   $	��150��6N   $��Center=�5X      $��Center=�6,   $y�Hidden=�6I   $��	0,0,0,500q�5Y      $��Back to Inventories��6$   5Z   )   �� 5\   
   -   5        $��	RaidLabel��5\      $��0��6!   $��0��6.   $y�Hidden=�6>   $"��Featured Raid for this week!��5]      $��Center=�67   $��Center=�6R   6q   �� 5^   
   -   5        $��FeaturedRaid��5^      $��1��6$   $��0��61   $y�Hidden=�6A   $��Unknown to me yet!��5_      $��Center=�61   $��Center=�6L   6k   �� 5a   
   -   5        $��DungeonLabel��5a      $��0��6$   $��1��61   $y�Hidden=�6A   $%��Featured Dungeon for this week!��5b      $��Center=�6:   $��Center=�6U   6t   �� 5c   
   -   5        $��FeaturedDungeon��5c      $��1��6'   $��1��64   $y�Hidden=�6D   $��Unknown to me yet!��5d      $��Center=�6-   $��Center=�6H   6g   5e      5f      