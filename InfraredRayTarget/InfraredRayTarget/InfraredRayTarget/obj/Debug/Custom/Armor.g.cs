﻿#pragma checksum "..\..\..\Custom\Armor.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "A4A4373BC5FDE7604F8610B1B41A14CFA16AF3047262BB67F97F2DAF3ABFFD5F"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using InfraredRayTarget.Custom;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace InfraredRayTarget.Custom {
    
    
    /// <summary>
    /// Armor
    /// </summary>
    public partial class Armor : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\Custom\Armor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid smallArmor;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\Custom\Armor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_smallLeft;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\Custom\Armor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_smallRight;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\Custom\Armor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_smallArmor;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\Custom\Armor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid bigArmor;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\Custom\Armor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_bigLeft;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\Custom\Armor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_bigRight;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\Custom\Armor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btn_bigArmor;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\Custom\Armor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label colorLabel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/InfraredRayTarget;component/custom/armor.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Custom\Armor.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\Custom\Armor.xaml"
            ((InfraredRayTarget.Custom.Armor)(target)).Loaded += new System.Windows.RoutedEventHandler(this.OnLoaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.smallArmor = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            
            #line 12 "..\..\..\Custom\Armor.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnChangeArmor);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btn_smallLeft = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\..\Custom\Armor.xaml"
            this.btn_smallLeft.Click += new System.Windows.RoutedEventHandler(this.OnChangeColor);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btn_smallRight = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\..\Custom\Armor.xaml"
            this.btn_smallRight.Click += new System.Windows.RoutedEventHandler(this.OnChangeColorMore);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btn_smallArmor = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\..\Custom\Armor.xaml"
            this.btn_smallArmor.Click += new System.Windows.RoutedEventHandler(this.OnChangeNumber);
            
            #line default
            #line hidden
            return;
            case 7:
            this.bigArmor = ((System.Windows.Controls.Grid)(target));
            return;
            case 8:
            
            #line 19 "..\..\..\Custom\Armor.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OnChangeArmor);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btn_bigLeft = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\..\Custom\Armor.xaml"
            this.btn_bigLeft.Click += new System.Windows.RoutedEventHandler(this.OnChangeColor);
            
            #line default
            #line hidden
            return;
            case 10:
            this.btn_bigRight = ((System.Windows.Controls.Button)(target));
            
            #line 21 "..\..\..\Custom\Armor.xaml"
            this.btn_bigRight.Click += new System.Windows.RoutedEventHandler(this.OnChangeColorMore);
            
            #line default
            #line hidden
            return;
            case 11:
            this.btn_bigArmor = ((System.Windows.Controls.Button)(target));
            
            #line 23 "..\..\..\Custom\Armor.xaml"
            this.btn_bigArmor.Click += new System.Windows.RoutedEventHandler(this.OnChangeNumber);
            
            #line default
            #line hidden
            return;
            case 12:
            this.colorLabel = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
