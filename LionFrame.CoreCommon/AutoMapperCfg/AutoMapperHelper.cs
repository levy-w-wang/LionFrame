using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Autofac;
using AutoMapper;

namespace LionFrame.CoreCommon.AutoMapperCfg
{
    public static class AutoMapperHelper
    {
        private static Mapper _mapper = LionWeb.AutofacContainer.Resolve<Mapper>();

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TDestination>(this object source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public static List<TDestination> MapToList<TDestination>(this IEnumerable source)
        {
            return _mapper.Map<List<TDestination>>(source);
        }

        public static List<TDestination> MapToList<TSource, TDestination>(this IEnumerable<TSource> source)
        {
            return _mapper.Map<List<TDestination>>(source);
        }

    }
}
