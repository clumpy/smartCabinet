   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem   UIShell.WpfAppCenterPlugin5   /UIShell.WpfAppCenterPlugin.AppCenterUserControl   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     7core&clr-namespace:System;assembly=mscorlib     N JFirstFloor.ModernUI, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null0mui&http://firstfloorsoftware.com/ModernUI  .���    ContentRoot#��[  .+�]� .+�� $	��Top=�  � ColumnDefinitions  �� $
��Auto ��� $
��Auto ��� $
��100* ��� $
��Auto ��� $
��Auto � � RowDefinitions �� $��26 �.+��� $��0��$��过滤词：���� -   $	��Key��$��1��$	��150��� -   $��LoadBundlesProgressBar��$��2�� � Minimum$ 0Q� � Maximum$ 1Q�$��16�� � IsIndeterminate . $��0,0,0,0q��� -   $��InstallButton��$��3��$��下载安装��$	��100���� -   $��RefreshButton��$��4��$��刷新��$	��100��&   System.Windows.Controls.DataGrid  -   $��BundlesDataGrid��$y�Hidden=�   SelectionMode$ Extended=�   
IsReadOnly .    AutoGenerateColumns .     CanUserAddRows .  ���� 	   Columns	 A  ;FirstFloor.ModernUI.Windows.Controls.DataGridCheckBoxColumn  1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 ��  �� Path$ 
IsSelected� �� RelativeSource �� *FindAncestor+ �� AncestorType)  #System.Windows.Controls.DataGridRow"  =  7FirstFloor.ModernUI.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   
IsReadOnly .    Header$ 名称��
 �� *Name+   . $ 标识��
 �� *SymbolicName+   . $ 版本��
 �� *	Version+   . $ 	新版本��
 �� *NewVersionText+   . $ 更新��
 �� *
UpdateDate+ �� StringFormat$ 
yyyy-MM-dd��