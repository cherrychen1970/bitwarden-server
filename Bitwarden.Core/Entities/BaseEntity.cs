using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using AutoMapper;
using DomainModels = Bit.Core.Models;
using Bit.Core.Enums;
using Bit.Core.Models.Data;

namespace Bit.Core.Entities
{
    public class BaseGuidEntity : DomainModels.IKey<Guid>,IGenerateKey
    {
        public Guid Id { get; private set; }
        public void SetNewId() {Id=Guid.NewGuid();}
        public void SetId(Guid id) {Id=id;}
    }
}