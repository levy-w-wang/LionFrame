using LionFrame.CoreCommon.CustomException;
using LionFrame.Domain.BaseDomain;
using LionFrame.Model;
using LionFrame.Model.ResponseDto.ResultModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LionFrame.Data.BasicData
{
    /// <summary>
    /// 所以dao方法继承此方法
    /// </summary>
    public abstract class BaseData : IBaseData
    {
        /// <summary>
        /// 属性注入数据库上下文
        /// </summary>
        public LionDbContext CurrentDbContext { get; set; }

        /// <summary>
        /// 建议每个只查询的方法 加上关闭数据跟踪
        /// </summary>
        public void CloseTracking()
        {
            CurrentDbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        #region 增删改的公共方法

        public T Add<T>(T entity) where T : BaseModel, new()
        {
            CurrentDbContext.Set<T>().Add(entity);
            //DbContext.Entry(entity).State = EntityState.Added;
            return entity;
        }
        public List<T> Add<T>(List<T> entity) where T : BaseModel, new()
        {
            CurrentDbContext.Set<T>().AddRange(entity);
            //DbContext.Entry(entity).State = EntityState.Added;
            return entity;
        }

        public void Update<T>(T entity) where T : BaseModel, new()
        {
            CurrentDbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete<T>(params object[] keyValues) where T : BaseModel, new()
        {
            var entity = CurrentDbContext.Set<T>().Find(keyValues);
            CurrentDbContext.Entry(entity).State = EntityState.Deleted;
        }

        #endregion

        #region 查询方法

        /// <summary>
        /// 查看是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anyLambda"></param>
        /// <returns></returns>
        public bool Exist<T>(Expression<Func<T, bool>> anyLambda) where T : BaseModel, new()
        {
            return CurrentDbContext.Set<T>().Any(anyLambda);
        }

        /// <summary>
        /// 根据主键得到数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public T Find<T>(params object[] keyValues) where T : BaseModel, new()
        {
            return CurrentDbContext.Set<T>().Find(keyValues);
        }

        /// <summary>
        /// 得到条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="countLambda"></param>
        /// <returns></returns>
        public int Count<T>(Expression<Func<T, bool>> countLambda) where T : BaseModel, new()
        {
            return CurrentDbContext.Set<T>().AsNoTracking().Count(countLambda);
        }

        /// <summary>
        /// 获取第一个或默认的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstLambda"></param>
        /// <returns></returns>
        public T First<T>(Expression<Func<T, bool>> firstLambda) where T : BaseModel, new()
        {
            return CurrentDbContext.Set<T>().FirstOrDefault(firstLambda);
        }

        /// <summary>
        /// 根据某个条件  排序后 获取第一条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOrderKey"></typeparam>
        /// <param name="match"></param>
        /// <param name="orderFun"></param>
        /// <returns></returns>
        public T FirstOrderByDesc<T, TOrderKey>(Expression<Func<T, bool>> match, Expression<Func<T, TOrderKey>> orderFun) where T : BaseModel, new()
        {
            return CurrentDbContext.Set<T>().Where(match).OrderByDescending(orderFun).FirstOrDefault();
        }

        /// <summary>
        /// 得到IQueryable数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public IQueryable<T> LoadEntities<T>(Expression<Func<T, bool>> whereLambda = null) where T : BaseModel, new()
        {
            if (whereLambda == null)
            {
                return CurrentDbContext.Set<T>().AsQueryable();
            }
            return CurrentDbContext.Set<T>().Where(whereLambda).AsQueryable();
        }

        /// <summary>
        /// 从某个表中获取分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageCount"></param>
        /// <param name="whereLambda"></param>
        /// <param name="isAsc"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public List<T> LoadPageEntities<T, TKey>(int currentPage, int pageSize, out int totalCount, out int pageCount, Expression<Func<T, bool>> whereLambda,
            bool isAsc, Expression<Func<T, TKey>> orderBy) where T : BaseModel, new()
        {
            currentPage = currentPage < 1 ? 1 : currentPage;
            pageSize = pageSize < 1 ? 20 : pageSize;
            var temp = CurrentDbContext.Set<T>().AsNoTracking().Where(whereLambda); //去掉.AsQueryable().AsNoTracking()，将下面改为

            totalCount = temp.Count();
            pageCount = (int)Math.Ceiling((double)totalCount / pageSize);
            if (isAsc)
            {
                return temp.OrderBy(orderBy)
                    .Skip(pageSize * (currentPage - 1))
                    .Take(pageSize).Select(t => new T()).ToList(); //去掉.AsQueryable()，添加.select(t=>new Dto()).ToList()
            }

            return temp.OrderByDescending(orderBy)
                .Skip(pageSize * (currentPage - 1))
                .Take(pageSize).Select(t => new T()).ToList(); //.select(t=>new Dto()).ToList()

        }

        /// <summary>
        /// 返回分页模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="whereLambda"></param>
        /// <param name="isAsc"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public PageResponse<T> LoadPageEntities<T, TKey>(int currentPage, int pageSize, Expression<Func<T, bool>> whereLambda, bool isAsc, Expression<Func<T, TKey>> orderBy) where T : BaseModel, new()
        {
            currentPage = currentPage < 1 ? 1 : currentPage;
            pageSize = pageSize < 1 ? 20 : pageSize;
            var temp = CurrentDbContext.Set<T>().AsNoTracking().Where(whereLambda); //去掉.AsQueryable().AsNoTracking()，将下面改为

            var rest = new PageResponse<T>();
            rest.CurrentPage = currentPage;
            rest.PageSize = pageSize;
            rest.RecordTotal = temp.Count();//记录总条数时，自动设置了总页数
            if (isAsc)
            {
                rest.Data = temp.OrderBy(orderBy)
                     .Skip(pageSize * (currentPage - 1))
                     .Take(pageSize).Select(t => new T()).ToList(); //去掉.AsQueryable()，添加.select(t=>new Dto()).ToList()
            }

            rest.Data = temp.OrderByDescending(orderBy)
                .Skip(pageSize * (currentPage - 1))
                .Take(pageSize).Select(t => new T()).ToList(); //.select(t=>new Dto()).ToList()

            return rest;
        }

        /// <summary>
        /// 将查询出来的数据 转换成IQueryable，然后进行分页   不跟踪数据状态
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <typeparam name="TKey">根据哪个字段排序（必须）</typeparam>
        /// <param name="query">数据集</param>
        /// <param name="currentPage">页数</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="totalCount">总条数</param>
        /// <param name="pageCount">总页数</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>IQueryable分页结果</returns>
        public IQueryable<T> LoadPageEntities<T, TKey>(IQueryable<T> query, int currentPage, int pageSize, out int totalCount, out int pageCount, bool isAsc, Expression<Func<T, TKey>> orderBy) where T : class, new()
        {
            currentPage = currentPage < 1 ? 1 : currentPage;
            pageSize = pageSize < 1 ? 20 : pageSize;
            IQueryable<T> temp = query.AsNoTracking();
            totalCount = temp.Count();
            pageCount = (int)Math.Ceiling((double)totalCount / pageSize);
            if (isAsc)
            {
                temp = temp.OrderBy(orderBy)
                           .Skip(pageSize * (currentPage - 1))
                           .Take(pageSize).AsQueryable();
            }
            else
            {
                temp = temp.OrderByDescending(orderBy)
                          .Skip(pageSize * (currentPage - 1))
                          .Take(pageSize).AsQueryable();
            }
            return temp;
        }

        /// <summary>
        /// 将查询出来的数据 转换成IQueryable，然后进行分页   不跟踪数据状态
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <typeparam name="TKey">根据哪个字段排序（必须）</typeparam>
        /// <param name="query">数据集</param>
        /// <param name="currentPage">页数</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>PageResponse分页结果</returns>
        public PageResponse<T> LoadPageEntities<T, TKey>(IQueryable<T> query, int currentPage, int pageSize, bool isAsc, Expression<Func<T, TKey>> orderBy) where T : class, new()
        {
            currentPage = currentPage < 1 ? 1 : currentPage;
            pageSize = pageSize < 1 ? 20 : pageSize;
            var rest = new PageResponse<T>();
            IQueryable<T> temp = query.AsNoTracking();
            rest.PageSize = pageSize;
            rest.CurrentPage = currentPage;
            rest.RecordTotal = temp.Count();
            if (isAsc)
            {
                rest.Data = temp.OrderBy(orderBy)
                    .Skip(pageSize * (currentPage - 1))
                    .Take(pageSize).ToList();
            }
            else
            {
                rest.Data = temp.OrderByDescending(orderBy)
                    .Skip(pageSize * (currentPage - 1))
                    .Take(pageSize).ToList();
            }
            return rest;
        }

        /// <summary>
        /// 自带事务，调用此方法保存
        /// </summary>
        public int SaveChanges()
        {
            try
            {
                return CurrentDbContext.SaveChanges();
            }
            catch (DbException ex)
            {
                throw new CustomSystemException($"数据库保存失败!{ex.Message}", ResponseCode.DbEx);
            }
            catch (Exception ex)
            {
                throw new CustomSystemException($"数据库保存失败!{ex.Message}", ResponseCode.DbEx);
            }
        }

        public void Dispose()
        {
            this.CurrentDbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        #region 增删改的公共方法

        public async Task<T> AddAsync<T>(T entity) where T : BaseModel, new()
        {
            await CurrentDbContext.Set<T>().AddAsync(entity);
            //DbContext.Entry(entity).State = EntityState.Added;
            return entity;
        }
        public async Task<List<T>> AddAsync<T>(List<T> entity) where T : BaseModel, new()
        {
            await CurrentDbContext.Set<T>().AddRangeAsync(entity);
            //DbContext.Entry(entity).State = EntityState.Added;
            return entity;
        }


        public async Task DeleteAsync<T>(params object[] keyValues) where T : BaseModel, new()
        {
            var entity = await CurrentDbContext.Set<T>().FindAsync(keyValues);
            CurrentDbContext.Entry(entity).State = EntityState.Deleted;
        }
        
        #endregion

        #region 查询方法

        /// <summary>
        /// 查看是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anyLambda"></param>
        /// <returns></returns>
        public async Task<bool> ExistAsync<T>(Expression<Func<T, bool>> anyLambda) where T : BaseModel, new()
        {
            return await CurrentDbContext.Set<T>().AnyAsync(anyLambda);
        }

        /// <summary>
        /// 根据主键得到数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public async Task<T> FindAsync<T>(params object[] keyValues) where T : BaseModel, new()
        {
            return await CurrentDbContext.Set<T>().FindAsync(keyValues);
        }

        /// <summary>
        /// 得到条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="countLambda"></param>
        /// <returns></returns>
        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> countLambda) where T : BaseModel, new()
        {
            return await CurrentDbContext.Set<T>().AsNoTracking().CountAsync(countLambda);
        }

        /// <summary>
        /// 获取第一个或默认的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstLambda"></param>
        /// <returns></returns>
        public async Task<T> FirstAsync<T>(Expression<Func<T, bool>> firstLambda) where T : BaseModel, new()
        {
            return await CurrentDbContext.Set<T>().FirstOrDefaultAsync(firstLambda);
        }

        /// <summary>
        /// 根据某个条件  排序后 获取第一条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOrderKey"></typeparam>
        /// <param name="match"></param>
        /// <param name="orderFun"></param>
        /// <returns></returns>
        public async Task<T> FirstOrderByDescAsync<T, TOrderKey>(Expression<Func<T, bool>> match, Expression<Func<T, TOrderKey>> orderFun) where T : BaseModel, new()
        {
            return await CurrentDbContext.Set<T>().Where(match).OrderByDescending(orderFun).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 返回分页模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="whereLambda"></param>
        /// <param name="isAsc"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public async Task<PageResponse<T>> LoadPageEntitiesAsync<T, TKey>(int currentPage, int pageSize, Expression<Func<T, bool>> whereLambda, bool isAsc, Expression<Func<T, TKey>> orderBy) where T : BaseModel, new()
        {
            currentPage = currentPage < 1 ? 1 : currentPage;
            pageSize = pageSize < 1 ? 20 : pageSize;
            var temp = CurrentDbContext.Set<T>().AsNoTracking().Where(whereLambda); //去掉.AsQueryable().AsNoTracking()，将下面改为

            var rest = new PageResponse<T>();
            rest.CurrentPage = currentPage;
            rest.PageSize = pageSize;
            rest.RecordTotal = await temp.CountAsync();//记录总条数时，自动设置了总页数
            if (isAsc)
            {
                rest.Data = await temp.OrderBy(orderBy)
                     .Skip(pageSize * (currentPage - 1))
                     .Take(pageSize).Select(t => new T()).ToListAsync(); //去掉.AsQueryable()，添加.select(t=>new Dto()).ToList()
            }

            rest.Data = await temp.OrderByDescending(orderBy)
                .Skip(pageSize * (currentPage - 1))
                .Take(pageSize).Select(t => new T()).ToListAsync(); //.select(t=>new Dto()).ToList()

            return rest;
        }


        /// <summary>
        /// 将查询出来的数据 转换成IQueryable，然后进行分页   不跟踪数据状态
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <typeparam name="TKey">根据哪个字段排序（必须）</typeparam>
        /// <param name="query">数据集</param>
        /// <param name="currentPage">页数</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>PageResponse分页结果</returns>
        public async Task<PageResponse<T>> LoadPageEntitiesAsync<T, TKey>(IQueryable<T> query, int currentPage, int pageSize, bool isAsc, Expression<Func<T, TKey>> orderBy) where T : class, new()
        {
            currentPage = currentPage < 1 ? 1 : currentPage;
            pageSize = pageSize < 1 ? 20 : pageSize;
            var rest = new PageResponse<T>();
            IQueryable<T> temp = query.AsNoTracking();
            rest.PageSize = pageSize;
            rest.CurrentPage = currentPage;
            rest.RecordTotal = await temp.CountAsync();
            if (isAsc)
            {
                rest.Data = await temp.OrderBy(orderBy)
                    .Skip(pageSize * (currentPage - 1))
                    .Take(pageSize).ToListAsync();
            }
            else
            {
                rest.Data = await temp.OrderByDescending(orderBy)
                    .Skip(pageSize * (currentPage - 1))
                    .Take(pageSize).ToListAsync();
            }
            return rest;
        }

        /// <summary>
        /// 自带事务，调用此方法保存
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await CurrentDbContext.SaveChangesAsync();
            }
            catch (DbException ex)
            {
                throw new CustomSystemException($"数据库保存失败!{ex.Message}", ResponseCode.DbEx);
            }
            catch (Exception ex)
            {
                throw new CustomSystemException($"数据库保存失败!{ex.Message}", ResponseCode.DbEx);
            }
        }

        #endregion
    }
}
