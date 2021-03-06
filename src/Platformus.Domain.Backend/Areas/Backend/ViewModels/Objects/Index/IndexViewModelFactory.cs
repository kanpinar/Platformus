﻿// Copyright © 2015 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Platformus.Barebone;
using Platformus.Barebone.Backend.ViewModels.Shared;
using Platformus.Domain.Backend.ViewModels.Shared;
using Platformus.Domain.Data.Abstractions;
using Platformus.Domain.Data.Entities;
using Platformus.Globalization;
using Platformus.Globalization.Backend.ViewModels;

namespace Platformus.Domain.Backend.ViewModels.Objects
{
  public class IndexViewModelFactory : ViewModelFactoryBase
  {
    private IEnumerable<Member> classMembers;

    public IndexViewModelFactory(IRequestHandler requestHandler)
      : base(requestHandler)
    {
    }

    public IndexViewModel Create(int? classId, int? objectId, string orderBy, string direction, int skip, int take, string filter)
    {
      IClassRepository classRepository = this.RequestHandler.Storage.GetRepository<IClassRepository>();
      IObjectRepository objectRepository = this.RequestHandler.Storage.GetRepository<IObjectRepository>();
      ISerializedObjectRepository serializedObjectRepository = this.RequestHandler.Storage.GetRepository<ISerializedObjectRepository>();

      if (classId != null && string.IsNullOrEmpty(orderBy))
        orderBy = this.GetDefaultMember((int)classId)?.Code;

      return new IndexViewModel()
      {
        Class = classId == null ? null : new ClassViewModelFactory(this.RequestHandler).Create(
          classRepository.WithKey((int)classId)
        ),
        ClassesByAbstractClasses = this.GetClassesByAbstractClasses(),
        Grid = classId == null ? null : new GridViewModelFactory(this.RequestHandler).Create(
          orderBy, direction, skip, take, objectRepository.CountByClassId((int)classId),
          this.GetGridColumns((int)classId),
          objectId == null ?
            serializedObjectRepository.FilteredByCultureIdAndClassId(
              CultureManager.GetCurrentCulture(this.RequestHandler.Storage).Id,
              (int)classId,
              this.GetParams((int)classId, orderBy, direction, skip, take, filter)
            ).ToList().Select(so => new ObjectViewModelFactory(this.RequestHandler).Create(so.ObjectId)) :
            serializedObjectRepository.FilteredByCultureIdAndClassIdAndObjectId(
              CultureManager.GetCurrentCulture(this.RequestHandler.Storage).Id,
              (int)classId,
              (int)objectId,
              this.GetParams((int)classId, orderBy, direction, skip, take, filter)
            ).ToList().Select(so => new ObjectViewModelFactory(this.RequestHandler).Create(so.ObjectId)),
          "_Object"
        )
      };
    }

    private Params GetParams(int classId, string orderBy, string direction, int skip, int take, string filter)
    {
      Member member = this.RequestHandler.Storage.GetRepository<IMemberRepository>().WithClassIdAndCodeInlcudingParent((int)classId, orderBy);

      return new Params(
        sorting: member == null ? null : new Sorting(this.RequestHandler.Storage.GetRepository<IDataTypeRepository>().WithKey((int)member.PropertyDataTypeId).StorageDataType, member.Id, direction),
        paging: new Paging(skip, take),
        filtering: string.IsNullOrEmpty(filter) ? null : new Filtering(filter)
      );
    }

    private Member GetDefaultMember(int classId)
    {
      return this.GetClassMembers(classId).FirstOrDefault();
    }

    private IEnumerable<Member> GetClassMembers(int classId)
    {
      if (this.classMembers == null)
        this.classMembers = this.RequestHandler.Storage.GetRepository<IMemberRepository>().FilteredByClassIdInlcudingParentPropertyVisibleInList(classId);

      return this.classMembers;
    }

    private IDictionary<ClassViewModel, IEnumerable<ClassViewModel>> GetClassesByAbstractClasses()
    {
      Dictionary<ClassViewModel, IEnumerable<ClassViewModel>> classesByAbstractClasses = new Dictionary<ClassViewModel, IEnumerable<ClassViewModel>>();

      foreach (Class abstractClass in this.RequestHandler.Storage.GetRepository<IClassRepository>().Abstract().ToList())
        classesByAbstractClasses.Add(
          new ClassViewModelFactory(this.RequestHandler).Create(abstractClass),
          this.RequestHandler.Storage.GetRepository<IClassRepository>().FilteredByClassId(abstractClass.Id).Select(
            c => new ClassViewModelFactory(this.RequestHandler).Create(c)
          )
        );

      classesByAbstractClasses.Add(
        new ClassViewModel() { PluralizedName = "Others" },
        this.RequestHandler.Storage.GetRepository<IClassRepository>().FilteredByClassId(null).ToList().Select(
          c => new ClassViewModelFactory(this.RequestHandler).Create(c)
        )
      );

      return classesByAbstractClasses;
    }

    private IEnumerable<GridColumnViewModel> GetGridColumns(int classId)
    {
      List<GridColumnViewModel> gridColumns = new List<GridColumnViewModel>();

      gridColumns.AddRange(
        this.GetClassMembers(classId).Select(m => new GridColumnViewModelFactory(this.RequestHandler).Create(m.Name, m.Code))
      );

      foreach (Member member in this.RequestHandler.Storage.GetRepository<IMemberRepository>().FilteredByRelationClassIdRelationSingleParent(classId).ToList())
      {
        Class @class = this.RequestHandler.Storage.GetRepository<IClassRepository>().WithKey(member.ClassId);

        gridColumns.Add(new GridColumnViewModelFactory(this.RequestHandler).Create(@class.PluralizedName));
      }

      gridColumns.Add(new GridColumnViewModelFactory(this.RequestHandler).CreateEmpty());
      return gridColumns;
    }
  }
}