   M S B A M L   `   `   `  ����   KinectWPFOpenCV     KinectWPFOpenCV.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  
MainWindow��$	��850��$	��575�� � WindowStyle$
 None=� � AllowsTransparency . .���� ���#00bcf2$��1q�._��  � RowDefinitions �� $��90 ��� $��* � � ColumnDefinitions �� �� $	��35* ��� $
��auto �.+��� -      SimpleButton#��[  $��2��$��Right=�$��1��$	��Top=�$��0 10 q�.���� $	����  SegoeSymbol#��[ $��12(����#999�� $��Images\Status.png��$
��None=�$
��Left=�$��1���� -    ��Name$ title��$��Kinect + OpenCV ��$
��Left=�$��
65,26,0,26q�$��Center=�  KinectFontLight#��[   TitleForegroundBrush#��[   HeaderFontSize#��[ $��1���� ���#666$��1��$��
15 0 10 15q�$��2���� -   $ txtError��$��1��$��No Kinect Found��$y�	Collapsed=�#��[ ���#ccc$��21(�$��Center=�$��Center=�$��2��(� -    (�Name$ outputViewbox��$��
15 0 10 15q�$��1�� (� Stretch$ Uniform=�$��2��.���� -   $��outImg��$	��640��$	��480�� J� RenderTransformOrigin$ 0.5,0.5&��� -   $��colorImg��$	��160��$	��120��$��1��$��2��$��Uniform=�$ 0.5,0.5&�$��0 2 12 0q�$��Right=�$	��Top=��� $��	0 0 15 15q�$	��300�����#f0f0f0$��Right=�$��2��$��1��.+��� $��Settings��	 �� Padding$	 10q����#ccc�� $��Min Distance��$	 10q��� 
 �� Orientation$
 
Horizontal=�.+��� -    ��Name$ 	sliderMin��$	��250�� � Minimum$	 300Q� � Value$	 600Q� � Maximum$	 900Q� �� TickFrequency$ 1Q� �� IsSnapToTickEnabled .  �� Interval$ 1��$��5q�$��10q��� $��Center=����� *Value+ �� ElementName$ 	sliderMin���� $��Max Distance��$	 10q��� $
 
Horizontal=�.+��� -   $ 	sliderMax��$	��250��$	 900Q�$	 900Q�$
 6000Q�$��5q�$��10q��� $��Center=����� *Value+$ 	sliderMax���� $��Min Blob Size��$	 10q��� $
 
Horizontal=�.+��� -	   $ sliderMinSize��$	��250��$ 1Q�$ 10Q�$ 50Q�$��5q�$ 1Q� . $ 1��$��10q��� $��Center=����� *Value+$ sliderMinSize���� $��Max Blob Size��$	 10q��� $
 
Horizontal=�.+��� -
   $ sliderMaxSize��$	��250��$ 1Q�$	 110Q�$	 500Q�$��5q�$ 1Q� . $ 1��$��10q��� $��Center=����� *Value+$ sliderMaxSize���� ���#ccc$
��Info��$��0,20,0,0q�$	 10q��� $
 
Horizontal=�$��0 10 0 0q�.+��� $��Blobs Detected��$��10 0 0 0q��� -   $ txtBlobCount��$
��Wrap=�$��0��$��10 0 0 0q��� -   $ txtInfo��$
��Wrap=�$�� ��$	 10q�