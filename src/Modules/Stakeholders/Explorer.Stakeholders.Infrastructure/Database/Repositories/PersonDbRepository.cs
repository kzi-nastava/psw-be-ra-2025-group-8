using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class PersonDbRepository : ICrudRepository<Person>
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Person> _dbSet;

    public PersonDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Person>();
    }

    public PagedResult<Person> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public Person Get(long id)
    {
        // Try find by primary key Id first
        var entity = _dbSet.Find(id);
        if (entity != null) return entity;

        // Fallback: some tests/services pass UserId where Person.Id is expected — try to find by UserId
        entity = _dbSet.FirstOrDefault(p => p.UserId == id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        return entity;
    }

    public Person Create(Person entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public Person Update(Person entity)
    {
        try
        {
            DbContext.Update(entity);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return entity;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}