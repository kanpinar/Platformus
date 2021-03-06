﻿// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Platformus.Barebone;
using Platformus.Menus.Data.Entities;

namespace Platformus.Menus
{
  public class MenuCreatedEventHandler : IMenuCreatedEventHandler
  {
    public int Priority => 1000;

    public void HandleEvent(IRequestHandler requestHandler, Menu menu)
    {
      new SerializationManager(requestHandler).SerializeMenu(menu);
    }
  }
}