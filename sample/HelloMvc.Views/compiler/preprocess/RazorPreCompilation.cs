// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.Razor.Precompilation;
using Microsoft.Dnx.Compilation.CSharp;
using Microsoft.Dnx.Runtime;

namespace HelloMvc.Views
{
    public class TagHelperPrecompilation : RazorPreCompileModule
    {
        public TagHelperPrecompilation()
        {
            GenerateSymbols = true;
        }

        //protected override bool EnablePreCompilation(BeforeCompileContext context)
        //{
        //    return true;
        //}
    }
}
