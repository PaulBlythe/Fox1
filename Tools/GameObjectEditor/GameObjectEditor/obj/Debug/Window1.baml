   M S B A M L   `   `   `  ����K  GGameObjectEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:GameObjectEditorGameObjectEditor  5   	       GameObjectEditor.Window1   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:GameObjectEditor     5   	     � Title$  Filter game components��5   	   $	��800��5	   	   $
��1200��6    � WindowStyle$
 None=�5
   	    � WindowState$ 	Maximized=�5   	    � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5       �� 
TargetType ��Name �� Padding �� EndPoint	 �� 
StartPoint
 � Color � Offset � ColumnDefinitions z� SharedSizeGroup �� Triggers P� Property P� Value �� 
TargetName �� Property �� Value �� 
TargetType��%�  (��       5      *!MenuItem.SubmenuItemTemplateKey+)(�� B    5,   '   *MenuItem.SeparatorStyleKey+)�� 5   
   " m�6M   .R�5      �� $ _grid��6   $ 1q�6$   $��1q�60   
���   ���5      �� 5      $	 0,1&�6*   $		 0,0&�69   .��5      � $
 	#FF3A60AD��6'   $ 0.528Q�69   � 5      $
 	#FF6A85D8��6'   $	 0.2Q�69   � 5      $
 	#FF173262��6'   $ 1Q�69   5      ._�5      �  5      �� 5      $
��Auto �6+   $��24��68   $ 	IconGroup��6F   �� 5      $
��Auto �6+   $��30��68   �� 5      $��20 �6+   .+�5      �� $
��Icon��6'   #��z��6<   �� 5      $��1��6'   $��Center=�67   $��Header��6R   ��. 6i   5      5        5"      P� 5#       �@m�IsHighlighted . . �5%      �� $ _grid��6A    � ��
 � � ��� 5&      $ _grid��6B    � ��
 �   ��� 5'       � ��
 �����63   5(      5*      �� 5,   
   " ��6   .�5-      ��  �@�OverridesDefaultStyle . 66   �� 5.       � �� 5/      �� 50      " ��6&   .R�51      �� 
����  �$��1��63   52      53      55      56      58      .��59      � ��5:   
   �� 5;      $	 0,1&�6"   $		 0,0&�61   .��5<      � $
 CornflowerBlue��6   $	 0.0Q�66   � 5=      $
 White��6   $ 1Q�6-   5>       � RowDefinitions 5@   
   �� 5A      $��30 �6   �� 5B      6   .+�5E   
   o�  o� 
IsMainMenu . $��	10,5,10,0q�6!   $��16(�64   
�������$��0��6V    J� BitmapEffect 5F      F� 5G      ��5I      �� 5J      $	 0,1&�6&   $		 0,0&�65   .��5K      � $
 	#FF3A60AD��6#   $ 0.528Q�65   � 5L      $
 	#FF6A85D8��6#   $	 0.2Q�65   � 5M      $
 	#FF173262��6#   $ 1Q�65   5N      .@�5Q      m� -   5        $��All on��5Q      6'   m� 5R      -   5        $��All off��5R      6(   m� 5S      -   5        $��Cancel��5S      6'   m� 5T      -   5        $��_Exit��5T      6&   5U      � 5W   
   -   5         �Name$ Groups��5W      $��1��6$   5Y      5Z      5[      