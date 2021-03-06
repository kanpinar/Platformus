﻿// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Platformus.Barebone.Backend;
using Platformus.Globalization.Backend.ViewModels;

namespace Platformus.Domain.Backend.ViewModels.DataSources
{
  public class CreateOrEditViewModel : ViewModelBase
  {
    public int? Id { get; set; }
    public int MicrocontrollerId { get; set; }

    [Display(Name = "Code")]
    [Required]
    [StringLength(32)]
    public string Code { get; set; }

    [Display(Name = "C# class name")]
    [Required]
    [StringLength(128)]
    public string CSharpClassName { get; set; }
    public IEnumerable<Option> CSharpClassNameOptions { get; set; }

    public string Parameters { get; set; }
    public IEnumerable<dynamic> DataSources { get; set; }
  }
}