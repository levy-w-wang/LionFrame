using LionFrame.Basic.AutofacDependency;
using LionFrame.Domain.BaseDomain;
using LionFrame.Model.ResponseDto.ResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LionFrame.Data.BasicData
{
    public interface IBaseData : IScopedDependency
    {
        T Add<T>(T entity) where T : BaseModel, new();

        List<T> Add<T>(List<T> entity) where T : BaseModel, new();

        void Delete<T>(params object[] keyValues) where T : BaseModel, new();
        /// <summary>
        /// 设置为更新状态  需调用保存方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Update<T>(T entity) where T : BaseModel, new();

        bool Exist<T>(Expression<Func<T, bool>> anyLambda) where T : BaseModel, new();

        T Find<T>(params object[] keyValues) where T : BaseModel, new();

        int Count<T>(Expression<Func<T, bool>> countLambda) where T : BaseModel, new();

        T First<T>(Expression<Func<T, bool>> firstLambda) where T : BaseModel, new();
        /// <summary>
        /// 根据某个条件  排序后 获取第一条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOrderKey"></typeparam>
        /// <param name="match"></param>
        /// <param name="orderFun"></param>
        /// <returns></returns>
        T FirstOrderByDesc<T, TOrderKey>(Expression<Func<T, bool>> match, Expression<Func<T, TOrderKey>> orderFun) where T : BaseModel, new();
        IQueryable<T> LoadEntities<T>(Expression<Func<T, bool>> whereLambda = null) where T : BaseModel, new();

        List<T> LoadPageEntities<T, TKey>(int pageIndex, int pageSize,
            out int totalCount, out int pageCount,
            Expression<Func<T, bool>> whereLambda, bool isAsc, Expression<Func<T, TKey>> orderBy) where T : BaseModel, new();

        PageResponse<T> LoadPageEntities<T, TKey>(int pageIndex, int pageSize,
            Expression<Func<T, bool>> whereLambda, bool isAsc, Expression<Func<T, TKey>> orderBy) where T : BaseModel, new();

        IQueryable<T> LoadPageEntities<T, TKey>(IQueryable<T> query, int pageIndex, int pageSize,
            out int totalCount, out int pageCount, bool isAsc, Expression<Func<T, TKey>> orderBy) where T : class, new();

        PageResponse<T> LoadPageEntities<T, TKey>(IQueryable<T> query, int pageIndex, int pageSize,
            bool isAsc, Expression<Func<T, TKey>> orderBy) where T : class, new();

        int SaveChanges();

        Task<T> AddAsync<T>(T entity) where T : BaseModel, new();

        Task<List<T>> AddAsync<T>(List<T> entity) where T : BaseModel, new();

        Task DeleteAsync<T>(params object[] keyValues) where T : BaseModel, new();

        Task<bool> ExistAsync<T>(Expression<Func<T, bool>> anyLambda) where T : BaseModel, new();

        Task<T> FindAsync<T>(params object[] keyValues) where T : BaseModel, new();

        Task<int> CountAsync<T>(Expression<Func<T, bool>> countLambda) where T : BaseModel, new();

        Task<T> FirstAsync<T>(Expression<Func<T, bool>> firstLambda) where T : BaseModel, new();
        /// <summary>
        /// 根据某个条件  排序后 获取第一条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOrderKey"></typeparam>
        /// <param name="match"></param>
        /// <param name="orderFun"></param>
        /// <returns></returns>
        Task<T> FirstOrderByDescAsync<T, TOrderKey>(Expression<Func<T, bool>> match, Expression<Func<T, TOrderKey>> orderFun) where T : BaseModel, new();

        Task<PageResponse<T>> LoadPageEntitiesAsync<T, TKey>(int currentPage, int pageSize, Expression<Func<T, bool>> whereLambda, bool isAsc, Expression<Func<T, TKey>> orderBy) where T : BaseModel, new();

        Task<PageResponse<T>> LoadPageEntitiesAsync<T, TKey>(IQueryable<T> query, int currentPage, int pageSize, bool isAsc, Expression<Func<T, TKey>> orderBy) where T : class, new();

        Task<int> SaveChangesAsync();
    }
}
