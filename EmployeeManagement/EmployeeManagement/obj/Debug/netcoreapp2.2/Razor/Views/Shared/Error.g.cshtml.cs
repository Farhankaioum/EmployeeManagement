#pragma checksum "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\Shared\Error.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "43e73ee0de692be2506c8d1efa88c78d633add1a"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared_Error), @"mvc.1.0.view", @"/Views/Shared/Error.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Shared/Error.cshtml", typeof(AspNetCore.Views_Shared_Error))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\_ViewImports.cshtml"
using EmployeeManagement.ViewModels;

#line default
#line hidden
#line 2 "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\_ViewImports.cshtml"
using EmployeeManagement.Models;

#line default
#line hidden
#line 3 "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#line 4 "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"43e73ee0de692be2506c8d1efa88c78d633add1a", @"/Views/Shared/Error.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"63b9ae8ef32eb7d366c669217136ee37f042d076", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared_Error : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 2 "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\Shared\Error.cshtml"
  
    ViewBag.Title = "Exception occured!";

#line default
#line hidden
            BeginContext(52, 4, true);
            WriteLiteral("<h2>");
            EndContext();
            BeginContext(57, 18, false);
#line 5 "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\Shared\Error.cshtml"
Write(ViewBag.ErrorTitle);

#line default
#line hidden
            EndContext();
            BeginContext(75, 7, true);
            WriteLiteral("</h2>\r\n");
            EndContext();
#line 6 "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\Shared\Error.cshtml"
 if (ViewBag.ErrorMessage != null)
{

#line default
#line hidden
            BeginContext(121, 18, true);
            WriteLiteral("    <h3>\r\n        ");
            EndContext();
            BeginContext(140, 20, false);
#line 9 "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\Shared\Error.cshtml"
   Write(ViewBag.ErrorMessage);

#line default
#line hidden
            EndContext();
            BeginContext(160, 13, true);
            WriteLiteral("\r\n    </h3>\r\n");
            EndContext();
#line 11 "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\Shared\Error.cshtml"
}
else
{

#line default
#line hidden
            BeginContext(185, 63, true);
            WriteLiteral("    <h3>Error occured, please contact our administration</h3>\r\n");
            EndContext();
#line 15 "G:\Farhan(Vacation)\NewWorld\Core(Prac)\Day_1(s)\EmployeeManagement\EmployeeManagement\Views\Shared\Error.cshtml"
}

#line default
#line hidden
            BeginContext(251, 6, true);
            WriteLiteral("\r\n\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
