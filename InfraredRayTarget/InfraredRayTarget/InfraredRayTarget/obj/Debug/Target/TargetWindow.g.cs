﻿#pragma checksum "..\..\..\Target\TargetWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "73ADBEA79F455E61628B86584D6C43665332C290CC7163AA4AB3FABDE17D0700"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using InfraredRayTarget;
using InfraredRayTarget.Custom;
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


namespace InfraredRayTarget {
    
    
    /// <summary>
    /// TargetWindow
    /// </summary>
    public partial class TargetWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\Target\TargetWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_bg;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\Target\TargetWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_axis;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\..\Target\TargetWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_ring;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\Target\TargetWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_armor;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\Target\TargetWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal InfraredRayTarget.Custom.BigArmorEntity armor_big;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Target\TargetWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal InfraredRayTarget.Custom.SmallArmorEntity armor_small;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\Target\TargetWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_custom;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Target\TargetWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_bullet;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\Target\TargetWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_minicircle;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\Target\TargetWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_avecircle;
        
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
            System.Uri resourceLocater = new System.Uri("/InfraredRayTarget;component/target/targetwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Target\TargetWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            
            #line 8 "..\..\..\Target\TargetWindow.xaml"
            ((InfraredRayTarget.TargetWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.OnLoaded);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\Target\TargetWindow.xaml"
            ((InfraredRayTarget.TargetWindow)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.OnMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\Target\TargetWindow.xaml"
            ((InfraredRayTarget.TargetWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.OnClosing);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\Target\TargetWindow.xaml"
            ((InfraredRayTarget.TargetWindow)(target)).MouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.OnMouseWheel);
            
            #line default
            #line hidden
            return;
            case 2:
            this.canvas_bg = ((System.Windows.Controls.Canvas)(target));
            return;
            case 3:
            this.canvas_axis = ((System.Windows.Controls.Canvas)(target));
            return;
            case 4:
            this.canvas_ring = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.canvas_armor = ((System.Windows.Controls.Canvas)(target));
            return;
            case 6:
            this.armor_big = ((InfraredRayTarget.Custom.BigArmorEntity)(target));
            return;
            case 7:
            this.armor_small = ((InfraredRayTarget.Custom.SmallArmorEntity)(target));
            return;
            case 8:
            this.canvas_custom = ((System.Windows.Controls.Canvas)(target));
            return;
            case 9:
            this.canvas_bullet = ((System.Windows.Controls.Canvas)(target));
            return;
            case 10:
            this.canvas_minicircle = ((System.Windows.Controls.Canvas)(target));
            return;
            case 11:
            this.canvas_avecircle = ((System.Windows.Controls.Canvas)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

