   M S B A M L   `   `   `  ����I  EDisassemblerUI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:DisassemblerUIDisassemblerUI  5   	   0    *DisassemblerUI.Views.DisassemblerShellView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   .localclr-namespace:DisassemblerUI     5   	   $��18(�5      $��Segoe UI Light)�6(     � Title$  MyChip8 Disassembler��5   	   $	��450��6&   $	��800��63   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $��10 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $��10 �6    � RowDefinitions 5   
   �� 5      $��10 �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $��10 �6   .+�5   
   �� $��1��6   $��1��6$   $��0,0,0,0q�61   $��File:��6B   �� 5   
   -   5        $��FileName��5      $��2��6"   $��1��62   $��	10,0,10,0q�6?   $	��100��6R   �� 5    
   -   5        $��OnLoadFileButton��5       $��3��6)   $��1��69   $
��Load��6G   $��50��6V   6c   �� 5#   
   $��1��6   $��2��6$   
Show Hex6;   �� 5$   
   -   5        $��OnShowHexCheckBox��5$      $��2��6,   $��2��6<   $��Center=�6I   $��Center=�6f   6�   &   System.Windows.Controls.DataGrid  5%   
   -   5          Name$ ProgramInstructions��5%      $��5��60   $��1��6=   $��5��6M   "   VerticalScrollBarVisibility$
 Auto=�6a   $   HorizontalScrollBarVisibility$
 Auto=�6�      AutoGenerateColumns . 6�   5'      5(      