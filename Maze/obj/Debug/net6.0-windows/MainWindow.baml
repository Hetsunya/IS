   M S B A M L   `   `   `  ����  Maze    Maze.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Maze Solver��5   	   $	��450��6   $	��800��6*   .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5
   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� -   5        $��
MazeCanvas��5      $��0��6#   $��0��60   
�������6@   �� 5   
   -   5        $��DebugOutput��5      $��2��6'   $��2��64   $��Center=�5      $	��Top=�6)   $��14(�6A   �� 5   
   $��0��6   $��1��6"   $��10q�62   .+�5      �� -   5        $��AllPathsButton��5   I   $ ��Найти все пути��6   $��5q�6_   �� 5      -   5        $��ShortestRouteButton��5   T   $.��(Найти кратчайший путь��6   $��5q�6o   �� 5      -   5        $��NextRouteButton��5   V   $8��2Показать следующий маршрут��6   $��5q�6m   �� 5      -   5        $��
EditButton��5   ?   $B��<Изменить конфигурацию лабиринта��6   $��5q�6h   �� 5      -   5        $��ImportButton��5      $ ��Импортировать��6)   $��5q�6\   �� 5      -   5        $��ExportButton��5      $"��Экспортировать��6)   $��5q�6]   5      �� 5"   
   -	   5        $��	RouteInfo��5"      $��1��6%   $��0��62   $��2��6B   $��Center=�5#      $��Center=�61   $��16(�6L   5$      5%      