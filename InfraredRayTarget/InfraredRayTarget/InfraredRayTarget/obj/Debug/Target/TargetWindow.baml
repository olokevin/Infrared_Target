   M S B A M L   `   `   `  ����L  HInfraredRayTarget, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:InfraredRayTargetInfraredRayTarget  5   	   C&clr-namespace:InfraredRayTarget.CustomInfraredRayTarget.Custom  5   	   $    InfraredRayTarget.TargetWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:InfraredRayTarget     5   	   9custom&clr-namespace:InfraredRayTarget.Custom     5   	   -   5          � WindowStyle$
  None=�5       � AllowsTransparency .  � Left$ 0��6G   
 � Top$ 0��6P    � Title$ TargetWindow��5
   	   $
��1080��6   $
��1920��6,   ��5      �� 5   
    �� Opacity$ 0Q�6   .��5      � .+�5   
   �� -   5        $��	canvas_bg��5      ���	#FFDEDEDE$��Stretch=�69   $��Stretch=�6W   .+�5      �� $
��1918��6   $
��1078��6"   $��1q�60   
���  �� �� Left$ 1��6V   
 �� Top$ 1��6f    �� Right$ 1��6u   	 �� Bottom$	 1��6�   5      �� 5   
   -   5        $��canvas_axis��5      $��Stretch=�6$   $��Stretch=�6B   �� 5   
   -   5        $��canvas_ring��5      $��Stretch=�6$   $��Stretch=�6B   $y�Visible=�6^   �� 5   
   -   5        $��canvas_armor��5      $��Stretch=�6%   $��Stretch=�6C   .+�5      -   'InfraredRayTarget.Custom.BigArmorEntity  -   5        
  Name$
 	armor_big��5   $   -   'InfraredRayTarget.Custom.BigArmorEntity   MouseLeftButtonDown$ OnHandleMoveArmorBegin��6p      MouseLeftButtonUp$ OnHandleMoveArmorEnd��6�      	MouseMove$ OnHandleMoveArmor��6�      
MouseEnter$ OnMouseEnterArmor��6�      
MouseLeave$ OnMouseLeaveArmor��6  $	 529��67   $ 303.5��6I   $y�Hidden=�6\   6  /   )InfraredRayTarget.Custom.SmallArmorEntity  5      -   5          Name$ armor_small��5   &   /   )InfraredRayTarget.Custom.SmallArmorEntity   MouseLeftButtonDown$ OnHandleMoveArmorBegin��6r      MouseLeftButtonUp$ OnHandleMoveArmorEnd��6�      	MouseMove$ OnHandleMoveArmor��6�      
MouseEnter$ OnMouseEnterArmor��6�      
MouseLeave$ OnMouseLeaveArmor��6  $	 690��6;   $	 300��6M   $y�Hidden=�6^   $	��458��6$  $	��517��61  5      �� 5!   
   -   5        $��canvas_custom��5!      $��Stretch=�6&   $��Stretch=�6D   �� 5$   
   -	   5        $��canvas_bullet��5$      $��Stretch=�6&   $��Stretch=�6D   �� 5'   
   -
   5        $��canvas_minicircle��5'      $��Stretch=�6*   $��Stretch=�6H   �� 5*   
   -   5        $��canvas_avecircle��5*      $��Stretch=�6)   $��Stretch=�6G   5+      5,      