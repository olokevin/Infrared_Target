﻿#pragma checksum "..\..\..\Custom\SmallArmorEntity.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9A4D08D34811A49A04986A4C6605EC2851D086C82FEBD8C36B4D96FE034D4AA3"
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
using InfraredRayTarget.Custom.MyStroke;
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
    /// SmallArmorEntity
    /// </summary>
    public partial class SmallArmorEntity : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\..\Custom\SmallArmorEntity.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Path leftLight;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\Custom\SmallArmorEntity.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Path rightLight;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\Custom\SmallArmorEntity.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal InfraredRayTarget.Custom.MyStroke.TextStrokeControl number;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\Custom\SmallArmorEntity.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_bullet;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\Custom\SmallArmorEntity.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_minicircle;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Custom\SmallArmorEntity.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas canvas_avecircle;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\Custom\SmallArmorEntity.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border border;
        
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
            System.Uri resourceLocater = new System.Uri("/InfraredRayTarget;component/custom/smallarmorentity.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Custom\SmallArmorEntity.xaml"
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
            this.leftLight = ((System.Windows.Shapes.Path)(target));
            return;
            case 2:
            this.rightLight = ((System.Windows.Shapes.Path)(target));
            return;
            case 3:
            this.number = ((InfraredRayTarget.Custom.MyStroke.TextStrokeControl)(target));
            return;
            case 4:
            this.canvas_bullet = ((System.Windows.Controls.Canvas)(target));
            return;
            case 5:
            this.canvas_minicircle = ((System.Windows.Controls.Canvas)(target));
            return;
            case 6:
            this.canvas_avecircle = ((System.Windows.Controls.Canvas)(target));
            return;
            case 7:
            this.border = ((System.Windows.Controls.Border)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
