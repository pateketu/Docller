﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.544
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Docller.Core.Resources {
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
    public class StringResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal StringResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Docller.Core.Resources.StringResources", typeof(StringResources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CustomerId is not valid, its value must be more than Zero.
        /// </summary>
        public static string ArgumnetException_InvalidCustomerId {
            get {
                return ResourceManager.GetString("ArgumnetException_InvalidCustomerId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expression must contain a MemberAccessExpression to a property ( t =&gt;t.Property)..
        /// </summary>
        public static string ExceptionArgumentMustBePropertyExpression {
            get {
                return ResourceManager.GetString("ExceptionArgumentMustBePropertyExpression", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Get Request for Json is not allowed.
        /// </summary>
        public static string JsonRequest_GetNotAllowed {
            get {
                return ResourceManager.GetString("JsonRequest_GetNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Context was null in {0}.
        /// </summary>
        public static string NoDcollerContext {
            get {
                return ResourceManager.GetString("NoDcollerContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to For &quot;Member&quot; Federation FederationKey must be greater than Zero.
        /// </summary>
        public static string Repository_InvalidMemberFederation {
            get {
                return ResourceManager.GetString("Repository_InvalidMemberFederation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ConnectionString for Respostiory is invalid.
        /// </summary>
        public static string Repository_NoConnectionString {
            get {
                return ResourceManager.GetString("Repository_NoConnectionString", resourceCulture);
            }
        }
    }
}
