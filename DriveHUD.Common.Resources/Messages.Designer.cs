﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DriveHUD.Common.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DriveHUD.Common.Resources.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cancel.
        /// </summary>
        internal static string Message_Migration0017_Cancel {
            get {
                return ResourceManager.GetString("Message_Migration0017_Cancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Rebuild stats.
        /// </summary>
        internal static string Message_Migration0017_Rebuild {
            get {
                return ResourceManager.GetString("Message_Migration0017_Rebuild", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Rebuilding stats.
        /// </summary>
        internal static string Message_Migration0017_Status {
            get {
                return ResourceManager.GetString("Message_Migration0017_Status", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New stats have been added to DriveHUD. You&apos;ll need to rebuild your stats to see the proper value of these new stats. This operation might take some time (1 - 6 minutes depending on database size), but it isn&apos;t mandatory to perform. You can skip it by clicking on Cancel. You can run the rebuild stats function at a later time from the settings menu.
        ///
        ///New stats:
        ///
        ///LIMP
        ///
        ///    Limp%
        ///    Limp EP%
        ///    Limp MP%
        ///    Limp CO%
        ///    Limp BTN%
        ///    Limp SB%
        ///    Limp/Call%
        ///
        ///COLD CALL
        ///
        ///    Cold Call EP%
        ///    C [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Message_Migration0017_Text {
            get {
                return ResourceManager.GetString("Message_Migration0017_Text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DriveHUD: Action is required.
        /// </summary>
        internal static string Message_Migration0017_Title {
            get {
                return ResourceManager.GetString("Message_Migration0017_Title", resourceCulture);
            }
        }
    }
}
