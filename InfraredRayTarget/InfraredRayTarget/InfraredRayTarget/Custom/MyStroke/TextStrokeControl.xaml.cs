using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InfraredRayTarget.Custom.MyStroke
{
    /// <summary>
    /// TextStrokeControl.xaml 的交互逻辑
    /// </summary>
    public partial class TextStrokeControl : UserControl
    {
        public TextStrokeControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty OriginPointProperty =
                                        DependencyProperty.Register("Origin", typeof(Point), typeof(TextStrokeControl),
                                                new FrameworkPropertyMetadata(new Point(0, 0),
                                                        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure,
                                                             OnPropertyChanged));
        [Bindable(true), Category("Appearance")]
        [TypeConverter(typeof(PointConverter))]
        public Point Origin { get { return (Point)GetValue(OriginPointProperty); } set { SetValue(OriginPointProperty, value); } }


        public static readonly DependencyProperty TextProperty =
                                 DependencyProperty.Register("Text", typeof(string), typeof(TextStrokeControl), new PropertyMetadata("", OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        [Bindable(true), Category("Appearance")]
        public string Text { get { return (string)GetValue(TextProperty); } set { SetValue(TextProperty, value); } }

        public static readonly DependencyProperty StrokeThicknessProperty =
                         DependencyProperty.Register("StrokeThickness", typeof(double), typeof(TextStrokeControl), new PropertyMetadata((double)0, OnPropertyChanged));

        [Bindable(true), Category("Appearance")]
        public double StrokeThickness { get { return (double)GetValue(StrokeThicknessProperty); } set { SetValue(StrokeThicknessProperty, value); } }

        public static readonly DependencyProperty ShapeStyleProperty =
                 DependencyProperty.Register("ShapeStyle", typeof(double), typeof(TextStrokeControl), new PropertyMetadata(null));

        /// <summary>
        /// 用于描边的特殊设置
        /// 类型是Shape
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public Style ShapeStyle { get { return (Style)GetValue(StrokeThicknessProperty); } set { SetValue(StrokeThicknessProperty, value); } }
    }
}
