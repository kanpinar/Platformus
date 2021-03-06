﻿// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using Platformus.Barebone;
using Platformus.Globalization;
using Platformus.Globalization.Data.Entities;
using Platformus.Menus.Data.Entities;

namespace Platformus.Menus
{
  public class MenuDeletedEventHandler : IMenuDeletedEventHandler
  {
    public int Priority => 1000;

    public void HandleEvent(IRequestHandler requestHandler, Menu menu)
    {
      foreach (Culture culture in CultureManager.GetNotNeutralCultures(requestHandler.Storage))
        requestHandler.HttpContext.RequestServices.GetService<ICache>().RemoveMenuViewComponentResult(menu.Code, culture.Code);
    }
  }
}