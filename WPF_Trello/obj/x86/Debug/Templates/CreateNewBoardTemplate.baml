   M S B A M L   `   `   `  ����E  AWPF_Trello, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:WPF_Trello.TemplatesWPF_Trello.Templates  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Z VDevExpress.Mvvm.UI, Version=19.2.0.0, Culture=neutral, PublicKeyToken=382ad28f6ae31f30W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089U QMaterialDesignThemes.Wpf, Version=3.0.1.920, Culture=neutral, PublicKeyToken=null   CreateNewBoardTemplate  �Name �� Color �� Opacity �� Orientation �� CornerRadius �� Padding)   #MaterialDesignThemes.Wpf.HintAssist    Hint �� Mode �� UpdateSourceTrigger  MaterialDesignFlatButton	 � ToolTip'  !MaterialDesignThemes.Wpf.PackIcon
   Kind j� Stretch �� ImageSource+  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth �� SelectedItem � Orientation K� IsItemsHost � Cursor �� 
TargetType �� Property # RandomPictureCollectionTemplate �� Value �� Triggers �� Value �� Binding �� 
TargetName�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      FmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      ,local"clr-namespace:WPF_Trello.Templates  5      %}
  &	        7  7  7  �� 5      .R�5   
   � $  AddNewBoardContainer��6   ��5      �� 5      $ Black��6"   $	 0.7Q�60   .+�5
      �� $��Center=�6   $��60q�66   $ 
Horizontal=�6B   .+�5      �� .+�5      �� $ 3��6   $	��296��6.   $��96��6:   ���#838c91._�5      �� $ 3��6!   $ 12 8 4 8q�62   ._�5      � .+�5   "   �� $	��236��6)   $	 8 0q�65   $
��Left=�5   %   $	��Top=�6@   $ 3��5   %   $��2q�66   ��5   &   �� 5   *   $ White��6:   $	 0.2Q�6H   ._�5   &   �� 
�������$ Add board title��5   )   $
��Bold"�5   )   $��16(�5   )   $��0q�5   )   
������ ��5   )   �� *NewBoardTitle+$ TwoWay=�$ PropertyChanged=�5   )   5   #   �� 5   "   $��26��5   %   $��26��5    %   $��0q�5!   %   $��0 0 5 0q�5"   %   $	��Top=�5#   %   $��Right=�5$   %   8��  $	 MaterialDesignFlatButton��5'   %   ��5&   %   �� *HideCreateNewBoardCommand+.��5(   &     $
 Close=�5)   )   $��24��5*   )   
�������$��24��5,   )   5-   #   �� 5.   "   $	 4 0q�6)   $
��Left=�5/   %   $��Bottom=�6@   $ 3��50   %   $��2q�66   ��51   &   �� 52   *   $ White��6:   $	 0.3Q�6H   ._�54   &   �� $ Img URL��55   )   
�������$��DemiBold"�58   )   $��0q�59   )   $��14(�5:   )   
������ ��56   )   �� *BoardBackgrounPicture+$ TwoWay=�$ PropertyChanged=�5;   )   5<   #   5=      ��5>      �� 5?   "   $ UniformToFill=�6�    6-   �� *BoardBackgrounPicture+$ PropertyChanged=�6�   5A      5B      �� 5C      $
��Left=�6   $	��115��69   ���#5AAC44
������ $ Depth0=�5E      $��0 15 0 0q�5G      ��5F      �� *AddNewBoardCommand+.��5G   +   Create Board5I      5J      �� 5L      .+�5M      �� $	��120��5O      $��3 -3 0 0q�6-   ��5M      �� *RandomPictureCollection+ 5N      �� *SelectedPicture+��5P      �� 5Q      .R�5R   "   � $ 
Horizontal=�6,    . $
 Hand��5S   ?   5T      ��5V      �� 5W      " ��6$   .�5X   "   ��  � ��8  6=   5Y      5[      �� 5\      $��26��5]      $��26��5^      $��0q�5_      $��0 10 3 0q�5`      $	��Top=�5a      $��Right=�5b      8�� $	 MaterialDesignFlatButton��5e      ��5d      �� *RefreshPictureCommand+.��5f        $
 Refresh=�5g      $��24��5h      
�������$��24��5j      5k      5l      5m      5n       5o   
   �� 5p      $ false��6e    6   �� *IsAddBoardTrigger+$ PropertyChanged=�.a�5q      �� $ AddNewBoardContainer��6    � y�$ 	Collapsed=�6Q   5r      5t      5u      