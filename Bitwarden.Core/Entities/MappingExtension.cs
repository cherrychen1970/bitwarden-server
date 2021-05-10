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
    static public class MapperExtension
    {
        static public IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination, TMember>(
            this IMappingExpression<TSource, TDestination> expression,
            Expression<Func<TDestination, TMember>> member
            )
        {
            return expression.ForMember(member, opt => opt.Ignore());
        }
    }
}