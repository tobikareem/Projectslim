using Microsoft.Extensions.Logging;
using Slim.Core.Model;
using Slim.Data.Context;
using Slim.Data.Entity;
using Slim.Shared.Interfaces.Repo;
using Slim.Shared.Interfaces.Serv;

namespace Slim.Shared.Repositories
{
    public class CommentRepository : IBaseStore<Comment>
    {
        private readonly SlimDbContext _context;
        private readonly ILogger<CommentRepository> _logger;
        private readonly ICacheService _cacheService;


        public CommentRepository(SlimDbContext context, ILogger<CommentRepository> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
        }
        
        public void AddEntity(Comment entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.Comments.Add(entity);
                _context.SaveChanges();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Add Comment Entity");
                throw;
            }
            finally
            {
                if (hasCache)
                {
                    _cacheService.Remove(CacheKey.GetComments);
                }
            }
        }

        public void UpdateEntity(Comment entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.Comments.Update(entity);
                _context.SaveChanges();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Update Comment Entity");
                throw;
            }
            finally
            {
                if (hasCache)
                {
                    _cacheService.Remove(CacheKey.GetComments);
                }
            }
        }

        public Comment GetEntity(int id)
        {
            try
            {
                return _context.Comments.FirstOrDefault(x => x.Id == id) ?? new Comment();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Get Comment Entity");
                throw;
            }
        }

        public IEnumerable<Comment> GetAll()
        {
            try
            {
                return _context.Comments.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Get All Comment Entity");
                throw;
            }

        }

        public void DeleteEntity(Comment entity, CacheKey cacheKey = CacheKey.None, bool hasCache = false)
        {
            try
            {
                _context.Comments.Remove(entity);
                _context.SaveChanges();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in Delete Comment Entity");
                throw;
            }
            finally
            {
                if (hasCache)
                {
                    _cacheService.Remove(CacheKey.GetComments);
                }
            }
        }
    }
}
