﻿// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Platformus.Barebone;
using Platformus.Domain.Data.Abstractions;
using Platformus.Domain.Data.Entities;

namespace Platformus.Domain
{
  public class StronglyTypedObjectMapper
  {
    private IRequestHandler requestHandler;
    private IClassRepository classRepository;
    private IObjectRepository objectRepository;

    public StronglyTypedObjectMapper(IRequestHandler requestHandler)
    {
      this.requestHandler = requestHandler;
      this.classRepository = this.requestHandler.Storage.GetRepository<IClassRepository>();
      this.objectRepository = this.requestHandler.Storage.GetRepository<IObjectRepository>();
    }

    public T WithKey<T>(int id)
    {
      Object @object = this.objectRepository.WithKey(id);
      StronglyTypedObjectBuilder<T> stronglyTypedObjectBuilder = new StronglyTypedObjectBuilder<T>();

      new ObjectDirector(this.requestHandler).ConstructObject(stronglyTypedObjectBuilder, @object);
      return stronglyTypedObjectBuilder.Build();
    }

    public IEnumerable<T> All<T>()
    {
      Class @class = this.classRepository.WithCode(typeof(T).Name);
      IEnumerable<Object> objects = this.objectRepository.FilteredByClassId(@class.Id);
      ObjectDirector objectDirector = new ObjectDirector(this.requestHandler);

      return objects.Select(
        o =>
        {
          StronglyTypedObjectBuilder<T> stronglyTypedObjectBuilder = new StronglyTypedObjectBuilder<T>();

          objectDirector.ConstructObject(stronglyTypedObjectBuilder, o);
          return stronglyTypedObjectBuilder.Build();
        }
      );
    }

    public void Create<T>(T @object)
    {
      ObjectManipulator objectManipulator = new ObjectManipulator(this.requestHandler);

      objectManipulator.BeginCreateTransaction<T>();

      foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
        objectManipulator.SetPropertyValue(propertyInfo.Name, propertyInfo.GetValue(@object));

      objectManipulator.CommitTransaction();
    }

    public void Edit<T>(T @object)
    {
      int id = this.GetId<T>(@object);

      if (id == 0)
        return;

      ObjectManipulator objectManipulator = new ObjectManipulator(this.requestHandler);

      objectManipulator.BeginEditTransaction<T>(id);

      foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
        objectManipulator.SetPropertyValue(propertyInfo.Name, propertyInfo.GetValue(@object));

      objectManipulator.CommitTransaction();
    }

    public void Delete<T>(T @object)
    {
      int id = this.GetId<T>(@object);

      if (id != 0)
      {
        this.requestHandler.Storage.GetRepository<IObjectRepository>().Delete(id);
        this.requestHandler.Storage.Save();
      }
    }

    private int GetId<T>(T @object)
    {
      int id = 0;
      PropertyInfo idPropertyInfo = typeof(T).GetProperty("Id");

      if (idPropertyInfo != null)
        id = (int)idPropertyInfo.GetValue(@object);

      return id;
    }
  }
}